using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace StartupImpact.Patch
{
    [HarmonyPatch(typeof(ModContentPack), "LoadDefs")]
    class ModContentPackLoadDefs
    {
        static IEnumerable<LoadableXmlAsset> Postfix(IEnumerable<LoadableXmlAsset> __result, ModContentPack __instance)
        {
            var info = StartupImpact.modlist.Get(__instance);
            info.Start("defs-0-load");

            foreach (var v in __result)
            {
                yield return v;
            }

            info.Stop("defs-0-load");

            yield break;
        }
    }

    [HarmonyPatch(typeof(ModContentPack), "LoadPatches")]
    class ModContentPackLoadPatches
    {
        public static Dictionary<string, ModInfo> patchMods = new Dictionary<string, ModInfo>();

        static void Prefix(ModContentPack __instance)
        {
            StartupImpact.modlist.Get(__instance).Start("load-patches");
        }
        static void Postfix(ModContentPack __instance)
        {
            ModInfo info = StartupImpact.modlist.Get(__instance);
            info.Stop("load-patches");

            foreach(PatchOperation patch in __instance.Patches) {
                patchMods[patch.sourceFile] = info;
            }
        }
    }

    [HarmonyPatch(typeof(ModContentPack), "<ReloadContent>m__1")]
    class ModContentPackReloadContentm__1
    {
        static bool Prefix(ModContentPack __instance)
        {
            DeepProfilerStart.mute = true;

            var info = StartupImpact.modlist.Get(__instance);

            info.Start("audioclips");
            Traverse.Create(__instance).Field<ModContentHolder<AudioClip>>("audioClips").Value.ReloadAll();
            info.Stop("audioclips");

            info.Start("textures");
            Traverse.Create(__instance).Field<ModContentHolder<Texture2D>>("textures").Value.ReloadAll();
            info.Stop("textures");

            info.Start("strings");
            Traverse.Create(__instance).Field<ModContentHolder<string>>("strings").Value.ReloadAll();
            info.Stop("strings");

            DeepProfilerStart.mute = false;

            return false;
        }

    }

    
}
