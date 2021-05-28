using ASCII_FPS.Input;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using System.Reflection;


namespace ASCII_FPS.UI
{
    public class UIKeybinds : UIElement
    {
        private UIStack stack;
        private readonly FieldInfo[] fields;
        private bool waitingForKey = false;
        private int option = 0;


        public Action BackAction { private get; set; }


        public UIKeybinds(UIStack stack)
        {
            this.stack = stack;
            fields = typeof(Keybinds).GetFields();
        }


        public override void Update()
        {
            FieldInfo[] filteredFields =
                   Controls.Scheme == ControlScheme.MouseKeyboard
                   ? fields.Where(f => !f.GetCustomAttribute<KeybindAttribute>().MouseInput).ToArray()
                   : fields;
            if (waitingForKey)
            {
                if (Controls.Scheme == ControlScheme.GamePad)
                {
                    foreach (Buttons button in Enum.GetValues(typeof(Buttons)))
                    {
                        if (Controls.IsPressed(button))
                        {
                            if (button != Buttons.Back)
                            {
                                ((Keybind)filteredFields[option - 2].GetValue(null)).Update(button);
                                Assets.dingDing.Play();
                            }
                            waitingForKey = false;
                            break;
                        }
                    }

                    Keys[] keys = Keyboard.GetState().GetPressedKeys();
                    if (keys.Length > 0 && Controls.IsPressed(keys[0]))
                    {
                        waitingForKey = false;
                    }
                }
                else
                {
                    Keys[] keys = Keyboard.GetState().GetPressedKeys();

                    if (keys.Length > 0 && Controls.IsPressed(keys[0]))
                    {
                        if (keys[0] != Keys.Escape)
                        {
                            ((Keybind)filteredFields[option - 2].GetValue(null)).Update(keys[0]);
                            Assets.dingDing.Play();
                        }
                        waitingForKey = false;
                    }

                    foreach (Buttons button in Enum.GetValues(typeof(Buttons)))
                    {
                        if (Controls.IsPressed(button))
                        {
                            waitingForKey = false;
                            break;
                        }
                    }
                }

                if (!waitingForKey)
                {
                    stack.MenuBackPops = true;
                }
            }
            else
            {
                if (Controls.IsMenuDownPressed())
                {
                    Assets.ding.Play();
                    option = (option + 1) % (filteredFields.Length + 2);
                }
                else if (Controls.IsMenuUpPressed())
                {
                    Assets.ding.Play();
                    option = (option + filteredFields.Length + 1) % (filteredFields.Length + 2);
                }
                else if (Controls.IsMenuAcceptPressed())
                {
                    if (option == 0)
                    {
                        Assets.dingDing.Play();
                        BackAction.Invoke();
                    }
                    else if (option == 1)
                    {
                        Assets.dingDing.Play();
                        Controls.Scheme = (ControlScheme)(((int)Controls.Scheme + 1) % 3);
                    }
                    else
                    {
                        stack.MenuBackPops = false;
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
            UIUtils.Text(console, c, 16, "Input mode - " + Controls.Scheme, option == 1 ? UIUtils.colorLightBlue : UIUtils.colorGray);

            FieldInfo[] filteredFields =
                Controls.Scheme == ControlScheme.MouseKeyboard
                ? fields.Where(f => !f.GetCustomAttribute<KeybindAttribute>().MouseInput).ToArray()
                : fields;
            for (int i = 0; i < filteredFields.Length; i++)
            {
                string keyName = filteredFields[i].GetCustomAttribute<KeybindAttribute>().Name;
                string currentBind = filteredFields[i].GetValue(null).ToString();
                string text = keyName + " - " + (waitingForKey && option == i + 2 ? "< Press key >" : currentBind);
                byte color = option == i + 2 ? UIUtils.colorLightBlue : UIUtils.colorGray;
                UIUtils.Text(console, c, 19 + 2 * i, text, color);
            }
        }
    }
}