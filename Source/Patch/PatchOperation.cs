using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace StartupImpact.Patch
{
    [HarmonyPatch(typeof(PatchOperation), "Apply")]
    class PatchOperationApply
    {
        static void Prefix(PatchOperation __instance)
        {
            if (__instance.sourceFile == null) return;
            DeepProfilerStart.mute = true;

            ModInfo info = null;
            if (ModContentPackLoadPatches.patchMods.TryGetValue(__instance.sourceFile, out info))
            {
                info.Start("patch");
            }
        }
        static void Postfix(PatchOperation __instance)
        {
            if (__instance.sourceFile == null) return;
            DeepProfilerStart.mute = false;

            ModInfo info = null;
            if (ModContentPackLoadPatches.patchMods.TryGetValue(__instance.sourceFile, out info))
            {
                info.Stop("patch");
            }
        }
    }
}
