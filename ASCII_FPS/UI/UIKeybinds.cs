using ASCII_FPS.Input;
using Microsoft.Xna.Framework.Input;
using System;
using System.Reflection;


namespace ASCII_FPS.UI
{
    public class UIKeybinds : UIElement
    {
        private readonly FieldInfo[] fields;
        private bool waitingForKey = false;
        private int option = 0;


        public Action BackAction { private get; set; }


        public UIKeybinds()
        {
            fields = typeof(Keybinds).GetFields();
        }


        public override void Update()
        {
            if (waitingForKey)
            {
                Keys[] keys = Keyboard.GetState().GetPressedKeys();
                if (keys.Length > 0 && Controls.IsPressed(keys[0]))
                {
                    ((Keybind)fields[option - 1].GetValue(null)).Update(keys[0]);
                    Assets.dingDing.Play();
                    waitingForKey = false;
                }
            }
            else
            {
                if (Controls.IsPressed(Keys.Down))
                {
                    Assets.ding.Play();
                    option = (option + 1) % (fields.Length + 1);
                }
                else if (Controls.IsPressed(Keys.Up))
                {
                    Assets.ding.Play();
                    option = (option + fields.Length) % (fields.Length + 1);
                }
                else if (Controls.IsPressed(Keys.Enter))
                {
                    if (option == 0)
                    {
                        Assets.dingDing.Play();
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

            UIUtils.Text(console, c, 12, "Back", option == 0 ? UIUtils.colorLightBlue : UIUtils.colorGray);
            for (int i = 0; i < fields.Length; i++)
            {
                string keyName = fields[i].GetCustomAttribute<KeyNameAttribute>().Name;
                string currentBind = ((Keybind)fields[i].GetValue(null)).Display(Controls.Scheme);
                string text = keyName + " - " + (waitingForKey && option == i + 1 ? "< Press key >" : currentBind);
                byte color = option == i + 1 ? UIUtils.colorLightBlue : UIUtils.colorGray;
                UIUtils.Text(console, c, 16 + 2 * i, text, color);
            }
        }
    }
}