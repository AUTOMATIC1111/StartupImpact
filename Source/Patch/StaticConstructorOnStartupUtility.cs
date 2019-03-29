using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Verse;

namespace StartupImpact.Patch
{
    [HarmonyPatch(typeof(StaticConstructorOnStartupUtility), "CallAll")]
    class StaticConstructorOnStartupUtilityCallAll
    {
        static public bool Prefix() {
            DeepProfilerStart.mute = true;

            IEnumerable<Type> enumerable = GenTypes.AllTypesWithAttribute<StaticConstructorOnStartup>();
            foreach (Type type in enumerable)
            {
                ModInfo info = StartupImpact.modlist.Get(ModConstructor.FindMod(type.Assembly));
                if (info != null) info.Start("constructor");
                RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                if (info != null) info.Stop("constructor");
            }
            StaticConstructorOnStartupUtility.coreStaticAssetsLoaded = true;

            if (!StartupImpact.loadingTimeMeasured)
            {
                StartupImpact.loadingTimeMeasured = true;
                StartupImpact.loadingProfiler.Stop("loading");
                StartupImpact.loadingTime = StartupImpact.loadingProfiler.total;
            }

            int spentResolving;
            if (StartupImpact.baseGameProfiler.metrics.TryGetValue("Resolve references.", out spentResolving))
            {
                spentResolving -= ResolveReferences.totalResolving;
                if (spentResolving < 0) spentResolving = 0;
                StartupImpact.baseGameProfiler.metrics["Resolve references."] = spentResolving;
            }

            return false;
        }
    }
}
