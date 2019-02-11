using System;
using System.Collections.Generic;
using System.Text;

namespace ORToolsRoutingAndScheduling
{
    public class DataModel
    {
        private long[,] timeMatrix_;
        private long[,] timeWindows_;
        private long[] timeDemand_;
        private long[] demands_;

        // Constructor:
        public DataModel()
        {
            timeMatrix_ = new long[,] {
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 128, 123, 22, 26, 28, 24, 28, 28, 213, 27, 25, 28, 212, 210, 214},
                {0, 128, 0, 1211, 1210, 26, 23, 29, 25, 28, 24, 215, 214, 213, 29, 218, 29},
                {0, 123, 1211, 0, 121, 27, 210, 26, 210, 210, 214, 26, 27, 29, 214, 26, 216},
                {0, 22, 1210, 121, 0, 26, 29, 24, 28, 29, 213, 24, 26, 28, 212, 28, 214},
                {0, 26, 26, 27, 26, 0, 22, 23, 22, 22, 27, 29, 27, 27, 26, 212, 28},
                {0, 28, 23, 210, 29, 22, 0, 26, 22, 25, 24, 212, 210, 210, 26, 215, 25},
                {0, 24, 29, 26, 24, 23, 26, 0, 24, 24, 28, 25, 24, 23, 27, 28, 210},
                {0, 28, 25, 210, 28, 22, 22, 24, 0, 23, 24, 29, 28, 27, 23, 213, 26},
                {0, 28, 28, 210, 29, 22, 25, 24, 23, 0, 24, 26, 25, 24, 23, 29, 25},
                {0, 213, 24, 214, 213, 27, 24, 28, 24, 24, 0, 210, 29, 28, 24, 213, 24},
                {0, 27, 215, 26, 24, 29, 212, 25, 29, 26, 210, 0, 21, 23, 27, 23, 210},
                {0, 25, 214, 27, 26, 27, 210, 24, 28, 25, 29, 21, 0, 22, 26, 24, 28},
                {0, 28, 213, 29, 28, 27, 210, 23, 27, 24, 28, 23, 22, 0, 24, 25, 26},
                {0, 212, 29, 214, 212, 26, 26, 27, 23, 23, 24, 27, 26, 24, 0, 29, 22},
                {0, 210, 218, 26, 28, 212, 215, 28, 213, 29, 213, 23, 24, 25, 29, 0, 29},
              {0, 214, 29, 216, 214, 28, 25, 210, 26, 25, 24, 210, 28, 26, 22, 29, 0},
            };
            timeWindows_ = new long[,] {
                {0, 0},
                {100, 150},
                {100, 150},
                {5, 100},
                {5, 100},
                {200, 500},
                {5, 100},
                {0, 50},
                {5, 100},
                {220, 500},
                {10, 1500},
                {10, 1500},
                {400, 500},
                {5, 1000},
                {5, 1000},
                {10, 1500},
                {5, 1000},
            };
            demands_ = new long[] { 0, 2221, 3171, 6393, 2186, 7423, 796, 3138, 7278, 5621, 9252, 6191, 5432, 4236, 2356, 8288, 2178 };
            timeDemand_ = new long[] { //Task Size
                0,
                10,
                120,
                40,
                160,
                300,
                50,
                20,
                360,
                30,
                90,
                150,
                170,
                200,
                210,
                230,
                70,
            };
        }
        public ref readonly long[,] GetTimeMatrix() { return ref timeMatrix_; }
        public ref readonly long[] GetTimeDemandMatrix() { return ref timeDemand_; }
        public ref readonly long[,] GetTimeWindows() { return ref timeWindows_; }
        public ref readonly long[] GetDemands() { return ref demands_; }
        public int GetVehicleNumber() { return 5; }
        public int GetDepot() { return 0; }

        public long getAllowWaitingTime()
        {
            // To allow for any waiting time:
            return this.getMaximumWorkerCapacity();
        }
        public long getMaximumWorkerCapacity()
        {
            return 360;
        }
    }
}
