using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;


namespace ASCII_FPS.UI
{
    public class UIMenu : UIElement
    {
        private Rectangle bounds;

        private readonly List<MenuEntry> entries;
        private int option = 0;

        public MenuEntry SelectedEntry
        {
            get
            {
                if (entries.Count == 0)
                {
                    return null;
                }
                return entries[option];
            }
        }


        public UIMenu(Rectangle bounds)
        {
            this.bounds = bounds;
            entries = new List<MenuEntry>();
        }


        public override void Update(KeyboardState keyboard, KeyboardState keyboardPrev)
        {
            while (entries[option].IsHidden || !entries[option].IsCallable)
            {
                option = (option + 1) % entries.Count;
            }

            if (keyboard.IsKeyDown(Keys.Down) && !keyboardPrev.IsKeyDown(Keys.Down))
            {
                do
                {
                    option = (option + 1) % entries.Count;
                }
                while (entries[option].IsHidden || !entries[option].IsCallable);
            }
            else if (keyboard.IsKeyDown(Keys.Up) && !keyboardPrev.IsKeyDown(Keys.Up))
            {
                do
                {
                    option = (option + entries.Count - 1) % entries.Count;
                }
                while (entries[option].IsHidden || !entries[option].IsCallable);
            }
            else if (keyboard.IsKeyDown(Keys.Enter) && !keyboardPrev.IsKeyDown(Keys.Enter))
            {
                entries[option].Call();
            }
        }

        public override void Draw(Console console)
        {
            for (int x = bounds.Left; x < bounds.Right; x++)
            {
                for (int y = bounds.Top; y < bounds.Bottom; y++)
                {
                    console.Data[x, y] = ' ';
                }
            }

            int c = console.Width / 2;
            foreach (MenuEntry entry in entries)
            {
                if (!entry.IsHidden)
                {
                    byte color = entry == entries[option] ? entry.ColorSelected : entry.Color;
                    Text(console, c, entry.Position, entry.Text, color);
                }
            }
        }


        public void AddEntry(MenuEntry entry)
        {
            entries.Add(entry);
            entries.Sort();
        }

        public void MoveToFirst()
        {
            option = 0;
            while (entries[option].IsHidden || !entries[option].IsCallable)
            {
                option = (option + 1) % entries.Count;
            }
        }

        public void MoveToLast()
        {
            option = entries.Count - 1;
            while (entries[option].IsHidden || !entries[option].IsCallable)
            {
                option = (option + entries.Count - 1) % entries.Count;
            }
        }


        private void Text(Console console, int x, int y, string text, byte color)
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
    }
}
