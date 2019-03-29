using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Verse;

namespace StartupImpact
{
    public class ModInfo
    {
        public ModInfo(ModContentPack m) {
            mod = m;
            profile = new Profiler(mod.Name);
        }

        public ModContentPack mod;
        public Profiler profile;
        public bool hideInUi = false;
 
        public void Start(string cat) {
            profile.Start(cat);
        }

        public int Stop(string cat)
        {
            return profile.Stop(cat);
        }
    }
}
