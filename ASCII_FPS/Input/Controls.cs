using Microsoft.Xna.Framework.Input;

namespace ASCII_FPS.Input
{
    public enum ControlScheme
    {
        Keyboard, MouseKeyboard, GamePad
    }

    public static class Controls
    {
        private static KeyboardState kbState0, kbState1;
        private static GamePadState gpState0, gpState1;

        public static ControlScheme Scheme { get; set; } = ControlScheme.Keyboard;

        public static void UpdateState()
        {
            kbState0 = kbState1;
            kbState1 = Keyboard.GetState();
            gpState0 = gpState1;
            gpState1 = GamePad.GetState(0);
        }


        public static bool IsPressed(Keys key) => kbState1.IsKeyDown(key) && kbState0.IsKeyUp(key);
        public static bool IsPressed(Buttons button) => gpState1.IsButtonDown(button) && gpState0.IsButtonUp(button);
        public static bool IsPressed(Keybind keybind) => keybind.IsDown(kbState1, gpState1, Scheme)
                                                      && !keybind.IsDown(kbState0, gpState0, Scheme);

        public static bool IsDown(Keys key) => kbState1.IsKeyDown(key);
        public static bool IsDown(Buttons button) => gpState1.IsButtonDown(button);
        public static bool IsDown(Keybind keybind) => keybind.IsDown(kbState1, gpState1, Scheme);

        public static bool IsUp(Keys key) => kbState1.IsKeyUp(key);
        public static bool IsUp(Buttons button) => gpState1.IsButtonUp(button);
        public static bool IsUp(Keybind keybind) => !keybind.IsDown(kbState1, gpState1, Scheme);

    }
}
