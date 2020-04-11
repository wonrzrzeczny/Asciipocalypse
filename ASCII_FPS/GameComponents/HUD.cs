using Microsoft.Xna.Framework;
using System;

namespace ASCII_FPS.GameComponents
{
    public class HUD
    {
        private readonly Console console;

        private readonly byte colorRed = Mathg.ColorTo8Bit(Color.Red.ToVector3());
        private readonly byte colorBlack = Mathg.ColorTo8Bit(Color.Black.ToVector3());
        private readonly byte colorGray = Mathg.ColorTo8Bit(Color.DarkGray.ToVector3());
        private readonly byte colorWhite = Mathg.ColorTo8Bit(Color.White.ToVector3());
        private readonly byte colorForestGreen = Mathg.ColorTo8Bit(Color.ForestGreen.ToVector3());

        public HUD(Console console)
        {
            this.console = console;
        }



        private void LineHorizontal(int y, int left, int right, byte color, char data)
        {
            if (left < 0) left += console.Width;
            if (right < 0) right += console.Width;
            if (y < 0) y += console.Height;

            for (int i = left; i <= right; i++)
            {
                console.Data[i, y] = data;
                console.Color[i, y] = color;
            }
        }

        private void LineVertical(int x, int top, int bottom, byte color, char data)
        {
            if (top < 0) top += console.Height;
            if (bottom < 0) bottom += console.Height;
            if (x < 0) x += console.Width;

            for (int i = top; i <= bottom; i++)
            {
                console.Data[x, i] = data;
                console.Color[x, i] = color;
            }
        }

        private void Border(int left, int top, int right, int bottom, byte color, char data)
        {
            LineHorizontal(top, left, right, color, data);
            LineHorizontal(bottom, left, right, color, data);
            LineVertical(left, top, bottom, color, data);
            LineVertical(right, top, bottom, color, data);
        }

        private void Rectangle(int left, int top, int right, int bottom, byte color, char data)
        {
            if (left < 0) left += console.Width;
            if (right < 0) right += console.Width;
            if (top < 0) top += console.Height;
            if (bottom < 0) bottom += console.Height;

            for (int x = left; x <= right; x++)
            {
                for (int y = top; y <= bottom; y++)
                {
                    console.Data[x, y] = data;
                    console.Color[x, y] = color;
                }
            }
        }

        private void Text(int x, int y, string text, byte color)
        {
            if (x < 0) x += console.Width;
            if (y < 0) y += console.Height;

            int start = x - text.Length / 2;
            for (int xx = start; xx < start + text.Length; xx++)
            {
                if (xx >= 0 && xx < console.Width)
                {
                    console.Data[xx, y] = text[xx - start];
                    console.Color[xx, y] = color;
                }
            }
        }


        public void Draw()
        {
            // HP bar
            Rectangle(0, -22, 21, -1, colorBlack, ' ');
            Border(0, -22, 21, -1, colorGray, '@');
            Text(11, -20, "Health", colorWhite);
            Text(11, -18, (int)Math.Ceiling(ASCII_FPS.playerStats.health) 
                + " / " + (int)Math.Ceiling(ASCII_FPS.playerStats.maxHealth), colorWhite);
            LineHorizontal(-16, 0, 21, colorGray, '@');
            int hpDots = (int)(20 * 14 * ASCII_FPS.playerStats.health / ASCII_FPS.playerStats.maxHealth);
            if (hpDots >= 20) Rectangle(1, -1 - hpDots / 20, 20, -2, colorRed, '%');
            if (hpDots % 20 > 0) Rectangle(1, -2 - hpDots / 20, 1 + hpDots % 20, -2 - hpDots / 20, colorRed, '%');

            // Armor bar
            Rectangle(-22, -22, -1, -1, colorBlack, ' ');
            Border(-22, -22, -1, -1, colorGray, '@');
            Text(-11, -20, "Armor", colorWhite);
            Text(-11, -18, (int)Math.Ceiling(ASCII_FPS.playerStats.armor)
                + " / " + (int)Math.Ceiling(ASCII_FPS.playerStats.maxArmor), colorWhite);
            LineHorizontal(-16, -22, -1, colorGray, '@');
            int armorDots = (int)(20 * 14 * ASCII_FPS.playerStats.armor / ASCII_FPS.playerStats.maxArmor);
            if (armorDots >= 20) Rectangle(-21, -1 - armorDots / 20, -2, -2, colorForestGreen, '#');
            if (armorDots % 20 > 0) Rectangle(-(1 + armorDots % 20), -2 - armorDots / 20, -2, -2 - armorDots / 20, colorForestGreen, '#');
        }
    }
}
