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

			triangles.Add(new Triangle(f, r, u, texture, new Vector2(0.5f, 0f), new Vector2(1f, 0.5f), new Vector2(0.5f, 0.5f)));
			triangles.Add(new Triangle(r, b, u, texture, new Vector2(1f, 0.5f), new Vector2(0.5f, 1f), new Vector2(0.5f, 0.5f)));
			triangles.Add(new Triangle(b, l, u, texture, new Vector2(0.5f, 1f), new Vector2(0f, 0.5f), new Vector2(0.5f, 0.5f)));
			triangles.Add(new Triangle(l, f, u, texture, new Vector2(0f, 0.5f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0.5f)));

			triangles.Add(new Triangle(r, f, d, texture, new Vector2(0.5f, 0f), new Vector2(1f, 0.5f), new Vector2(0.5f, 0.5f)));
			triangles.Add(new Triangle(b, r, d, texture, new Vector2(1f, 0.5f), new Vector2(0.5f, 1f), new Vector2(0.5f, 0.5f)));
			triangles.Add(new Triangle(l, b, d, texture, new Vector2(0.5f, 1f), new Vector2(0f, 0.5f), new Vector2(0.5f, 0.5f)));
			triangles.Add(new Triangle(f, l, d, texture, new Vector2(0f, 0.5f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0.5f)));

			return new MeshObject(triangles, position, 0f);
		}
	}
}
