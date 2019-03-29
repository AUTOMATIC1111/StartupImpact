using StartupImpact.Patch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;

namespace StartupImpact
{
    [StaticConstructorOnStartup]
    class DialogStartupImpact : Window
    {
        public override Vector2 InitialSize => new Vector2(800f, Math.Min(800,UI.screenHeight*0.75f));

        public static readonly Dictionary<string, Color> categoryColors = new Dictionary<string, Color>
        {
            { "textures", new Color(156f/255, 147f/255, 67f/255) },
            { "audioclips", new Color(67f/255, 84f/255, 156f/255)},
            { "strings", new Color(130f/255, 130f/255, 130f/255)},
            { "defs-0-load", new Color(84f/255, 207f/255, 154f/255)},
            { "defs-1-create", new Color(72f/255, 121f/255, 175f/255)},
            { "load-patches", new Color(136f/255, 156f/255, 67f/255)},
            { "patch", new Color(156f/255, 67f/255, 121f/255)},
            { "constructor", new Color(176f/255, 223f/255, 224f/255)},
            { "resolve-references", new Color(82f/255, 26f/255, 106f/255)},

            { "total-mods", new Color(175f/255, 126f/255, 72f/255)},
            { "total-mods-hidden", new Color(103f/255, 83f/255, 61f/255)},
            { "total-basegame", new Color(72f/255, 121f/255, 175f/255)},
            { "total-others", new Color(35f/255, 50f/255, 84f/255)},
            
        };
        public static readonly Color defaultColor = new Color(120f / 255, 205f / 255, 168f / 255);
        public static readonly Color infoColor = new Color(128 / 255f, 128 / 255f, 128 / 255f);

        public Dictionary<string, string> categoryHints = new Dictionary<string, string>();

        List<ModInfo> mods;
        int maxImpact;

        List<string> categories;
        List<string> categoriesTotal = new List<string> { "total-mods", "total-mods-hidden", "total-basegame", "total-others" };
        List<string> categoriesNonmods;
        Dictionary<string, Color> categoryColorsNonmods = new Dictionary<string, Color>();
        Dictionary<string, string> categoryHintsNonmods = new Dictionary<string, string>();

        Dictionary<string, int> metricsTotal;
        int basegameLoadingTime;
        int modsLoadingTime;
        int hiddenModsLoadingTime;
        bool failedMeasuringLoadingTime = false;
        UiTable table;

        public DialogStartupImpact() {
            foreach (var entry in categoryColors) {
                categoryHints[entry.Key] = ("StartupImpact-" + entry.Key).Translate();
            }

            mods = StartupImpact.modlist.GetMods();

            redoBaseGameStats();
            redoStats();

            table = new UiTable(this, mods.Count, 40, new float[] { -40, 30, -80, 38 });
            
        }

        void redoStats() {
            modsLoadingTime = 0;
            hiddenModsLoadingTime = 0;
            maxImpact = 0;

            HashSet<string> catSet = new HashSet<string>();
            foreach (ModInfo info in mods)
            {
                if (info.hideInUi)
                {
                    hiddenModsLoadingTime += info.profile.totalImpact;
                }
                else
                {
                    if (maxImpact < info.profile.totalImpact)
                        maxImpact = info.profile.totalImpact;
                    modsLoadingTime += info.profile.totalImpact;
                }

                foreach (KeyValuePair<string, int> entry in info.profile.metrics)
                {
                    catSet.Add(entry.Key);
                }
            }

            categories = catSet.OrderBy(x => x).ToList();

            if (StartupImpact.loadingTime == 0) {
                StartupImpact.loadingTime = modsLoadingTime + hiddenModsLoadingTime + basegameLoadingTime;
                failedMeasuringLoadingTime = true;
            }

            metricsTotal = new Dictionary<string, int> {
                { "total-mods", modsLoadingTime },
                { "total-mods-hidden", hiddenModsLoadingTime },
                { "total-basegame", basegameLoadingTime },
                { "total-others",  StartupImpact.loadingTime - modsLoadingTime - hiddenModsLoadingTime - basegameLoadingTime },
            };
        }

