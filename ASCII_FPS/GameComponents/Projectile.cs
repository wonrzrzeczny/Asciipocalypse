using Microsoft.Xna.Framework;

namespace ASCII_FPS.GameComponents
{
    public class Projectile : GameObject
    {
        private readonly Scene scene;
        private readonly Vector3 direction;
        private readonly float speed;
        private readonly MeshObject meshObject;
        private readonly float damage;

        public Vector3 Position
        {
            get { return meshObject.Position; }
            set { meshObject.Position = value; }
        }

        public Projectile(Scene scene, MeshObject meshObject, Vector3 direction, float speed, float damage)
        {
            this.scene = scene;
            this.direction = Vector3.Normalize(direction);
            this.meshObject = meshObject;
            this.speed = speed;
            this.damage = damage;
        }

        public override void Update(float deltaTime)
        {
            // This is extremely inefficient, but let's hope that it's sufficient :p
            foreach (GameObject gameObject in scene.gameObjects)
            {
                if (gameObject is Monster monster)
                {
                    if (Vector3.Distance(Position, monster.Position) < monster.HitRadius)
                    {
                        monster.DealDamage(damage);
                        Destroy = true;
                        scene.RemoveDynamicMesh(meshObject);
                    }
                }
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
