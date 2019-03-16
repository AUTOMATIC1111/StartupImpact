using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace StartupImpact
{
    public class ModConstructor
    {
        static List<ModContentPack> runningMods = Traverse.Create(typeof(LoadedModManager)).Field<List<ModContentPack>>("runningMods").Value;
        static Dictionary<Type, Mod> runningModClasses = Traverse.Create(typeof(LoadedModManager)).Field<Dictionary<Type, Mod>>("runningModClasses").Value;

        public static ModContentPack FindMod(Assembly assembly) {
            return ( from modpack in runningMods where modpack.assemblies.loadedAssemblies.Contains(assembly) select modpack ).FirstOrDefault();
        }

        public static void Create()
        {
 
            using (IEnumerator<Type> enumerator = typeof(Mod).InstantiableDescendantsAndSelf().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Type type = enumerator.Current;
                    try
                    {
                        if (type == typeof(StartupImpact)) continue;

                        if (!runningModClasses.ContainsKey(type))
                        {
                            ModContentPack modContentPack = FindMod(type.Assembly);

                            var info = StartupImpact.modlist.Get(modContentPack);
                            info.Start("constructor");
                            runningModClasses[type] = (Mod)Activator.CreateInstance(type, new object[] { modContentPack });
                            info.Stop("constructor");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(string.Concat(new object[] { "Error while instantiating a mod of type ", type, ": ", ex }), false);
                    }
                }
            }
        }
    }
}
