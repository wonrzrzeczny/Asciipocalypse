using Microsoft.Xna.Framework.Input;

namespace ASCII_FPS.Input
{
    public static class Controls
    {
        private static KeyboardState kbState0, kbState1;

        public static void UpdateState()
        {
            kbState0 = kbState1;
            kbState1 = Keyboard.GetState();
        }


        public static bool IsPressed(Keys key)
        {
            return kbState1.IsKeyDown(key) && !kbState0.IsKeyDown(key);
        }

        public static bool IsUp(Keys key)
        {
            return kbState1.IsKeyUp(key);
        }

        public static bool IsDown(Keys key)
        {
            return kbState1.IsKeyDown(key);
        }
    }
}
