using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ORToolsRoutingAndScheduling
{
    public class WorkforceDistributionSolution
    {
        public WorkerAssignment[] WorkerAssignments { get; set; }
        public IEnumerable<int> GetUnvisitedSites()
        {
            if (this.WorkerAssignments != null)
            {
                return this.WorkerAssignments.SelectMany(wa => wa.Visits).Select(v => v.Site).Distinct();
            }
            else
                return null;
        }
        public List<int> FailedToAddSites { get; set; }
    }
}
