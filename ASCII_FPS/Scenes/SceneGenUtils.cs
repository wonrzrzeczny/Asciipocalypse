using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASCII_FPS.Scenes
{
    public static class SceneGenUtils
    {
        public static List<Triangle> MakeWall(Vector2[] points, float h, AsciiTexture texture)
        {
            List<Triangle> triangles = new List<Triangle>();
            for (int i = 0; i < points.Length - 1; i++)
            {
                float x0 = points[i].X;
                float z0 = points[i].Y;
                float x1 = points[i + 1].X;
                float z1 = points[i + 1].Y;
                float ratio = (new Vector2(x0, z0) - new Vector2(x1, z1)).Length() / h;

                Triangle triangle1 = new Triangle(new Vector3(x0, h, z0), new Vector3(x1, h, z1), new Vector3(x0, -h, z0), texture,
                    new Vector2(0f, 0f), new Vector2(ratio, 0f), new Vector2(0f, 1f));
                Triangle triangle2 = new Triangle(new Vector3(x0, -h, z0), new Vector3(x1, h, z1), new Vector3(x1, -h, z1), texture,
                    new Vector2(0f, 1f), new Vector2(ratio, 0f), new Vector2(ratio, 1f));
                triangles.Add(triangle1);
                triangles.Add(triangle2);
            }

            return triangles;
        }

        public static void AddWalls(Scene scene, Zone zone, List<Vector2[]> walls, float h, AsciiTexture texture, Vector3 offset)
        {
            Vector2 offset2D = new Vector2(offset.X, offset.Z);
            foreach (Vector2[] wall in walls)
            {
                List<Triangle> wallTriangles = MakeWall(wall, h, texture);
                MeshObject meshObject = new MeshObject(wallTriangles, offset, 0f);
                zone.AddMesh(meshObject);
                for (int i = 0; i < wall.Length - 1; i++)
                {
                    scene.AddWall(wall[i] + offset2D, wall[i + 1] + offset2D);
                }
            }
        }
    }
}
