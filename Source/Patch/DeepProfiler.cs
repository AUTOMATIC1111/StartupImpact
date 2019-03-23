using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Verse;

namespace StartupImpact.Patch
{
    [HarmonyPatch(typeof(DeepProfiler), "Start")]
    public class DeepProfilerStart
    {
        public static string storedLabel;
        public static bool mute = false;

        public static void Prefix(string label) {
            if (mute) return;

            if (storedLabel == null) {
                StartupImpact.baseGameProfiler.Start(storedLabel = label);
            }
        }
    }

    [HarmonyPatch(typeof(DeepProfiler), "End")]
    public class DeepProfilerEnd
    {
        public static void Prefix()
        {
            if (DeepProfilerStart.storedLabel != null)
            {
                StartupImpact.baseGameProfiler.Stop(DeepProfilerStart.storedLabel);
            }

            DeepProfilerStart.storedLabel = null;
        }
    }
}
