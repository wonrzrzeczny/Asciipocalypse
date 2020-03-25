using Microsoft.Xna.Framework;

namespace ASCII_FPS.Scenes
{
    public static partial class Scenes
    {
		public static Scene Level1()
        {
            Scene scene = new Scene();

            scene.AddWall(-30f, -40f, 30f, -40f, 4f, ASCII_FPS.texture1);
            scene.AddWall(30f, -40f, 40f, -30f, 4f, ASCII_FPS.texture1);
            scene.AddWall(40f, -30f, 40f, 30f, 4f, ASCII_FPS.texture1);
            scene.AddWall(40f, 30f, 30f, 40f, 4f, ASCII_FPS.texture1);
            scene.AddWall(30f, 40f, -30f, 40f, 4f, ASCII_FPS.texture1);
            scene.AddWall(-30f, 40f, -40f, 30f, 4f, ASCII_FPS.texture1);
            scene.AddWall(-40f, 30f, -40f, -30f, 4f, ASCII_FPS.texture1);
            scene.AddWall(-40f, -30f, -30f, -40f, 4f, ASCII_FPS.texture1);
            
            Vector3 trl = new Vector3(40f, -4f, 40f);
            Vector3 trh = new Vector3(40f, 4f, 40f);

            Vector3 tll = new Vector3(-40f, -4f, 40f);
            Vector3 tlh = new Vector3(-40f, 4f, 40f);

            Vector3 brl = new Vector3(40f, -4f, -40f);
            Vector3 brh = new Vector3(40f, 4f, -40f);

            Vector3 bll = new Vector3(-40f, -4f, -40f);
            Vector3 blh = new Vector3(-40f, 4f, -40f);

            scene.AddTriangle(new Triangle(tll, trl, brl, ASCII_FPS.texture2, Vector2.Zero, Vector2.UnitX, Vector2.One));
            scene.AddTriangle(new Triangle(tlh, trh, brh, ASCII_FPS.texture2, Vector2.Zero, Vector2.One, Vector2.UnitY));
            scene.AddTriangle(new Triangle(tll, brl, bll, ASCII_FPS.texture2, Vector2.Zero, Vector2.UnitX, Vector2.One));
            scene.AddTriangle(new Triangle(tlh, brh, blh, ASCII_FPS.texture2, Vector2.Zero, Vector2.One, Vector2.UnitY));

			scene.AddMesh(new MeshObject(ASCII_FPS.barrelModel, ASCII_FPS.barrelTexture, new Vector3(0f, -2f, 0f)));

            return scene;
        }
    }
}
