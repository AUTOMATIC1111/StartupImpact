using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace StartupImpact
{
    public class ProfilerBar
    {
        public static string TimeText(int ms)
        {
            if (ms > 10000) return "StartupImpactSeconds".Translate((ms * 0.001f).ToString("F1"));
            if (ms > 1000) return "StartupImpactSeconds".Translate((ms * 0.001f).ToString("F2"));
            if (ms == 0) return "0";
            return "StartupImpactMilliseconds".Translate(ms);
        }
        
        public static void Draw(
            Rect rect,
            Dictionary<string, int> metrics,
            List<string> categories,
            int maxImpact,
            Dictionary<string, string> categoryHints,
            Dictionary<string, Color> categoryColors,
            Color defaultColor,
            float progressBarPadding = 4
        )
        {
            float progressBarWidth = rect.width - progressBarPadding * 2;
            float progressBarX = 0;
            float x = rect.x + progressBarPadding;

            foreach (string cat in categories)
            {
                int impact = 0;
                metrics.TryGetValue(cat, out impact);
                if (impact == 0) continue;

                Color color;
                if(! categoryColors.TryGetValue(cat, out color)) color = defaultColor;

                float width = progressBarWidth * impact / maxImpact;
                Rect textRect = new Rect(x + progressBarX, rect.y + progressBarPadding, width, rect.height - progressBarPadding * 2);


                Color stored = GUI.color;

                if (Mouse.IsOver(textRect))
                {
                    GUI.color = Color.Lerp(color * stored, Color.white, 0.25f);
                    if (textRect.width > 6)
                    {
                        GUI.DrawTexture(textRect, BaseContent.WhiteTex);
                        GUI.color = color * stored;
                        GUI.DrawTexture(GenUI.ContractedBy(textRect, 3f), BaseContent.WhiteTex);
                    }
                    else
                    {
                        GUI.DrawTexture(textRect, BaseContent.WhiteTex);
                    }
                }
                else
                {
                    GUI.color = color * stored;
                    GUI.DrawTexture(textRect, BaseContent.WhiteTex);
                }
                GUI.color = stored;

                progressBarX += width;

                TooltipHandler.TipRegion(textRect, () => {
                    string res;
                    if(! categoryHints.TryGetValue(cat, out res)) res = cat;
                    res += ": " + TimeText(impact);
                    return res;
                }, 1163428609);
            }
        }

    }
}
