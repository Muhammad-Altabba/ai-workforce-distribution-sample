using System;
using System.Collections.Generic;
using System.Text;

namespace ORToolsRoutingAndScheduling
{
    public class Visit
    {

        // Hours, Minutes
        public KeyValuePair<int, int> RoutingTime { get; set; }
        // Set starting time in minutes
        public void setRoutingTime(long routingTime)
        {
            this.RoutingTime = new KeyValuePair<int, int>((int)routingTime / 60, (int)routingTime % 60);
        }

        // Hours, Minutes
        public KeyValuePair<int, int> ArrivalRangeStart { get; set; }
        // Set starting time in minutes
        public void setArrivalRangeStart(long time)
        {
            this.ArrivalRangeStart = new KeyValuePair<int, int>((int)time / 60, (int)time % 60);
        }

        // Hours, Minutes
        public KeyValuePair<int, int> ArrivalRangeEnd { get; set; }
        // Set total time in minutes
        public void setArrivalRangeEnd(long time)
        {
            this.ArrivalRangeEnd = new KeyValuePair<int, int>((int)time / 60, (int)time % 60);
        }

        public int Site { get; set; }
    }
}
