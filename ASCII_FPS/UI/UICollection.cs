using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASCII_FPS.UI
{
    public class UICollection : UIElement
    {
        private readonly List<UIElement> elements;

        public UICollection()
        {
            elements = new List<UIElement>();
        }


        public void AddElement(UIElement element)
        {
            elements.Add(element);
        }

        public override void Draw(Console console)
        {
            foreach (UIElement element in elements)
            {
                element.Draw(console);
            }
        }

        public override void Update(KeyboardState keyboard, KeyboardState keyboardPrev)
        {
            foreach (UIElement element in elements)
            {
                element.Update(keyboard, keyboardPrev);
            }
        }
    }
}
