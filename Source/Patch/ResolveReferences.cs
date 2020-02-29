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

        static void Prefix(Def __instance)
        {
            ModInfo info = StartupImpact.modlist.Get(__instance.modContentPack);
            if (info!=null)
            {
                info.Start("resolve-references");
            }
        }
        static void Postfix(Def __instance)
        {
            ModInfo info = StartupImpact.modlist.Get(__instance.modContentPack);
            if (info != null)
            {
                totalResolving += info.Stop("resolve-references");
            }
        }
    }
}
