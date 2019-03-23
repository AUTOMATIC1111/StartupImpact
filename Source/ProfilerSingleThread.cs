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

        string category;

        string What {
            get {
                if (what == null) return "";
                return what + " profiler";
            }
        }

        public void Start(string cat)
        {
            if (cat == null) return;

            if (category != null)
            {
                Log.Error("Starting " + What + " for [" + cat + "] while it's already active timing [" + category + "]");
                stop();
            }

            category = cat;
            start();
        }

        public int Stop(string cat)
        {
            if (category == null)
            {
                Log.Error("Stopping " + What + " for [" + cat + "] while it's already inactive");
                return 0;
            }
            if (cat != category)
            {
                Log.Error("Stopping " + What + " for [" + cat + "] while it's timing [" + category + "]");
            }

            int ms = stop();
            total += ms;
            category = null;
            return ms;
        }
    }
}
