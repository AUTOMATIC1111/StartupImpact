using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Verse;

namespace StartupImpact
{

    public class Profiler
    {
        string what;
        public readonly ProfilerType profilerType;

        public Profiler(string w)
        {
            profilerType = StartupImpact.settings.profilerType;
            what = w;
        }

        public Dictionary<string, int> metrics = new Dictionary<string, int>();
        public int totalImpact = 0;

        public ProfilerSingleThread CreateProfiler()
        {
            switch (profilerType)
            {
                case ProfilerType.Date:
                    return new ProfilerDate() { what = what };
                case ProfilerType.Stopwatch:
                    return new ProfilerStopwatch() { what = what };
                default:
                case ProfilerType.Ticks:
                    return new ProfilerTickCount() { what = what };
            }
        }

        ProfilerSingleThread[] threadProfilers = new ProfilerSingleThread[20]; //yolo

        ProfilerSingleThread threadProfiler()
        {
            int id = Thread.CurrentThread.ManagedThreadId;
            if (id < 0 || id > threadProfilers.Length)
                id = threadProfilers.Length - 1;

            if (threadProfilers[id] == null)
                threadProfilers[id] = CreateProfiler();
                
            return threadProfilers[id];
        }

        public void Start(string cat)
        {
            threadProfiler().Start(cat);
        }

        public int Stop(string cat)
        {
            int ms = threadProfiler().Stop(cat);
            totalImpact += ms;

            int total = 0;
            metrics.TryGetValue(cat, out total);
            total += ms;
            metrics[cat] = total;

            return ms;
        }

        public int Impact(string v)
        {
            int res = 0;
            metrics.TryGetValue(v, out res);
            return res;
        }


        class SingleThreadDateTime
        {
            Profiler profiler;
            DateTime startTime = DateTime.MinValue;
            string category;

            internal SingleThreadDateTime(Profiler profiler)
            {
                this.profiler = profiler;
            }

            public void Start(string cat)
            {
                if (startTime != DateTime.MinValue)
                {
                    return;
                }

                startTime = DateTime.Now;
                category = cat;
            }


            public int Stop(string cat)
            {
                if (startTime == DateTime.MinValue)
                {
                    return 0;
                }
                if (cat != category)
                {
                    return 0;
                }

                int ms = (DateTime.Now - startTime).Milliseconds;
                startTime = DateTime.MinValue;
                return ms;
            }
        }
    }
}
