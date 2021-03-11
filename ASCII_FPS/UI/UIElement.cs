using Microsoft.Xna.Framework.Input;

namespace ASCII_FPS.UI
{
    public abstract class UIElement
    {
        public abstract void Draw(Console console);
        public abstract void Update(KeyboardState keyboard, KeyboardState keyboardPrev);

        public bool IsActive { get; set; } = true;
    }
}
