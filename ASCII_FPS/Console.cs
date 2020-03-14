using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCII_FPS
{
    public class Console
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public const int FONT_SIZE = 12;

        public char[,] Data { get; set; }
        public byte[,] Color { get; set; }

        public Console(int width, int height)
        {
            Width = width;
            Height = height;
            Data = new char[width, height];
            Color = new byte[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Data[i, j] = ' ';
                    Color[i, j] = 0;
                }
            }
        }
    }
}
