using ASCII_FPS.Scenes;
using Microsoft.Xna.Framework;
using System.IO;

namespace ASCII_FPS.GameComponents
{
    public class EnemyProjectile : GameObject
    {
        private readonly Vector3 direction;
        private readonly float speed;
        private readonly float damage;
        private readonly bool poison;


        public EnemyProjectile(MeshObject meshObject, Vector3 direction, float speed, float damage, bool poison) : base(meshObject)
        {
            this.direction = Vector3.Normalize(direction);
            this.speed = speed;
            this.damage = damage;
            this.poison = poison;
        }

        public EnemyProjectile(MeshObject meshObject, Vector3 direction, float speed, float damage)
            : this(meshObject, direction, speed, damage, false) { }

        public override void Update(float deltaTime)
        {
            if (Vector3.Distance(Position, Camera.CameraPos) < PlayerStats.thickness)
            {
                Game.PlayerStats.DealDamage(damage);
                if (poison)
                {
                    Game.PlayerStats.Poison(damage);
                }
                Destroy = true;
            }

            if (Scene.CheckMovement(Position, direction * speed * deltaTime, 0f, ObstacleLayerMask.GetMask(ObstacleLayer.Wall)))
            {
                Position += direction * speed * deltaTime;
            }
            else
            {
                Destroy = true;
            }
        }

        public override void Save(BinaryWriter writer)
        {
            writer.Write(typeof(Loaders.EnemyProjectileLoader).FullName);

            MeshObject.Save(writer);
            GameSave.WriteVector3(writer, direction);
            writer.Write(speed);
            writer.Write(damage);
            writer.Write(poison);
        }
    }
}
