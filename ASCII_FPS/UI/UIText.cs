using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace ASCII_FPS.UI
{
    public class UIText : UIElement
    {
        private readonly byte color;
        private readonly UIPosition pivot;
        private readonly List<string> lines;
        private readonly UIAlignment alignment;


        public UIText(byte color, UIPosition pivot, UIAlignment alignment)
        {
            this.color = color;
            this.pivot = pivot;
            this.alignment = alignment;
            lines = new List<string>();
        }

        public UIText(byte color, UIPosition pivot, UIAlignment alignment, params string[] lines)
        {
            this.color = color;
            this.pivot = pivot;
            this.alignment = alignment;
            this.lines = lines.ToList();
        }

        public UIText(byte color, UIPosition pivot)
            : this(color, pivot, UIAlignment.Center) { }

        public UIText(byte color, UIPosition pivot, params string[] lines)
            : this(color, pivot, UIAlignment.Center, lines) { }

        


        public void AddLine(string line)
        {
            lines.Add(line);
        }

        public override void Draw(Console console)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                UIUtils.Text(console, pivot.GetX(console), pivot.GetY(console) + i, lines[i], color, alignment);
            }
        }

        public override void Update(KeyboardState keyboard, KeyboardState keyboardPrev)
        {
        }
    }
}
