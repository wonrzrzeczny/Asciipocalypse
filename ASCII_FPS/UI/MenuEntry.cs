using System;

namespace ASCII_FPS.UI
{
    public class MenuEntry : IComparable<MenuEntry>
    {
        private readonly string text;
        private readonly Func<string> textFunc;
        private readonly Action callback;


        public int Position { get; private set; }
        public byte Color { get; private set; }
        public byte ColorSelected { get; private set; }
        public bool IsHidden { get; set; } = false;

        public string Text
        {
            get
            {
                return textFunc?.Invoke() ?? text;
            }
        }

        public bool IsCallable
        {
            get
            {
                return callback != null;
            }
        }


        public MenuEntry(int position, string text, Action callback, byte color, byte colorSelected)
        {
            Position = position;
            this.text = text;
            this.callback = callback;
            Color = color;
            ColorSelected = colorSelected;
        }

        public MenuEntry(int position, string text, byte color)
            : this(position, text, null, color, color) { }

        public MenuEntry(int position, Func<string> textFunc, Action callback, byte color, byte colorSelected)
        {
            Position = position;
            this.textFunc = textFunc;
            this.callback = callback;
            Color = color;
            ColorSelected = colorSelected;
        }

        public MenuEntry(int position, Func<string> textFunc, byte color)
            : this(position, textFunc, null, color, color) { }


        public void Call()
        {
            callback.Invoke();
        }


        public int CompareTo(MenuEntry other)
        {
            return Position.CompareTo(other.Position);
        }
    }
}
