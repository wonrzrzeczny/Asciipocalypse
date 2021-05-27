using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASCII_FPS.Input
{
    // bindings for different control modes
    public struct Keybind
    {
        public Keys? Keyboard { get; }
        public Keys? MouseKeyboard { get; }
        public Buttons? GamePad { get; }

        public bool AllowKeyboardInGamepadMode { get; }


        public Keybind(Keys? keyboard, Keys? mouseKeyboard, Buttons? gamePad, bool allowKeyboardInGamepadMode = false)
        {
            Keyboard = keyboard;
            MouseKeyboard = mouseKeyboard;
            GamePad = gamePad;
            AllowKeyboardInGamepadMode = allowKeyboardInGamepadMode;
        }


        public bool IsDown(KeyboardState kbState, GamePadState gpState, ControlScheme controlScheme)
        {
            switch (controlScheme)
            {
                case ControlScheme.Keyboard:
                    return Keyboard != null && kbState.IsKeyDown(Keyboard.Value);
                case ControlScheme.MouseKeyboard:
                    return MouseKeyboard != null && kbState.IsKeyDown(MouseKeyboard.Value);
                case ControlScheme.GamePad:
                    return (GamePad != null && gpState.IsButtonDown(GamePad.Value) )
                        || (AllowKeyboardInGamepadMode && Keyboard != null && kbState.IsKeyDown(Keyboard.Value));
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
