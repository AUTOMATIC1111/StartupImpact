using Harmony;
using StartupImpact.Patch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using UnityEngine;
using Verse;

namespace StartupImpact
{
    public class StartupImpact :Mod
    {
        public static ModInfoList modlist = new ModInfoList();
        public static int loadingTime;

        public StartupImpact(ModContentPack pack) :base(pack) {
            loadingTime = Environment.TickCount;

            var harmony = HarmonyInstance.Create("com.github.automatic1111.startupimpact");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            ModConstructor.Create();
        }
    }
}
