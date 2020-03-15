using Microsoft.Xna.Framework;

namespace ASCII_FPS
{
    public class Triangle
    {
        public Vector3 V0 { get; set; }
        public Vector3 V1 { get; set; }
        public Vector3 V2 { get; set; }

        public Vector2 UV0 { get; set; }
        public Vector2 UV1 { get; set; }
        public Vector2 UV2 { get; set; }

        public Vector3 Color { get; set; }

        public Triangle(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 color, Vector2 uv0, Vector2 uv1, Vector2 uv2)
        {
            V0 = v0;
            V1 = v1;
            V2 = v2;
            UV0 = uv0;
            UV1 = uv1;
            UV2 = uv2;
            Color = color;
        }

        public Triangle(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 color) : this(v0, v1, v2, color, Vector2.Zero, Vector2.Zero, Vector2.Zero) { }
    }
}