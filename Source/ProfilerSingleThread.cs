using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace StartupImpact
{
    public abstract class ProfilerSingleThread
    {
        public string what;
        public int total = 0;

        public abstract void start();
        public abstract int stop();
        public virtual int stopAndStart() {
            int res = stop();
            start();
            return res;
        }

        List<string> categories = new List<string>();

        string What {
            get {
                if (what == null) return "";
                return what + " profiler";
            }
        }

        public void Start(string cat)
        {
            if (cat == null) return;

            if (categories.Count > 0)
            {
                int ms = stopAndStart();
                total += ms;
            }
            else
            {
                start();
            }

            categories.Insert(0, cat);
        }

        public int Stop(string cat)
        {
            string outCat;

            return Stop(cat, out outCat);
        }

        public int Stop(string cat, out string outCat)
        {
            if (categories.Count == 0)
            {
                if (cat != null) Log.Error("Stopping " + What + " for [" + cat + "] while it's already inactive");
                outCat = "none";
                return 0;
            }

            if (cat != null && cat != categories[0])
            {
                Log.Error("Stopping " + What + " for [" + cat + "] while it's timing [" + categories[0] + "]");
            }

            outCat = categories[0];
            categories.RemoveAt(0);

            int ms;
            if (categories.Count > 0)
            {
                ms = stopAndStart();
            }
            else
            {
                ms = stop();
            }

            total += ms;
            return ms;
        }
    }
}
