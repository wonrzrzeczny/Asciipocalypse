using Microsoft.Xna.Framework;

namespace ASCII_FPS.GameComponents
{
    public class HUD
    {
        private readonly Console console;

        private readonly byte colorRed = Mathg.ColorTo8Bit(Color.Red.ToVector3());
        private readonly byte colorBlack = Mathg.ColorTo8Bit(Color.Black.ToVector3());
        private readonly byte colorGray = Mathg.ColorTo8Bit(Color.DarkGray.ToVector3());
        private readonly byte colorWhite = Mathg.ColorTo8Bit(Color.White.ToVector3());

        public HUD(Console console)
        {
            this.console = console;
        }



        private void LineHorizontal(int y, int left, int right, byte color, char data)
        {
            for (int i = left; i <= right; i++)
            {
                console.Data[i, y] = data;
                console.Color[i, y] = color;
            }
        }

        private void LineVertical(int x, int top, int bottom, byte color, char data)
        {
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
            int width = (int)((console.Width - 3) * ASCII_FPS.playerStats.health / ASCII_FPS.playerStats.maxHealth);

            Rectangle(0, console.Height - 21, 20, console.Height - 1, colorBlack, ' ');
            Border(0, console.Height - 21, 20, console.Height - 1, colorGray, '@');
            Text(10, console.Height - 19, "Health", colorWhite);
            Text(10, console.Height - 17, (int)ASCII_FPS.playerStats.health + " / " + (int)ASCII_FPS.playerStats.maxHealth, colorWhite);
            LineHorizontal(console.Height - 15, 0, 20, colorGray, '@');
        }
    }
}
