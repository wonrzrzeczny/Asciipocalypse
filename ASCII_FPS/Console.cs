namespace ASCII_FPS
{
    public class Console
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public const int FONT_SIZE = 12;

        public char[,] Data { get; set; }
        public byte[,] Color { get; set; }

        public enum ColorEffect { None, Grayscale, Red, Fire }
        public ColorEffect Effect { get; set; } = ColorEffect.None;

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

        public byte GetColor(int x, int y)
        {
            byte color = Color[x, y];
            switch (Effect)
            {
                case ColorEffect.Grayscale:
                    int sum = (color & 0b111) + ((color >> 3) & 0b111) + ((color >> 5) & 0b110);
                    sum /= 3;
                    color = (byte)(sum + (sum << 3) + ((sum & 0b110) << 5));
                    break;
                case ColorEffect.Red:
                    sum = (color & 0b111) + ((color >> 3) & 0b111) + ((color >> 5) & 0b110);
                    color = (byte)(sum >> 1);
                    break;
                case ColorEffect.Fire:
                    byte r = (byte)(color & 0b111);
                    byte g = (byte)((color >> 3) & 0b111);
                    color = (byte)((r + 1 > 7 ? 7 : r + 1) | ((g >> 1) << 3));
                    break;
            }

            return color;
        }
    }
}
