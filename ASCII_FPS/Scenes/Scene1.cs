using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ASCII_FPS.Scenes
{
    public static partial class Scenes
    {
		static List<Triangle> MakeWall(float x0, float z0, float x1, float z1, float h, AsciiTexture texture)
		{
			float ratio = (new Vector2(x0, z0) - new Vector2(x1, z1)).Length() / h;

			Triangle triangle1 = new Triangle(new Vector3(x0, h, z0), new Vector3(x1, h, z1), new Vector3(x0, -h, z0), texture,
				new Vector2(0f, 0f), new Vector2(ratio, 0f), new Vector2(0f, 1f));
			Triangle triangle2 = new Triangle(new Vector3(x0, -h, z0), new Vector3(x1, h, z1), new Vector3(x1, -h, z1), texture,
				new Vector2(0f, 1f), new Vector2(ratio, 0f), new Vector2(ratio, 1f));
			
			return new List<Triangle>(new Triangle[] { triangle1, triangle2 });
		}

		static void AddRoom(Scene scene, Vector2 center)
		{
			List<Triangle> triangles = new List<Triangle>();

			triangles.AddRange(MakeWall(-30f, -40f, 30f, -40f, 4f, ASCII_FPS.texture1));
			triangles.AddRange(MakeWall(30f, -40f, 40f, -30f, 4f, ASCII_FPS.texture1));
			triangles.AddRange(MakeWall(40f, -30f, 40f, 30f, 4f, ASCII_FPS.texture1));
			triangles.AddRange(MakeWall(40f, 30f, 30f, 40f, 4f, ASCII_FPS.texture1));
			triangles.AddRange(MakeWall(30f, 40f, -30f, 40f, 4f, ASCII_FPS.texture1));
			triangles.AddRange(MakeWall(-30f, 40f, -40f, 30f, 4f, ASCII_FPS.texture1));
			triangles.AddRange(MakeWall(-40f, 30f, -40f, -30f, 4f, ASCII_FPS.texture1));
			triangles.AddRange(MakeWall(-40f, -30f, -30f, -40f, 4f, ASCII_FPS.texture1));

			Vector3 trl = new Vector3(40f, -4f, 40f);
			Vector3 trh = new Vector3(40f, 4f, 40f);

			Vector3 tll = new Vector3(-40f, -4f, 40f);
			Vector3 tlh = new Vector3(-40f, 4f, 40f);

			Vector3 brl = new Vector3(40f, -4f, -40f);
			Vector3 brh = new Vector3(40f, 4f, -40f);

			Vector3 bll = new Vector3(-40f, -4f, -40f);
			Vector3 blh = new Vector3(-40f, 4f, -40f);

			triangles.Add(new Triangle(tll, trl, brl, ASCII_FPS.texture2, Vector2.Zero, Vector2.UnitX, Vector2.One));
			triangles.Add(new Triangle(tlh, trh, brh, ASCII_FPS.texture2, Vector2.Zero, Vector2.One, Vector2.UnitY));
			triangles.Add(new Triangle(tll, brl, bll, ASCII_FPS.texture2, Vector2.Zero, Vector2.UnitX, Vector2.One));
			triangles.Add(new Triangle(tlh, brh, blh, ASCII_FPS.texture2, Vector2.Zero, Vector2.One, Vector2.UnitY));

			scene.AddMesh(new MeshObject(triangles, new Vector3(center.X, 0f, center.Y), 0f));
			scene.AddMesh(new MeshObject(ASCII_FPS.barrelModel, ASCII_FPS.barrelTexture, new Vector3(center.X, -2f, center.Y)));
		}

		public static Scene Level1()
        {
            Scene scene = new Scene();

			AddRoom(scene, Vector2.Zero);

            return scene;
        }
    }
}
