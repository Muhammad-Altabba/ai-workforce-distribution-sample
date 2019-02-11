using Google.OrTools.ConstraintSolver;
using System;
using System.Collections.Generic;
using System.Text;

namespace ORToolsRoutingAndScheduling
{
    class TimeCallback : LongLongToLong
    {
        private long[,] timeMatrix;
        private long[] timeDemand_;
        private RoutingIndexManager indexManager;
        public TimeCallback(DataModel data, RoutingIndexManager manager)
        {
            timeMatrix = data.GetTimeMatrix();
            timeDemand_ = data.GetTimeDemandMatrix();
            indexManager = manager;
        }

        override public long Run(long fromIndex, long toIndex)
        {
            // Convert from routing variable Index to time matrix NodeIndex.
            int fromNode = indexManager.IndexToNode(fromIndex);
            int toNode = indexManager.IndexToNode(toIndex);
            return timeMatrix[fromNode, toNode]
                 + timeDemand_[fromNode];
        }
    }
}
