using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ASCII_FPS
{
    public class AsciiTexture
    {
        private readonly Vector3[,] colors;

        public int ID { get; private set; }

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

            Register(this);
        }
        
        public Vector3 Sample(Vector2 uv)
        {
            return colors[(int)(uv.X * 256) & 0xff, (int)(uv.Y * 256) & 0xff];
        }



        private static List<AsciiTexture> textures = new List<AsciiTexture>();

        private static void Register(AsciiTexture asciiTexture)
        {
            asciiTexture.ID = textures.Count;
            textures.Add(asciiTexture);
        }

        public static AsciiTexture GetByID(int id)
        {
            return textures[id];
        }
    }
}
