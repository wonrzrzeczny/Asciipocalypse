using Microsoft.Xna.Framework;
using System.IO;

namespace ASCII_FPS.GameComponents
{
    public class EnemyProjectile : GameObject
    {
        private readonly Vector3 direction;
        private readonly float speed;
        private readonly float damage;


        public EnemyProjectile(MeshObject meshObject, Vector3 direction, float speed, float damage) : base(meshObject)
        {
            this.direction = Vector3.Normalize(direction);
            this.speed = speed;
            this.damage = damage;
        }

        public override void Update(float deltaTime)
        {
            if (Vector3.Distance(Position, Camera.CameraPos) < PlayerStats.thickness)
            {
                Game.PlayerStats.DealDamage(damage);
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

        public override void Save(BinaryWriter writer)
        {
            writer.Write(typeof(Loaders.EnemyProjectileLoader).FullName);

            MeshObject.Save(writer);
            GameSave.WriteVector3(writer, direction);
            writer.Write(speed);
            writer.Write(damage);
        }
    }
}
