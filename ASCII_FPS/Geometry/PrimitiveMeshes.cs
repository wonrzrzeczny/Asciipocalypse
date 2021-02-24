using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCII_FPS
{
    public static class PrimitiveMeshes
    {
        public static MeshObject Octahedron(Vector3 position, float radius, AsciiTexture texture)
        {
            List<Triangle> triangles = new List<Triangle>();

            Vector3 f = radius * Vector3.Forward;
            Vector3 b = radius * Vector3.Backward;
            Vector3 l = radius * Vector3.Left;
            Vector3 r = radius * Vector3.Right;
            Vector3 u = radius * Vector3.Up;
            Vector3 d = radius * Vector3.Down;

            triangles.Add(new Triangle(r, f, u, texture, new Vector2(1f, 0.5f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0.5f)));
            triangles.Add(new Triangle(b, r, u, texture, new Vector2(0.5f, 1f), new Vector2(1f, 0.5f),  new Vector2(0.5f, 0.5f)));
            triangles.Add(new Triangle(l, b, u, texture, new Vector2(0f, 0.5f), new Vector2(0.5f, 1f),  new Vector2(0.5f, 0.5f)));
            triangles.Add(new Triangle(f, l, u, texture, new Vector2(0.5f, 0f), new Vector2(0f, 0.5f), new Vector2(0.5f, 0.5f)));

            triangles.Add(new Triangle(f, r, d, texture, new Vector2(0.5f, 0f), new Vector2(1f, 0.5f), new Vector2(0.5f, 0.5f)));
            triangles.Add(new Triangle(r, b, d, texture, new Vector2(1f, 0.5f), new Vector2(0.5f, 1f), new Vector2(0.5f, 0.5f)));
            triangles.Add(new Triangle(b, l, d, texture, new Vector2(0.5f, 1f), new Vector2(0f, 0.5f), new Vector2(0.5f, 0.5f)));
            triangles.Add(new Triangle(l, f, d, texture, new Vector2(0f, 0.5f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0.5f)));

            return new MeshObject(triangles, position, 0f);
        }

        public static MeshObject Tetrahedron(Vector3 position, float radius, AsciiTexture texture)
        {
            List<Triangle> triangles = new List<Triangle>();

            float sqrt2 = (float)Math.Sqrt(2);
            float sqrt3 = (float)Math.Sqrt(3);

            Vector3 up = radius * Vector3.Up;
            Vector3 v0 = radius * new Vector3(0f, -1f / 3f, 2f * sqrt2 / 3f);
            Vector3 v1 = radius * new Vector3(-sqrt2 / sqrt3, -1f / 3f, 1 - sqrt2);
            Vector3 v2 = radius * new Vector3(sqrt2 / sqrt3, -1f / 3f, 1 - sqrt2);

            Vector2 uv0 = new Vector2(0.5f, 0f);
            Vector2 uv1 = new Vector2(0.25f, 0.25f * sqrt3);
            Vector2 uv2 = new Vector2(0.75f, 0.25f * sqrt3);
            Vector2 uvup0 = new Vector2(0.5f, 0.5f * sqrt3);
            Vector2 uvup1 = new Vector2(1f, 0f);
            Vector2 uvup2 = new Vector2(0f, 0f);

            triangles.Add(new Triangle(v0, v1, v2, texture, uv0, uv1, uv2));
            triangles.Add(new Triangle(up, v0, v2, texture, uvup1, uv0, uv2));
            triangles.Add(new Triangle(up, v1, v0, texture, uvup2, uv1, uv0));
            triangles.Add(new Triangle(up, v2, v1, texture, uvup0, uv2, uv1));

            return new MeshObject(triangles, position, 0f);
        }

        public static MeshObject TetraStar(Vector3 position, float radius, AsciiTexture texture)
        {
            List<Triangle> triangles = new List<Triangle>();

            float sqrt2 = (float)Math.Sqrt(2);
            float sqrt3 = (float)Math.Sqrt(3);

            Vector3 up = radius * Vector3.Up;
            Vector3 v0 = radius * new Vector3(0f, -1f / 3f, 2f * sqrt2 / 3f);
            Vector3 v1 = radius * new Vector3(-sqrt2 / sqrt3, -1f / 3f, 1 - sqrt2);
            Vector3 v2 = radius * new Vector3(sqrt2 / sqrt3, -1f / 3f, 1 - sqrt2);

            Vector2 uv0 = new Vector2(0.5f, 0f);
            Vector2 uv1 = new Vector2(0.25f, 0.25f * sqrt3);
            Vector2 uv2 = new Vector2(0.75f, 0.25f * sqrt3);
            Vector2 uvup0 = new Vector2(0.5f, 0.5f * sqrt3);
            Vector2 uvup1 = new Vector2(1f, 0f);
            Vector2 uvup2 = new Vector2(0f, 0f);

            triangles.Add(new Triangle(v0, v1, v2, texture, uv0, uv1, uv2));
            triangles.Add(new Triangle(up, v0, v2, texture, uvup1, uv0, uv2));
            triangles.Add(new Triangle(up, v1, v0, texture, uvup2, uv1, uv0));
            triangles.Add(new Triangle(up, v2, v1, texture, uvup0, uv2, uv1));

            up = -up;
            v0 = -v0;
            Vector3 tmp = -v1;
            v1 = -v2;
            v2 = tmp;
            triangles.Add(new Triangle(v0, v1, v2, texture, uv0, uv1, uv2));
            triangles.Add(new Triangle(up, v0, v2, texture, uvup1, uv0, uv2));
            triangles.Add(new Triangle(up, v1, v0, texture, uvup2, uv1, uv0));
            triangles.Add(new Triangle(up, v2, v1, texture, uvup0, uv2, uv1));

            return new MeshObject(triangles, position, 0f);
        }
    }
}
