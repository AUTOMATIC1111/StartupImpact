using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace StartupImpact
{
    public class ModInfoList
    {
        private List<ModInfo> mods = new List<ModInfo>();
        private Dictionary<ModContentPack, ModInfo> reference = new Dictionary<ModContentPack, ModInfo>();
        private ModContentPack coreMod = null;

        public void Clear() {
//            Log.Message("clearing modlist");
//            mods.Clear();
//            reference.Clear();
//            coreMod = null;
        }

        public ModInfo Get(ModContentPack mod)
        {
            if (mod != null && coreMod == null && mod.IsCoreMod)
            {
                coreMod = mod;
            }

            if (mod == null)
            {
                mod = coreMod;
            }

            if (mod == null) return null;

            ModInfo res;
            if(reference.TryGetValue(mod, out res)) return res;

            res = new ModInfo( mod );
            mods.Add(res);
            reference.Add(mod, res);
            return res;
        }

        public List<ModInfo> GetMods()
        {
            //            return mods.OrderBy(x => -x.profile.totalImpact).Where(x => !x.mod.IsCoreMod).ToList();
            return mods.OrderBy(x => -x.profile.totalImpact).ToList();
        }
        public ModInfo GetCore()
        {
            return null;
        }
    }
}
