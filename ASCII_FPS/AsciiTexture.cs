using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ASCII_FPS
{
    public class AsciiTexture
    {
        private readonly Vector3[,] colors;

        public AsciiTexture(Texture2D texture)
        {
            if (texture.Width != 256 || texture.Height != 256)
                throw new Exception("Invalid texture size");

            Color[] color1d = new Color[256 * 256];
            texture.GetData(color1d);
            colors = new Vector3[256, 256];
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    colors[i, j] = color1d[i + j * 256].ToVector3();
                }
            }
        }
        
        public Vector3 Sample(Vector2 uv)
        {
            return colors[(int)(uv.X * 256) & 0xff, (int)(uv.Y * 256) & 0xff];
        }
    }
}
