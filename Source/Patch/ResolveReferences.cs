using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace StartupImpact.Patch
{
    public class ResolveReferences
    {
        public static int totalResolving = 0;
        static Type currentlyProfiling = null;

        static void Prefix(Def __instance)
        {
            if (currentlyProfiling!=null) return;

            ModInfo info = StartupImpact.modlist.Get(__instance.modContentPack);
            if (info!=null)
            {
                info.Start("resolve-references");
                currentlyProfiling = __instance.GetType();
            }
        }
        static void Postfix(Def __instance)
        {
            if (currentlyProfiling != __instance.GetType()) return;

            ModInfo info = StartupImpact.modlist.Get(__instance.modContentPack);
            if (info != null)
            {
                totalResolving += info.Stop("resolve-references");
                currentlyProfiling = null;
            }
        }
    }
}
