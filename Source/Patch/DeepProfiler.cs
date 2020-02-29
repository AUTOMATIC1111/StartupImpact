using HarmonyLib;
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
        public static bool mute = false;

        public static void Prefix(string label) {
            if (mute) return;

            StartupImpact.baseGameProfiler.Start(label);
        }
    }

    [HarmonyPatch(typeof(DeepProfiler), "End")]
    public class DeepProfilerEnd
    {
        public static void Prefix()
        {
            if (DeepProfilerStart.mute) return;

            StartupImpact.baseGameProfiler.Stop(null);
        }
    }
}
