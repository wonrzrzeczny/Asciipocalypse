using Microsoft.Xna.Framework;

namespace ASCII_FPS
{
    public struct RectangleF
    {
        public float X { get; }
        public float Y { get; }
        public float Width { get; }
        public float Height { get; }

        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool TestPoint(Vector2 point)
        {
            return point.X >= X && point.X <= X + Width && point.Y >= Y && point.Y <= Y + Height;
        }

        public override string ToString()
        {
            return "(" + X + ", " + Y + ", " + Width + " x " + Height + ")";
        }
    }
}
