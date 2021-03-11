using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASCII_FPS.UI
{
    public class UIScrollMenu : UIMenu
    {
        public UIScrollMenu(UIPosition boundsStart, UIPosition boundsEnd)
            : base(boundsStart, boundsEnd) { }

        public UIScrollMenu() : base() { }

        private int offset = 0;


        public override void Draw(Console console)
        {
            Point start = boundsStart.GetPosition(console);
            Point end = boundsEnd.GetPosition(console);
            for (int x = start.X; x <= end.X; x++)
            {
                for (int y = start.Y; y <= end.Y; y++)
                {
                    console.Data[x, y] = ' ';
                }
            }

            int c = (end.X + start.X) / 2;
            int minPosition = entries.Min((MenuEntry e) => e.Position);
            int maxPosition = entries.Max((MenuEntry e) => e.Position);
            if (entries[option].Position + offset < start.Y)
            {
                offset = start.Y - entries[option].Position;
            }
            if (entries[option].Position + offset > end.Y)
            {
                offset = end.Y - entries[option].Position;
            }

            foreach (MenuEntry entry in entries)
            {
                if (!entry.IsHidden && entry.Position + offset >= start.Y && entry.Position + offset <= end.Y)
                {
                    byte color = (IsActive && entry == entries[option]) ? entry.ColorSelected : entry.Color;
                    UIUtils.Text(console, c, entry.Position + offset, entry.Text, color);
                }
            }
        }
    }
}
