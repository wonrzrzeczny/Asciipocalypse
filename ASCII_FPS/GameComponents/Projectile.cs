using System.IO;
using Microsoft.Xna.Framework;

namespace ASCII_FPS.GameComponents
{
    public class Projectile : GameObject
    {
        private readonly Vector3 direction;
        private readonly float speed;
        private readonly float damage;

        public Projectile(MeshObject meshObject, Vector3 direction, float speed, float damage) : base(meshObject)
        {
            this.direction = Vector3.Normalize(direction);
            this.speed = speed;
            this.damage = damage;
        }

        public override void Update(float deltaTime)
        {
            // This is extremely inefficient, but let's hope that it's sufficient :p
            foreach (GameObject gameObject in Scene.gameObjects)
            {
                if (gameObject is Monster monster)
                {
                    if (Vector3.Distance(Position, monster.Position) < monster.HitRadius)
                    {
                        monster.DealDamage(damage);
                        Destroy = true;
                    }
                }
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
            writer.Write(typeof(Loaders.ProjectileLoader).FullName);

            MeshObject.Save(writer);
            GameSave.WriteVector3(writer, direction);
            writer.Write(speed);
            writer.Write(damage);
        }
    }
}
