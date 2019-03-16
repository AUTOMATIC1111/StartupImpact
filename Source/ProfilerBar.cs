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
            Dictionary<string, Texture2D> categoryColors,
            Texture2D defaultColor,
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

                Texture2D tex = defaultColor;
                categoryColors.TryGetValue(cat, out tex);

                float width = progressBarWidth * impact / maxImpact;
                Rect textRect = new Rect(x + progressBarX, rect.y + progressBarPadding, width, rect.height - progressBarPadding * 2);
                GUI.DrawTexture(textRect, tex);
                progressBarX += width;

                TooltipHandler.TipRegion(textRect, () => {
                    string res = cat;
                    categoryHints.TryGetValue(cat, out res);
                    res += ": " + TimeText(impact);
                    return res;
                }, 1163428609);
            }
        }

    }
}
