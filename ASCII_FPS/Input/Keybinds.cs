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
        [KeyName("Walk forward")] public static Keybind forward = new Keybind(Keys.Up, Keys.W, Buttons.DPadUp);
        [KeyName("Walk backwards")] public static Keybind backwards = new Keybind(Keys.Down, Keys.S, Buttons.DPadDown);
        [KeyName("Turn left")] public static Keybind turnLeft = new Keybind(Keys.Left, null, null);
        [KeyName("Turn right")] public static Keybind turnRight = new Keybind(Keys.Right, null, null);
        [KeyName("Strafe left")] public static Keybind strafeLeft = new Keybind(Keys.Z, Keys.A, Buttons.DPadLeft);
        [KeyName("Strafe right")] public static Keybind strafeRight = new Keybind(Keys.X, Keys.D, Buttons.DPadRight);
        [KeyName("Sprint")] public static Keybind sprint = new Keybind(Keys.LeftShift, Keys.LeftShift, Buttons.LeftShoulder);
        [KeyName("Fire")] public static Keybind fire = new Keybind(Keys.Space, null, Buttons.RightShoulder);
        [KeyName("Action")] public static Keybind action = new Keybind(Keys.Enter, Keys.E, Buttons.A);
        [KeyName("Skill menu")] public static Keybind skills = new Keybind(Keys.P, Keys.P, Buttons.B);
    }
}
