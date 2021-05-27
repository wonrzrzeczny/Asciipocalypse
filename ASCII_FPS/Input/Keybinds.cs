using Microsoft.Xna.Framework.Input;
using System;

namespace ASCII_FPS.Input
{
    [AttributeUsage(AttributeTargets.Field)]
    public class KeyNameAttribute : Attribute
    {
        public string Name { get; }

        public KeyNameAttribute(string name)
        {
            Name = name;
        }
    }

    public static class Keybinds
    {
        [KeyName("Walk forward")] public static Keybind forward = new Keybind(Keys.Up, Keys.W, Buttons.LeftThumbstickUp);
        [KeyName("Walk backwards")] public static Keybind backwards = new Keybind(Keys.Down, Keys.S, Buttons.LeftThumbstickDown);
        [KeyName("Turn left")] public static Keybind turnLeft = new Keybind(Keys.Left, null, Buttons.RightThumbstickLeft);
        [KeyName("Turn right")] public static Keybind turnRight = new Keybind(Keys.Right, null, Buttons.RightThumbstickRight);
        [KeyName("Strafe left")] public static Keybind strafeLeft = new Keybind(Keys.Z, Keys.A, Buttons.LeftThumbstickLeft);
        [KeyName("Strafe right")] public static Keybind strafeRight = new Keybind(Keys.X, Keys.D, Buttons.LeftThumbstickRight);
        [KeyName("Sprint")] public static Keybind sprint = new Keybind(Keys.LeftShift, Keys.LeftShift, Buttons.LeftTrigger);
        [KeyName("Fire")] public static Keybind fire = new Keybind(Keys.Space, null, Buttons.RightTrigger);
        [KeyName("Action")] public static Keybind action = new Keybind(Keys.Enter, Keys.E, Buttons.A);
        [KeyName("Skill menu")] public static Keybind skills = new Keybind(Keys.P, Keys.P, Buttons.B);
    }
}
