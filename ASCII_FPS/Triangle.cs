using Microsoft.Xna.Framework;

namespace ASCII_FPS
{
    public class Triangle
    {
        public Vector3 V0 { get; set; }
        public Vector3 V1 { get; set; }
        public Vector3 V2 { get; set; }

        public Vector3 Color { get; set; }

        public Triangle(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 color)
        {
            V0 = v0;
            V1 = v1;
            V2 = v2;
            Color = color;
        }
    }
}