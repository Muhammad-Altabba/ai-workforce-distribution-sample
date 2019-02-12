using Google.OrTools.ConstraintSolver;
using System;
using System.Collections.Generic;
using System.Text;

namespace ORToolsRoutingAndScheduling
{
    public class RoutingSolver
    {
        public readonly DataModel Data;
        private readonly LongLongToLong timeCallback;
        private readonly RoutingIndexManager manager;

        public RoutingSolver(DataModel data)
        {
            if (this.Data.GetTimeWindows().GetLength(0) != this.Data.GetTimeMatrix().GetLength(0))
                throw new System.ComponentModel.DataAnnotations.ValidationException("Travel Time Matrix and Time Windows Matrix must have the same length.");

            this.Data = data;

            // Create Routing Index Manager
            // [START index_manager]
            this.manager = new RoutingIndexManager(
                this.Data.GetTimeMatrix().GetLength(0), // Total Number of Sites
                this.Data.GetVehicleNumber(),
                this.Data.GetDepot());
            // [END index_manager]

            this.timeCallback = new TimeCallback(this.Data, manager);
        }

        protected RoutingModel ComposeRoutingModel(out List<int> failedNodes)
        {
            failedNodes = new List<int>();
            int numberOfSites = Data.GetTimeMatrix().GetLength(0);

            // Create Routing Model.
            // [START routing_model]
            RoutingModel routing = new RoutingModel(manager);
            // [END routing_model]

            // Define cost of each arc.
            // [START arc_cost]


            int transitCallbackIndex = routing.RegisterTransitCallback(timeCallback);

            routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);
            // [END arc_cost]

            // Add Distance constraint.
            // [START time_constraint]
            // Documentation at: https://github.com/google/or-tools/blob/3494afff17d3dc60daf5ebe6ff2ab4cbc7777163/ortools/constraint_solver/routing.h#L383
            routing.AddDimension(
                transitCallbackIndex, // transit callback
                Data.getAllowWaitingTime(), // allow waiting time
                Data.getMaximumWorkerCapacity(), // vehicle maximum capacities
                false,  // start cumul to zero
                "Time");
            RoutingDimension timeDimension = routing.GetMutableDimension("Time");
            // Add time window constraints for each location except depot
            // and 'copy' the slack var in the solution object (aka Assignment) to print it
            for (int i = 1; i < Data.GetTimeWindows().GetLength(0); ++i)
            {
                try
                {
                    long index = manager.NodeToIndex(i);
                    // TODO: To be replaced to allow mulible shifts similar to:
                    // https://gist.github.com/Muhammad-Altabba/5e52cc1aee98e88f11a01181341f630e#file-vrpsolver-py-L121
                    timeDimension.CumulVar(index).SetRange(
                        Data.GetTimeWindows()[i, 0],
                        Data.GetTimeWindows()[i, 1]);
                    routing.AddToAssignment(timeDimension.SlackVar(index));

                }
                catch (System.ApplicationException ex)
                {
                    //Possible cases: The site cannot be visited during the attendance time of the worker.
                    // Or the starting time is after the end time...
                    // Most likely a problem in timing for this node..
                    failedNodes.Add(i);
                    Console.WriteLine("(Note: " + ex.Message + " to add for node " + i + ". Could be because the site cannot be visited during the attendance time of the workers. Or there is a time inconsistency for this node.)");
                }
            }
            // Add time window constraints for each vehicle start node
            // and 'copy' the slack var in the solution object (aka Assignment) to print
            // it
            for (int i = 0; i < Data.GetVehicleNumber(); ++i)
            {
                long index = routing.Start(i);
                timeDimension.CumulVar(index).SetRange(
                    Data.GetTimeWindows()[0, 0],
                    Data.GetTimeWindows()[0, 1]);
                routing.AddToAssignment(timeDimension.SlackVar(index));
            }
            // [END time_constraint]

            for (int i = 0; i < numberOfSites; i++)
            {
                CpInt64Vector v = new CpInt64Vector();
                v.Add(manager.NodeToIndex(i));
                routing.AddDisjunction(v, Data.GetDemands()[i]);
            }
            return routing;
        }

