﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace StartupImpact
{
    [StaticConstructorOnStartup]
    class DialogStartupImpact : Window
    {
        public override Vector2 InitialSize => new Vector2(800f, Math.Min(800,UI.screenHeight*0.75f));

        public static readonly Dictionary<string, Texture2D> categoryColors = new Dictionary<string, Texture2D>
        {
            { "textures", SolidColorMaterials.NewSolidColorTexture(new Color(156f/255, 147f/255, 67f/255)) },
            { "audioclips", SolidColorMaterials.NewSolidColorTexture(new Color(117f/255, 67f/255, 156f/255))},
            { "strings", SolidColorMaterials.NewSolidColorTexture(new Color(130f/255, 130f/255, 130f/255))},
            { "defs-0-load", SolidColorMaterials.NewSolidColorTexture(new Color(84f/255, 207f/255, 154f/255))},
            { "defs-1-create", SolidColorMaterials.NewSolidColorTexture(new Color(72f/255, 121f/255, 175f/255))},
            { "load-patches", SolidColorMaterials.NewSolidColorTexture(new Color(136f/255, 156f/255, 67f/255))},
            { "patch", SolidColorMaterials.NewSolidColorTexture(new Color(156f/255, 67f/255, 121f/255))},
            { "constructor", SolidColorMaterials.NewSolidColorTexture(new Color(176f/255, 223f/255, 224f/255))},

            { "total-core", SolidColorMaterials.NewSolidColorTexture(new Color(72f/255, 121f/255, 175f/255))},
            { "total-mods", SolidColorMaterials.NewSolidColorTexture(new Color(175f/255, 126f/255, 72f/255))},
            { "total-mods-hidden", SolidColorMaterials.NewSolidColorTexture(new Color(103f/255, 83f/255, 61f/255))},
            { "total-others", SolidColorMaterials.NewSolidColorTexture(new Color(35f/255, 50f/255, 84f/255))},
            
        };
        public static readonly Texture2D defaultColor = SolidColorMaterials.NewSolidColorTexture(new Color(120f / 255, 205f / 255, 168f / 255));

        public Dictionary<string, string> categoryHints = new Dictionary<string, string>();

        List<ModInfo> mods;
        int maxImpact;

        List<string> categories;
        List<string> categoriesTotal = new List<string> { "total-core", "total-mods", "total-mods-hidden", "total-others" };
        Dictionary<string, int> metricsTotal;
        int totalLoadingTime;
        int modsLoadingTime;
        int hiddenModsLoadingTime;
        UiTable table;


        public DialogStartupImpact() {
            foreach (var entry in categoryColors) {
                categoryHints[entry.Key] = ("StartupImpact-" + entry.Key).Translate();
            }

            mods = StartupImpact.modlist.GetMods();

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


            ModInfo core = StartupImpact.modlist.GetCore();
            totalLoadingTime = StartupImpact.loadingTime;
            int coreLoadingTime = core == null ? 0 : core.profile.totalImpact;
            int otherLoadingTime = totalLoadingTime - coreLoadingTime - modsLoadingTime - hiddenModsLoadingTime;
            metricsTotal = new Dictionary<string, int> {
                { "total-core", coreLoadingTime },
                { "total-mods", modsLoadingTime },
                { "total-mods-hidden", hiddenModsLoadingTime },
                { "total-others", otherLoadingTime },
            };
        }

        public override void DoWindowContents(Rect area)
        {
            Text.Anchor = TextAnchor.MiddleLeft;
            float y = 0;
            
            Rect titleRect = new Rect(0, y, area.width, 30);
            Text.Font = GameFont.Medium;
            Widgets.Label(titleRect, "StartupImpactStartupTime".Translate(ProfilerBar.TimeText(totalLoadingTime)));
            y += titleRect.height;

            Rect profileRect = new Rect(0, y, area.width-16, 46);
            ProfilerBar.Draw(profileRect, metricsTotal, categoriesTotal, totalLoadingTime, categoryHints, categoryColors, defaultColor);
            y += profileRect.height + 4;

            Rect modsTitleRect = new Rect(0, y, area.width, 30);
            Text.Font = GameFont.Medium;
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

            Rect rect3 = new Rect(area.width - 120, area.height - 35f, 120, 35f);
            if (Widgets.ButtonText(rect3, "Close".Translate(), true, false, true))
            {
                Close();
            }

            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
        }
        
        static readonly Texture2D eye = ContentFinder<Texture2D>.Get("StartupImpact/eye", true);
    }
}