        public void redoBaseGameStats() {
            categoriesNonmods = new List<string>();
            basegameLoadingTime = 0;

            foreach (var entry in StartupImpact.baseGameProfiler.metrics){
                string cat = entry.Key;
                string title = cat;
                if (title.EndsWith(".")) title = title.Substring(0, title.Length - 1);
                int hash = cat.GetHashCode();

                categoryHintsNonmods[cat] = title;
                categoryColorsNonmods[cat] = new Color((hash & 0xff) / 255f, ((hash>>8) & 0xff) / 255f, ((hash >> 16) & 0xff)/255f);
                categoriesNonmods.Add(cat);
                basegameLoadingTime += entry.Value;
            }
        }

        public override void DoWindowContents(Rect area)
        {
            Text.Anchor = TextAnchor.MiddleLeft;
            float y = 0;

            Text.Font = GameFont.Medium;

            Rect titleRect = new Rect(0, y, area.width, 30);
            Widgets.Label(titleRect, "StartupImpactStartupTime".Translate(ProfilerBar.TimeText(StartupImpact.loadingTime)));
            y += titleRect.height;

            Rect profileRect = new Rect(0, y, area.width - 16, 46);
            ProfilerBar.Draw(profileRect, metricsTotal, categoriesTotal, StartupImpact.loadingTime, categoryHints, categoryColors, defaultColor);
            y += profileRect.height + 4;
            
            Rect nonmodsTitleRect = new Rect(0, y, area.width, 30);
            Widgets.Label(nonmodsTitleRect, "StartupImpactStartupNonmods".Translate(ProfilerBar.TimeText(basegameLoadingTime)));
            y += nonmodsTitleRect.height;

            Rect nonmodsProfileRect = new Rect(0, y, area.width - 16, 46);
            ProfilerBar.Draw(nonmodsProfileRect, StartupImpact.baseGameProfiler.metrics, categoriesNonmods, basegameLoadingTime, categoryHintsNonmods, categoryColorsNonmods, defaultColor);
            y += nonmodsProfileRect.height + 8;

            Rect modsTitleRect = new Rect(0, y, area.width, 30);
            Widgets.Label(modsTitleRect, "StartupImpactStartupMods".Translate(ProfilerBar.TimeText(modsLoadingTime)));
            y += titleRect.height + 8;
            Text.Font = GameFont.Small;

            table.Start(0, y, area.width, area.height - y - 55);

            int row = 0;
            foreach (ModInfo info in mods)
            {
                if (Widgets.ButtonImage(table.cell(0, row), eye, info.hideInUi ? Color.white : Color.grey))
                {
                    info.hideInUi = !info.hideInUi;
                    redoStats();
                }

                GUI.color = info.hideInUi ? Color.grey : Color.white;

                table.text(1, row, info.mod.Name);

                table.text(2, row, ProfilerBar.TimeText(info.profile.totalImpact));

                ProfilerBar.Draw(table.cell(3, row), info.profile.metrics, categories, Math.Max(maxImpact, info.profile.totalImpact), categoryHints, categoryColors, defaultColor);

                row++;
            }

            table.Stop();

            
            GUI.color = infoColor;
            Text.Anchor = TextAnchor.LowerLeft;
            Rect rect4 = new Rect(0, area.height - 35f, area.width- 120, 35f);
            string verinfo = "StartupImpactVerinfo".Translate(StartupImpact.GetVersion(), StartupImpact.baseGameProfiler.profilerType.ToString().ToLowerInvariant());
            if (failedMeasuringLoadingTime) verinfo += "StartupImpactFailedMeasuringLoadingTime".Translate();
            Widgets.Label(rect4, verinfo);
            
            GUI.color = Color.white;
            Rect rect3 = new Rect(area.width - 120, area.height - 35f, 120, 35f);
            if (Widgets.ButtonText(rect3, "Close".Translate(), true, false, true))
            {
                Close();
            }
            Text.Anchor = TextAnchor.UpperLeft;
        }
        
        static readonly Texture2D eye = ContentFinder<Texture2D>.Get("StartupImpact/eye", true);
    }
}
