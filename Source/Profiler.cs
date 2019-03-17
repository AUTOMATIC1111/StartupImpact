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

        public Profiler(string w)
        {
            what = w;
        }

        public Dictionary<string, int> metrics = new Dictionary<string, int>();
        public int totalImpact = 0;

        SingleThreadProfiler[] threadProfilers = new SingleThreadProfiler[20]; //yolo
        
        SingleThreadProfiler threadProfiler() {
            int id = Thread.CurrentThread.ManagedThreadId;
            if (id < 0 || id > threadProfilers.Length)
                id = threadProfilers.Length - 1;


            if (threadProfilers[id] == null)
                threadProfilers[id] = new SingleThreadProfiler(this);

            return threadProfilers[id];
        }

        public void Start(string cat)
        {
            threadProfiler().Start(cat);
        }

        public void Stop(string cat)
        {
            int ms = threadProfiler().Stop(cat);
            totalImpact += ms;

            int total = 0;
            metrics.TryGetValue(cat, out total);
            total += ms;
            metrics[cat] = total;
        }

        public int Impact(string v)
        {
            int res = 0;
            metrics.TryGetValue(v, out res);
            return res;
        }

        class SingleThreadProfiler
        {
            Profiler profiler;
            int startTime = -1;
            string category;

            internal SingleThreadProfiler(Profiler profiler)
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
