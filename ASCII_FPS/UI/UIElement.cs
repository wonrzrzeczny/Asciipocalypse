using Microsoft.Xna.Framework.Input;

namespace ASCII_FPS.UI
{
    public abstract class UIElement
    {
        public abstract void Draw(Console console);
        public abstract void Update();

        public bool IsActive { get; set; } = true;
    }
}
