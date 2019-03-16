using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace StartupImpact
{
    class UiTable
    {
        Window window;
        float rowCount;
        float rowHeight;
        float[] columnWidthsTemplate;
        float[] columnOffsets;
        float[] columnWidths;
        Rect uiRect;
        Rect viewRect;
        Rect userRect;
        
        public UiTable(Window window, float rowCount, float rowHeight, float[] columnWidthsTemplate) {
            this.window = window;
            this.rowCount = rowCount;
            this.rowHeight = rowHeight;
            this.columnWidthsTemplate = columnWidthsTemplate;

            columnOffsets = new float[columnWidthsTemplate.Length];
            columnWidths = new float[columnWidthsTemplate.Length];
        }

        public void Start(float x, float y, float w, float h) {
 
            if (uiRect.x != x || uiRect.y != y || uiRect.width != w || uiRect.height != h) {
                uiRect = new Rect(x, y, w, h);
                viewRect = new Rect(0, 0, w - 16f, rowCount * rowHeight);

                float totalNeededWidth = 0;
                float totalAvailableWidth = viewRect.width;
                foreach (float cw in columnWidthsTemplate)
                {
                    if (cw > 0)
                    {
                        totalNeededWidth += cw;
                    }
                    else
                    {
                        totalAvailableWidth += cw;
                    }
                }

                float xoff = 0;
                int n = 0;
                foreach (float cw in columnWidthsTemplate)
                {
                    float calculatedWidth;
                    if (cw > 0)
                    {
                        calculatedWidth = cw * totalAvailableWidth / totalNeededWidth;
                    }
                    else
                    {
                        calculatedWidth = -cw;
                    }

                    columnOffsets[n] = xoff;
                    columnWidths[n] = calculatedWidth;

                    xoff += calculatedWidth;
                    n++;
                }
               
            }

            Widgets.BeginScrollView(uiRect, ref scrollPosition, viewRect, true);
        }

        public Rect cell(int x, int y) {
            if (x < 0 || y < 0 || x >= columnWidths.Length || y >= rowCount)
                throw new Exception("Bad cell coordinates: (" + x + "," + y + ")");
            /*
            if (x == 0 && y % 2 == 0) {
                userRect.x = 0;
                userRect.y = y * rowHeight;
                userRect.width = viewRect.width;
                userRect.height = rowHeight;
                Widgets.DrawAltRect(userRect);
            }*/

            userRect.x = columnOffsets[x];
            userRect.y = y * rowHeight;
            userRect.width = columnWidths[x];
            userRect.height = rowHeight;

//            if ((x+ y) % 2 == 0) {
//                Widgets.DrawAltRect(userRect);
//            }

            return userRect;
        }

        public void text(int x, int y, string text) {
            Rect rect = cell(x, y);
            Widgets.Label(rect, text.Truncate(rect.width, null));
        }

        public void Stop()
        {
            Widgets.EndScrollView();
        }



        protected Vector2 scrollPosition = Vector2.zero;
    }
}
