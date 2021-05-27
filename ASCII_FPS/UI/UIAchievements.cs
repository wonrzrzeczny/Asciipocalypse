using ASCII_FPS.GameComponents;
using ASCII_FPS.Input;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASCII_FPS.UI
{
    public class UIAchievements : UIElement
    {
        private int offset = 0;

        public Action BackAction { private get; set; }


        public override void Update()
        {
            if (Controls.IsPressed(Keys.Enter))
            {
                Assets.dingDing.Play();
                BackAction.Invoke();
            }
            else if (Controls.IsPressed(Keys.Up))
            {
                offset--;
            }
            else if (Controls.IsPressed(Keys.Down))
            {
                offset++;
            }
        }

        public override void Draw(Console console)
        {
            for (int x = 0; x < console.Width; x++)
            {
                for (int y = 0; y < console.Height; y++)
                {
                    console.Data[x, y] = ' ';
                }
            }

            List<Achievements.Entry> achievements = Achievements.Entries;
            achievements.Sort(new Comparison<Achievements.Entry>(
                (Achievements.Entry e1, Achievements.Entry e2) =>
                {
                    if (e1.Progress == e2.Progress)
                    {
                        return e1.ID.CompareTo(e2.ID);
                    }
                    return e2.Progress.CompareTo(e1.Progress);
                }
            ));

            int limit = (console.Height - 22) / 2;
            offset = Math.Clamp(offset, 0, achievements.Count - limit);

            int unlocked = achievements.Count((Achievements.Entry e) => e.Progress == 1);
            int total = achievements.Count;

            int xleft = console.Width / 5;

            UIUtils.Text(console, console.Width / 2, 8, "Back", UIUtils.colorLightBlue);
            for (int i = offset; i < total && i < offset + limit; i++)
            {
                byte color = achievements[i].Progress == 1 ? UIUtils.colorLightGray : UIUtils.colorDarkGray;
                UIUtils.Text(console, xleft, 14 + 2 * (i - offset), achievements[i].Description, color, UIAlignment.Left);
            }

            UIUtils.Text(console, console.Width / 2, console.Height - 6, unlocked + " unlocked out of " + total, UIUtils.colorWhite);
        }
    }
}
