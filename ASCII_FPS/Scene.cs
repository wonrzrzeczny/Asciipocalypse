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
    }
}