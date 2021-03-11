using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASCII_FPS.UI
{
    public static class UIUtils
    {
        public static readonly byte colorRed = Mathg.ColorTo8Bit(Color.Red.ToVector3());
        public static readonly byte colorBlack = Mathg.ColorTo8Bit(Color.Black.ToVector3());
        public static readonly byte colorDarkGray = Mathg.ColorTo8Bit(Color.DimGray.ToVector3());
        public static readonly byte colorGray = Mathg.ColorTo8Bit(Color.DarkGray.ToVector3());
        public static readonly byte colorLightGray = Mathg.ColorTo8Bit(Color.LightGray.ToVector3());
        public static readonly byte colorWhite = Mathg.ColorTo8Bit(Color.White.ToVector3());
        public static readonly byte colorForestGreen = Mathg.ColorTo8Bit(Color.ForestGreen.ToVector3());
        public static readonly byte colorLightBlue = Mathg.ColorTo8Bit(Color.LightBlue.ToVector3());


        public static void Text(Console console, int x, int y, string text, byte color, UIAlignment alignment = UIAlignment.Center)
        {
            int start = x;
            switch (alignment)
            {
                case UIAlignment.Center:
                    start -= text.Length / 2;
                    break;
                case UIAlignment.Right:
                    start -= text.Length - 1;
                    break;
            }

            for (int xx = start; xx < start + text.Length; xx++)
            {
                if (xx >= 0 && xx < console.Width)
                {
                    console.Data[xx, y] = text[xx - start];
                    console.Color[xx, y] = color;
                }
            }
        }

        public static void Text(Console console, UIPosition position, string text, byte color, UIAlignment alignment = UIAlignment.Center)
        {
            int x = position.GetX(console);
            int y = position.GetY(console);
            Text(console, x, y, text, color, alignment);
        }
    }
}
