using Microsoft.Xna.Framework;
using System;

namespace ASCII_FPS.UI
{
    public class UIPosition
    {
        public Vector2 Relative { get; }
        public Point Offset { get; }


        public UIPosition(Vector2 relative, Point offset)
        {
            Relative = relative;
            Offset = offset;
        }

        public UIPosition(Vector2 relative) : this(relative, Point.Zero) { }
        public UIPosition(Point offset) : this(Vector2.Zero, offset) { }

        public static UIPosition TopLeft
        {
            get { return new UIPosition(Vector2.Zero, Point.Zero); }
        }

        public static UIPosition BottomRight
        {
            get { return new UIPosition(Vector2.One, Point.Zero); }
        }


        public int GetX(Console console)
        {
            int x = Offset.X + (int)Math.Round(Relative.X * console.Width);
            return Math.Clamp(x, 0, console.Width - 1);
        }

        public int GetY(Console console)
        {
            int y = Offset.Y + (int)Math.Round(Relative.Y * console.Height);
            return Math.Clamp(y, 0, console.Height - 1);
        }
        
        public Point GetPosition(Console console)
        {
            return new Point(GetX(console), GetY(console));
        }
    }
}
