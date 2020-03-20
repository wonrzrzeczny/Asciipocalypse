using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ASCII_FPS
{
    public class Scene
    {
        public List<Triangle> triangles;

        private List<Vector2[]> walls;

        public Scene()
        {
            triangles = new List<Triangle>();
            walls = new List<Vector2[]>();
        }

        public void AddTriangle(Triangle triangle)
        {
            triangles.Add(triangle);
        }

        public void AddWall(float x0, float z0, float x1, float z1, float h, AsciiTexture texture)
        {
            float ratio = (new Vector2(x0, z0) - new Vector2(x1, z1)).Length() / h;

            triangles.Add(new Triangle(new Vector3(x0, h, z0), new Vector3(x1, h, z1), new Vector3(x0, -h, z0), texture, 
                new Vector2(0f, 0f), new Vector2(ratio, 0f), new Vector2(0f, 1f)));
            triangles.Add(new Triangle(new Vector3(x0, -h, z0), new Vector3(x1, h, z1), new Vector3(x1, -h, z1), texture,
                new Vector2(0f, 1f), new Vector2(ratio, 0f), new Vector2(ratio, 1f)));

            walls.Add(new Vector2[2] { new Vector2(x0, z0), new Vector2(x1, z1) });
        }

        public bool CheckMovement(Vector3 from, Vector3 direction)
        {
            Vector2 from2 = new Vector2(from.X, from.Z);
            Vector2 direction2 = new Vector2(direction.X, direction.Z);
            Vector2 to2 = from2 + direction2;
            foreach (Vector2[] wall in walls)
            {
                if (Mathg.Cross2D(wall[1] - wall[0], from2 - wall[0]) * Mathg.Cross2D(wall[1] - wall[0], to2 - wall[0]) < 0 &&
                    Mathg.Cross2D(to2 - from2, wall[0] - from2) * Mathg.Cross2D(to2 - from2, wall[1] - from2) < 0)
                    return false;
            }

            return true;
        }
    }
}