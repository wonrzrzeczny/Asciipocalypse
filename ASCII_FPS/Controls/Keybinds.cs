using Microsoft.Xna.Framework.Input;
using System;

namespace ASCII_FPS.Controls
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
        [KeyName("Walk forward")] public static Keys forward = Keys.Up;
        [KeyName("Walk backwards")] public static Keys backwards = Keys.Down;
        [KeyName("Turn left")] public static Keys turnLeft = Keys.Left;
        [KeyName("Turn right")] public static Keys turnRight = Keys.Right;
        [KeyName("Strafe left")] public static Keys strafeLeft = Keys.Z;
        [KeyName("Strafe right")] public static Keys strafeRight = Keys.X;
        [KeyName("Sprint")] public static Keys sprint = Keys.LeftShift;
        [KeyName("Fire")] public static Keys fire = Keys.Space;
        [KeyName("Action")] public static Keys action = Keys.Enter;
        [KeyName("Skill menu")] public static Keys skills = Keys.P;
    }
}
