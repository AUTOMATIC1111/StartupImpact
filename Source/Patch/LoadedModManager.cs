using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace StartupImpact.Patch
{
    [HarmonyPatch(typeof(LoadedModManager), "LoadModXML")]
    class LoadedModManagerLoadModXML
    {
        static void Prefix() {
            StartupImpact.modlist.Clear();
            ModContentPackLoadPatches.patchMods.Clear();
        }
    }
}
