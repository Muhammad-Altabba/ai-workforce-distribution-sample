using System;
using System.Collections.Generic;
using System.Text;

namespace ORToolsRoutingAndScheduling
{
    public class WorkerAssignment
    {
        public List<Visit> Visits { get; set; }

        public KeyValuePair<int, int> TotalTime { get; set; }
        public void setTotalTime(long totalTime)
        {
            this.TotalTime = new KeyValuePair<int, int>((int)totalTime / 60, (int)totalTime % 60);
        }

        public WorkerAssignment()
        {
            this.Visits = new List<Visit>();
            this.TotalTime = new KeyValuePair<int, int>();
        }
    }
}
