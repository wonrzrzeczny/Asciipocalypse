using ASCII_FPS.GameComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Reflection;


namespace ASCII_FPS.UI
{
    public class UIKeybinds : UIElement
    {
        private readonly byte colorLightBlue = Mathg.ColorTo8Bit(Color.LightBlue.ToVector3());
        private readonly byte colorWhite = Mathg.ColorTo8Bit(Color.White.ToVector3());
        private readonly byte colorGray = Mathg.ColorTo8Bit(Color.DarkGray.ToVector3());

        private readonly FieldInfo[] fields;
        private bool waitingForKey = false;
        private int option = 0;


        public Action BackAction { private get; set; }


        public UIKeybinds()
        {
            fields = typeof(Keybinds).GetFields();
        }


        public override void Update(KeyboardState keyboard, KeyboardState keyboardPrev)
        {
            if (waitingForKey)
            {
                Keys[] keys = keyboard.GetPressedKeys();
                if (keys.Length > 0 && !keyboardPrev.IsKeyDown(keys[0]))
                {
                    fields[option - 1].SetValue(null, keys[0]);
                    waitingForKey = false;
                }
            }
            else
            {
                if (keyboard.IsKeyDown(Keys.Down) && !keyboardPrev.IsKeyDown(Keys.Down))
                {
                    option = (option + 1) % (fields.Length + 1);
                }
                else if (keyboard.IsKeyDown(Keys.Up) && !keyboardPrev.IsKeyDown(Keys.Up))
                {
                    option = (option + fields.Length) % (fields.Length + 1);
                }
                else if (keyboard.IsKeyDown(Keys.Enter) && !keyboardPrev.IsKeyDown(Keys.Enter))
                {
                    if (option == 0)
                    {
                        BackAction.Invoke();
                    }
                    else
                    {
                        waitingForKey = true;
                    }
                }
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

            int c = console.Width / 2;

            Text(console, c, 12, "Back", option == 0 ? colorLightBlue : colorGray);
            for (int i = 0; i < fields.Length; i++)
            {
                string text = fields[i].Name + " - " + (waitingForKey && option == i + 1 ? "< Press key >" : fields[i].GetValue(null));
                byte color = option == i + 1 ? colorLightBlue : colorGray;
                Text(console, c, 16 + 2 * i, text, color);
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