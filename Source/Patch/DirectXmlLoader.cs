using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace StartupImpact.Patch
{
    [HarmonyPatch(typeof(DirectXmlLoader), "DefFromNode")]
    class DirectXmlLoaderDefFromNode
    {
        static void Prefix(LoadableXmlAsset loadingAsset)
        {
            DeepProfilerStart.mute = true;
            StartupImpact.modlist.Get(loadingAsset==null ? null : loadingAsset.mod).Start("defs-1-create");
        }
        static void Postfix(LoadableXmlAsset loadingAsset)
        {
            DeepProfilerStart.mute = false;
            StartupImpact.modlist.Get(loadingAsset == null ? null : loadingAsset.mod).Stop("defs-1-create");
        }

    }
}
