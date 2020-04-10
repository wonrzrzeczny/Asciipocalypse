using Microsoft.Xna.Framework;

namespace ASCII_FPS.GameComponents
{
    public class EnemyProjectile : GameObject
    {
        private readonly Scene scene;
        private readonly Camera camera;
        private readonly Vector3 direction;
        private readonly float speed;
        private readonly MeshObject meshObject;
        private readonly float damage;

        private const float playerRadius = 0.65f;

        public Vector3 Position
        {
            get { return meshObject.Position; }
            set { meshObject.Position = value; }
        }


        public EnemyProjectile(Scene scene, Camera camera, MeshObject meshObject, Vector3 direction, float speed, float damage)
        {
            this.scene = scene;
            this.camera = camera;
            this.direction = Vector3.Normalize(direction);
            this.meshObject = meshObject;
            this.speed = speed;
            this.damage = damage;
        }

        public override void Update(float deltaTime)
        {
            if (Vector3.Distance(Position, camera.CameraPos) < playerRadius)
            {
                //player.DealDamage()
                Destroy = true;
                scene.RemoveDynamicMesh(meshObject);
            }

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
