// The source code in the project is based on a modified copy of: 
//  https://github.com/google/or-tools/blob/0d8f52397810a022d149a7dc082017f88e8e8762/ortools/constraint_solver/samples/VrpTimeWindows.cs



// [START program]
// [START import]
using System;
using System.Collections.Generic;
using Google.OrTools.ConstraintSolver;

// [END import]


namespace ORToolsRoutingAndScheduling
{
    /// <summary>
    ///   Vehicle Routing Problem with Time Window.
    /// </summary>
    public class VrpTimeWindows
    {
        public static void Main(String[] args)
        {
            DataModel data = new DataModel();

            WorkforceDistributionSolution solution = new RoutingSolver(data).Solve();
        }
    }
    // [END program]
}