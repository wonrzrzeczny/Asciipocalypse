using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASCII_FPS.UI
{
    public class UIStack : UIElement
    {
        private Stack<UIElement> stack;

        public UIStack(UIElement root)
        {
            stack = new Stack<UIElement>();
            stack.Push(root);
        }


        public override void Draw(Console console)
        {
            stack.Peek().Draw(console);
        }

        public override void Update(KeyboardState keyboard, KeyboardState keyboardPrev)
        {
            if (keyboard.IsKeyDown(Keys.Escape) && !keyboardPrev.IsKeyDown(Keys.Escape))
            {
                Pop();
            }
            else
            {
                stack.Peek().Update(keyboard, keyboardPrev);
            }
        }


        public void Pop()
        {
            if (stack.Count > 1)
            {
                stack.Pop();
            }
        }

        public void Push(UIElement element)
        {
            stack.Push(element);
        }

        public void Clear()
        {
            while (stack.Count > 1)
            {
                stack.Pop();
            }
        }
    }
}
