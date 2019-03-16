using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace StartupImpact.Patch
{

    [HarmonyPatch(typeof(OptionListingUtility), "DrawOptionListing"), StaticConstructorOnStartup]
    class OptionListingUtilityDrawOptionListing
    {
        static void Prefix(List<ListableOption> optList)
        {
            if (optList.Count == 0) return;
            if (optList[0].GetType() != typeof(ListableOption_WebLink)) return;

            optList.Add(new ListableOption_WebLink("StartupImpact".Translate(), delegate () {
                Find.WindowStack.Add(new DialogStartupImpact());
            }, icon));
        }

        public static readonly Texture2D icon = ContentFinder<Texture2D>.Get("StartupImpact/icon", true);
    }
}
