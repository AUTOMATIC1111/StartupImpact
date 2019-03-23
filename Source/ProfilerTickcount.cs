using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StartupImpact
{
    class ProfilerTickCount
    {
        Profiler profiler;
        int startTime = -1;
        string category;

        internal ProfilerTickCount(Profiler profiler)
        {
            this.profiler = profiler;
        }

        public void Start(string cat)
        {
            if (startTime != -1)
            {
                return;
            }

            startTime = Environment.TickCount;
            category = cat;
        }


        public int Stop(string cat)
        {
            if (startTime == -1)
            {
                return 0;
            }
            if (cat != category)
            {
                return 0;
            }

            int ms = Environment.TickCount - startTime;
            startTime = -1;
            return ms;
        }
    }

}
