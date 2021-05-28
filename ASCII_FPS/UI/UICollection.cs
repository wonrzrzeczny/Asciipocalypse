using ASCII_FPS.Input;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace ASCII_FPS.UI
{
    public class UICollection : UIElement
    {
        private readonly List<UIElement> elements;

        public bool AllowSwitching { get; set; } = false;
        public Keys KeyPreviousTab { get; set; } = Keys.Left;
        public Keys KeyNextTab { get; set; } = Keys.Right;

        private int option = 0;
        private bool switchedThisFrame = false;

        public UICollection()
        {
            elements = new List<UIElement>();
        }


        public void AddElement(UIElement element)
        {
            elements.Add(element);
        }

        public void NextTab()
        {
            option = (option + 1) % elements.Count;
            switchedThisFrame = true;
        }

        public void PreviousTab()
        {
            option = (option + elements.Count - 1) % elements.Count;
            switchedThisFrame = true;
        }


        public override void Draw(Console console)
        {
            foreach (UIElement element in elements)
            {
                element.Draw(console);
            }
        }

        public override void Update()
        {
            if (AllowSwitching)
            {
                if (Controls.IsPressed(KeyNextTab))
                {
                    NextTab();
                }
                else if (Controls.IsPressed(KeyPreviousTab))
                {
                    PreviousTab();
                }
            }

            if (switchedThisFrame)
            {
                switchedThisFrame = false;
                return;
            }

            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].IsActive = option == i;
                if (option == i)
                {
                    elements[option].Update();
                    if (switchedThisFrame)
                    {
                        switchedThisFrame = false;
                        return;
                    }
                }
            }
        }
    }
}
