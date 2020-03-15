using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ASCII_FPS
{
    public class Scene
    {
        public List<Triangle> triangles;

        public Scene()
        {
            triangles = new List<Triangle>();
        }

        public void AddTriangle(Triangle triangle)
        {
            triangles.Add(triangle);
        }

        public void AddWall(float x0, float z0, float x1, float z1, float h, Vector3 color)
        {
            float ratio = (new Vector2(x0, z0) - new Vector2(x1, z1)).Length() / h;

            triangles.Add(new Triangle(new Vector3(x0, h, z0), new Vector3(x1, h, z1), new Vector3(x0, -h, z0), color, 
                new Vector2(0f, 0f), new Vector2(ratio, 0f), new Vector2(0f, 1f)));
            triangles.Add(new Triangle(new Vector3(x0, -h, z0), new Vector3(x1, h, z1), new Vector3(x1, -h, z1), color,
                new Vector2(0f, 1f), new Vector2(ratio, 0f), new Vector2(ratio, 1f)));
        }
    }
}