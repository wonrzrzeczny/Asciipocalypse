using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace ASCII_FPS.Input
{
    public enum ControlScheme
    {
        Keyboard, MouseKeyboard, GamePad
    }

    public static class Controls
    {
        public static float MouseSensitivity { get; set; } = 1f;

        private static KeyboardState kbState0, kbState1;
        private static GamePadState gpState0, gpState1;
        private static MouseState mState;

        // Mouse movement is smoothed over time, because Monogame sometimes has problems with querying mouse position in time
        private static float mouseDelta;
        private static readonly float[] mouseDeltaBuffer;
        private static int mouseDeltaBufferCtr;
        private static int screenWidth, screenHeight;
        private const int mouseDeltaBufferSize = 5;

        static Controls()
        {
            mouseDeltaBuffer = new float[mouseDeltaBufferSize];
        }


        public static ControlScheme Scheme { get; set; } = ControlScheme.Keyboard;

        public static void UpdateState(ASCII_FPS game, GraphicsDeviceManager graphics)
        {
            if (!game.IsActive)
                return;

            screenWidth = graphics.PreferredBackBufferWidth;
            screenHeight = graphics.PreferredBackBufferHeight;

            kbState0 = kbState1;
            kbState1 = Keyboard.GetState();
            gpState0 = gpState1;
            gpState1 = GamePad.GetState(0);

            mState = Mouse.GetState();

            // TODO: better smoothing algorithm
            mouseDelta -= mouseDeltaBuffer[mouseDeltaBufferCtr] / mouseDeltaBufferSize;
            mouseDeltaBuffer[mouseDeltaBufferCtr] = mState.X * 2f / screenWidth - 1f;
            mouseDelta += mouseDeltaBuffer[mouseDeltaBufferCtr] / mouseDeltaBufferSize;
            mouseDeltaBufferCtr++;
            mouseDeltaBufferCtr %= mouseDeltaBufferSize;

            Mouse.SetPosition(screenWidth / 2, screenHeight / 2);
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

        public static float MouseDelta() => mouseDelta * MouseSensitivity;
        public static bool IsLMBDown() => mState.LeftButton == ButtonState.Pressed;

        public static bool IsMenuDownPressed() => IsPressed(Keys.Down) || IsPressed(Keys.S) ||
            IsPressed(Buttons.DPadDown) || IsPressed(Buttons.LeftThumbstickDown) || IsPressed(Buttons.RightThumbstickDown);
        public static bool IsMenuUpPressed() => IsPressed(Keys.Up) || IsPressed(Keys.W) ||
            IsPressed(Buttons.DPadUp) || IsPressed(Buttons.LeftThumbstickUp) || IsPressed(Buttons.RightThumbstickUp);
        public static bool IsMenuAcceptPressed() => IsPressed(Keys.Enter) || IsPressed(Keys.Space) || IsPressed(Buttons.A);
        public static bool IsMenuBackPressed() => IsPressed(Keys.Escape) || IsPressed(Buttons.Back) || IsPressed(Buttons.B);
        public static bool IsInGameBackPressed() => IsPressed(Keys.Escape) || IsPressed(Buttons.Back);

    }
}
