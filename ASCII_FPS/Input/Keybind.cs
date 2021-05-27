using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASCII_FPS.Input
{
    // bindings for different control modes
    public class Keybind
    {
        public Keys? Keyboard { get; private set; }
        public Keys? MouseKeyboard { get; private set; }
        public Buttons? GamePad { get; private set; }


        public Keybind(Keys? keyboard, Keys? mouseKeyboard, Buttons? gamePad)
        {
            Keyboard = keyboard;
            MouseKeyboard = mouseKeyboard;
            GamePad = gamePad;
        }


        public void Update(Keys keys)
        {
            switch (Controls.Scheme)
            {
                case ControlScheme.Keyboard:
                    Keyboard = keys;
                    break;
                case ControlScheme.MouseKeyboard:
                    MouseKeyboard = keys;
                    break;
            }
        }

        public void Update(Buttons button)
        {
            switch (Controls.Scheme)
            {
                case ControlScheme.GamePad:
                    GamePad = button;
                    break;
            }
        }

        public bool IsDown(KeyboardState kbState, GamePadState gpState, ControlScheme controlScheme)
        {
            return controlScheme switch
            {
                ControlScheme.Keyboard => Keyboard != null && kbState.IsKeyDown(Keyboard.Value),
                ControlScheme.MouseKeyboard => MouseKeyboard != null && kbState.IsKeyDown(MouseKeyboard.Value),
                ControlScheme.GamePad => GamePad != null && gpState.IsButtonDown(GamePad.Value),
                _ => throw new NotImplementedException(),
            };
        }


        public string Serialize()
        {
            return Keyboard + "/" + MouseKeyboard + "/" + GamePad;
        }

        public string Display(ControlScheme controlScheme)
        {
            return controlScheme switch
            {
                ControlScheme.Keyboard => Keyboard.ToString(),
                ControlScheme.MouseKeyboard => MouseKeyboard.ToString(),
                ControlScheme.GamePad => GamePad.ToString(),
                _ => throw new NotImplementedException(),
            };
        }

        public static bool TryParse(string s, out Keybind keybind)
        {
            keybind = null;

            string[] parts = s.Split('/');
            if (parts.Length != 3)
                return false;

            Keys? keyboard = null;
            Keys? mouseKeyboard = null;
            Buttons? gamePad = null;

            if (Enum.TryParse(parts[0], out Keys keyboard2))
                keyboard = keyboard2;
            if (Enum.TryParse(parts[1], out Keys mouseKeyboard2))
                mouseKeyboard = mouseKeyboard2;
            if (Enum.TryParse(parts[2], out Buttons gamePad2))
                gamePad = gamePad2;

            keybind = new Keybind(keyboard, mouseKeyboard, gamePad);
            return true;
        }
    }
}