        // [START Compose_Solution_Result]
        /// <summary>
        ///   Get Workers Assignment and Print the solution.
        /// </summary>
        protected WorkerAssignment[] getWorkersAssignment(
            in RoutingModel routing,
            in Assignment solution)
        {
            if (solution == null)
            {
                Console.WriteLine("No solution found!!!");
                return null;
            }

            Console.WriteLine("\nWorkers Assignments:");

            WorkerAssignment[] solutionObjects = new WorkerAssignment[Data.GetVehicleNumber()];

            // Console.WriteLine("Objective: {0}", solution.ObjectiveValue());
            RoutingDimension timeDimension = routing.GetMutableDimension("Time");
            // Inspect solution.
            long totalTime = 0;
            for (int i = 0; i < Data.GetVehicleNumber(); ++i)
            {
                Console.WriteLine("Worker {0}:", i);
                Console.Write("\t");
                long routeTime = 0;
                var index = routing.Start(i);
                while (routing.IsEnd(index) == false)
                {
                    var previousIndex = index;
                    solutionObjects[i] = new WorkerAssignment();

                    long oneRouteTime = routing.GetArcCostForVehicle(index, solution.Value(routing.NextVar(index)), i);

                    if (manager.IndexToNode(index) == 0)
                    {
                        Console.Write("Start: ");
                    }
                    else
                    {
                        Console.Write("Routing Time {0} -> ", oneRouteTime);

                        var timeVar = timeDimension.CumulVar(index);
                        var slackVar = timeDimension.SlackVar(index);
                        long minSlack = -1, maxSlack = -1;

                        // If it is set to 'true', sometime this exception is raised (depending on the data): System.AccessViolationException!
                        bool calculateSlack = false; 
                        if (calculateSlack)
                        {
                            try { minSlack = solution.Min(slackVar); } catch (Exception) { }
                            try { maxSlack = solution.Max(slackVar); } catch (Exception) { }
                            Console.Write("Go to site({0}) try to arrive between ({1},{2}), Slack({3},{4}) ",
                                manager.IndexToNode(index),
                                solution.Min(timeVar),
                                solution.Max(timeVar),
                                minSlack,
                                maxSlack);
                        }
                        else
                        {
                            Console.Write("Go to site({0}) try to arrive between ({1},{2}), ",
                                    manager.IndexToNode(index),
                                solution.Min(timeVar),
                                solution.Max(timeVar));
                        }

                        solutionObjects[i].Visits.Add(new Visit()
                        {
                            Site = manager.IndexToNode(index),
                            ArrivalRangeStart = new KeyValuePair<int, int>((int)solution.Min(timeVar) / 60, (int)solution.Min(timeVar) % 60),
                            ArrivalRangeEnd = new KeyValuePair<int, int>((int)solution.Max(timeVar) / 60, (int)solution.Max(timeVar) % 60),
                        });
                    }
                    index = solution.Value(routing.NextVar(index));
                    routeTime += oneRouteTime;
                }
                var endTimeVar = timeDimension.CumulVar(index);
                Console.WriteLine("Finish: take a rest ({1},{2})",
                    manager.IndexToNode(index),
                    solution.Min(endTimeVar),
                    solution.Max(endTimeVar));

                Console.WriteLine("\tTime of the route: {0}m", routeTime);
                totalTime += routeTime;
                solutionObjects[i].setTotalTime(routeTime);
            }
            Console.WriteLine("Total Distance of all routes: {0}m", totalTime);
            return solutionObjects;
        }
        // [END Compose_Solution_Result]


        public WorkforceDistributionSolution Solve()
        {
            // Setting first solution heuristic.
            // [START parameters]
            RoutingSearchParameters searchParameters =
              operations_research_constraint_solver.DefaultRoutingSearchParameters();
            searchParameters.FirstSolutionStrategy =
              FirstSolutionStrategy.Types.Value.PathCheapestArc;
            // [END parameters]
            List<int> failedToAddSites;
            RoutingModel routing = this.ComposeRoutingModel(out failedToAddSites);

            // Solve the problem.
            // [START solve]
            Assignment solution = routing.SolveWithParameters(searchParameters);

            GC.KeepAlive(this.timeCallback);

            // [END solve]

            // RoutingSolver.PrintSolution(data, routing, manager, solution);

            WorkerAssignment[] wa = this.getWorkersAssignment(routing, solution);

            WorkforceDistributionSolution distributionSolution = new WorkforceDistributionSolution();
            distributionSolution.FailedToAddSites = failedToAddSites;
            return distributionSolution;
        }
    }
}
