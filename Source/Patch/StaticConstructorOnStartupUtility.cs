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
            IEnumerable<Type> enumerable = GenTypes.AllTypesWithAttribute<StaticConstructorOnStartup>();
            foreach (Type type in enumerable)
            {
                ModInfo info = StartupImpact.modlist.Get(ModConstructor.FindMod(type.Assembly));
                if (info != null) info.Start("constructor");
                RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                if (info != null) info.Stop("constructor");
            }
            StaticConstructorOnStartupUtility.coreStaticAssetsLoaded = true;

            StartupImpact.loadingTime = Environment.TickCount - StartupImpact.loadingTime;

            return false;
        }
    }
}
