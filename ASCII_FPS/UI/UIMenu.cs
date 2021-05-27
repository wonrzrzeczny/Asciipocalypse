using ASCII_FPS.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;


namespace ASCII_FPS.UI
{
    public class UIMenu : UIElement
    {
        protected readonly UIPosition boundsStart;
        protected readonly UIPosition boundsEnd;
        protected readonly List<MenuEntry> entries;
        protected int option = 0;

        public Action MovePastLastBehaviour { get; set; }
        public Action MoveBeforeFirstBehaviour { get; set; }


        public UIMenu(UIPosition boundsStart, UIPosition boundsEnd)
        {
            this.boundsStart = boundsStart;
            this.boundsEnd = boundsEnd;
            entries = new List<MenuEntry>();
            MovePastLastBehaviour = MoveToFirst;
            MoveBeforeFirstBehaviour = MoveToLast;
        }

        public UIMenu() : this(UIPosition.TopLeft, UIPosition.BottomRight) { }


        public override void Update()
        {
            if (!IsActive)
            {
                return;
            }

            if (entries[option].IsHidden || !entries[option].IsCallable)
            {
                int? next = GetNextSlot(option);
                if (next == null)
                {
                    option = GetPreviousSlot(option).Value;
                }
                else
                {
                    option = next.Value;
                }
            }

            if (Controls.IsPressed(Keys.Down))
            {
                Assets.ding.Play();
                int? next = GetNextSlot(option);
                if (next == null)
                {
                    MovePastLastBehaviour.Invoke();
                }
                else
                {
                    option = next.Value;
                }
            }
            else if (Controls.IsPressed(Keys.Up))
            {
                Assets.ding.Play();
                int? prev = GetPreviousSlot(option);
                if (prev == null)
                {
                    MoveBeforeFirstBehaviour.Invoke();
                }
                else
                {
                    option = prev.Value;
                }
            }
            else if (Controls.IsPressed(Keys.Enter))
            {
                Assets.dingDing.Play();
                entries[option].Call();
            }
        }

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
            foreach (MenuEntry entry in entries)
            {
                if (!entry.IsHidden)
                {
                    byte color = (IsActive && entry == entries[option]) ? entry.ColorSelected : entry.Color;
                    UIUtils.Text(console, c, entry.Position, entry.Text, color);
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


        private int? GetNextSlot(int position)
        {
            do
            {
                position++;
                if (position == entries.Count)
                {
                    return null;
                }
            }
            while (entries[position].IsHidden || !entries[position].IsCallable);
            
            return position;
        }

        private int? GetPreviousSlot(int position)
        {
            do
            {
                position--;
                if (position == -1)
                {
                    return null;
                }
            }
            while (entries[position].IsHidden || !entries[position].IsCallable);

            return position;
        }
    }
}
