using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StartupImpact
{
    class ProfilerDate : ProfilerSingleThread
    {
        DateTime startTime;

        public override void start()
        {
            startTime = DateTime.UtcNow;
        }

        public override int stop()
        {
            return (DateTime.UtcNow - startTime).Milliseconds;
        }
    }
}
