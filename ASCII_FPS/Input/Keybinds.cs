using Microsoft.Xna.Framework.Input;
using System;

namespace ASCII_FPS.Input
{
    [AttributeUsage(AttributeTargets.Field)]
    public class KeybindAttribute : Attribute
    {
        public string Name { get; }
        public bool MouseInput { get; }

        public KeybindAttribute(string name, bool mouseInput = false)
        {
            Name = name;
            MouseInput = mouseInput;
        }
    }

    public static class Keybinds
    {
        [Keybind("Walk forward")] public static Keybind forward = new Keybind(Keys.Up, Keys.W, Buttons.LeftThumbstickUp);
        [Keybind("Walk backwards")] public static Keybind backwards = new Keybind(Keys.Down, Keys.S, Buttons.LeftThumbstickDown);
        [Keybind("Turn left", true)] public static Keybind turnLeft = new Keybind(Keys.Left, null, Buttons.RightThumbstickLeft);
        [Keybind("Turn right", true)] public static Keybind turnRight = new Keybind(Keys.Right, null, Buttons.RightThumbstickRight);
        [Keybind("Strafe left")] public static Keybind strafeLeft = new Keybind(Keys.Z, Keys.A, Buttons.LeftThumbstickLeft);
        [Keybind("Strafe right")] public static Keybind strafeRight = new Keybind(Keys.X, Keys.D, Buttons.LeftThumbstickRight);
        [Keybind("Sprint")] public static Keybind sprint = new Keybind(Keys.LeftShift, Keys.LeftShift, Buttons.LeftTrigger);
        [Keybind("Fire", true)] public static Keybind fire = new Keybind(Keys.Space, null, Buttons.RightTrigger);
        [Keybind("Action")] public static Keybind action = new Keybind(Keys.Enter, Keys.E, Buttons.A);
        [Keybind("Skill menu")] public static Keybind skills = new Keybind(Keys.P, Keys.Tab, Buttons.B);
    }
}
