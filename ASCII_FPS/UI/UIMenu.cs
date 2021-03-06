using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;


namespace ASCII_FPS.UI
{
    public class UIMenu : UIElement
    {
        private Rectangle bounds;

        private readonly List<MenuEntry> callableEntries;
        private readonly List<MenuEntry> nonCallableEntries;
        private int option = 0;

        public MenuEntry SelectedEntry
        {
            get
            {
                if (callableEntries.Count == 0)
                {
                    return null;
                }
                return callableEntries[option];
            }
        }


        public UIMenu(Rectangle bounds)
        {
            this.bounds = bounds;
            callableEntries = new List<MenuEntry>();
            nonCallableEntries = new List<MenuEntry>();
        }


        public override void Update(KeyboardState keyboard, KeyboardState keyboardPrev)
        {
            while (callableEntries[option].IsHidden)
            {
                option = (option + 1) % callableEntries.Count;
            }

            if (keyboard.IsKeyDown(Keys.Down) && !keyboardPrev.IsKeyDown(Keys.Down))
            {
                do
                {
                    option = (option + 1) % callableEntries.Count;
                }
                while (callableEntries[option].IsHidden);
            }
            else if (keyboard.IsKeyDown(Keys.Up) && !keyboardPrev.IsKeyDown(Keys.Up))
            {
                do
                {
                    option = (option + callableEntries.Count - 1) % callableEntries.Count;
                }
                while (callableEntries[option].IsHidden);
            }
            else if (keyboard.IsKeyDown(Keys.Enter) && !keyboardPrev.IsKeyDown(Keys.Enter))
            {
                callableEntries[option].Call();
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
            foreach (MenuEntry entry in callableEntries)
            {
                if (!entry.IsHidden)
                {
                    byte color = entry == callableEntries[option] ? entry.ColorSelected : entry.Color;
                    Text(console, c, entry.Position, entry.Text, color);
                }
            }
            foreach (MenuEntry entry in nonCallableEntries)
            {
                if (!entry.IsHidden)
                {
                    Text(console, c, entry.Position, entry.Text, entry.Color);
                }
            }
        }


        public void AddEntry(MenuEntry entry)
        {
            if (entry.IsCallable)
            {
                callableEntries.Add(entry);
                callableEntries.Sort();
            }
            else
            {
                nonCallableEntries.Add(entry);
                callableEntries.Sort();
            }
        }

        public void ResetCursor()
        {
            option = 0;
            while (callableEntries[option].IsHidden)
            {
                option = (option + 1) % callableEntries.Count;
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
