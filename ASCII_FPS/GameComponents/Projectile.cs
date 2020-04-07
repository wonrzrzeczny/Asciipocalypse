using Microsoft.Xna.Framework;

namespace ASCII_FPS.GameComponents
{
    public class Projectile : GameObject
    {
        private readonly Scene scene;
        private readonly Vector3 direction;
        private readonly float speed;
        private readonly MeshObject meshObject;

        public Vector3 Position
        {
            get { return meshObject.Position; }
            set { meshObject.Position = value; }
        }

        public Projectile(Scene scene, Vector3 direction, float speed, MeshObject meshObject)
        {
            this.scene = scene;
            this.direction = Vector3.Normalize(direction);
            this.meshObject = meshObject;
            this.speed = speed;
        }

        public override void Update(float deltaTime)
        {
            if (scene.CheckMovement(Position, direction * speed * deltaTime, 0f))
            {
                Position += direction * speed * deltaTime;
            }
            else
            {
                Destroy = true;
                scene.RemoveDynamicMesh(meshObject);
            }
        }
    }
}
