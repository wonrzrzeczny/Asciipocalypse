using Microsoft.Xna.Framework;

namespace ASCII_FPS.GameComponents
{
    public class EnemyProjectile : GameObject
    {
        private readonly Vector3 direction;
        private readonly float speed;
        private readonly float damage;

        private const float playerRadius = 0.65f;


        public EnemyProjectile(MeshObject meshObject, Vector3 direction, float speed, float damage) : base(meshObject)
        {
            this.direction = Vector3.Normalize(direction);
            this.speed = speed;
            this.damage = damage;
        }

        public override void Update(float deltaTime)
        {
            if (Vector3.Distance(Position, Camera.CameraPos) < playerRadius)
            {
                ASCII_FPS.playerStats.DealDamage(damage);
                Destroy = true;
            }

            if (Scene.CheckMovement(Position, direction * speed * deltaTime, 0f))
            {
                Position += direction * speed * deltaTime;
            }
            else
            {
                Destroy = true;
            }
        }
    }
}
