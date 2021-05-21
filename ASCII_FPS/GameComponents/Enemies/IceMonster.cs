using Microsoft.Xna.Framework;
using System;

namespace ASCII_FPS.GameComponents.Enemies
{
    public class IceMonster : Monster
    {
        public IceMonster(Vector3 position, float health, float damage)
            : base(PrimitiveMeshes.Tetrahedron(position, 3f, Assets.iceMonsterTexture), health, damage)
        {
            rand = new Random();
        }


        public override float HitRadius => 3f;
        protected override float AlertDistance => 70f;
        protected override float AttackDistance => 30f;
        protected override float Speed => 12f;
        protected override float ShootSpeed => 0.6f;


        private readonly Random rand;

        private const int burstShots = 6;
        private const float burstDelay = 0.05f;
        private int burstCounter = 0;
        private float burstTimer = 0f;


        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (burstCounter > 0)
            {
                burstTimer -= deltaTime;
                if (burstTimer <= 0f)
                {
                    Assets.tsch.Play();

                    Vector3 direction = Vector3.Normalize(Camera.CameraPos - Position);
                    float delta = (float)rand.NextDouble() * 0.125f - 0.0625f;
                    direction = Vector3.Transform(direction, Mathg.RotationMatrix(delta));

                    Fire(direction);
                    burstCounter--;
                    if (burstCounter > 0)
                    {
                        burstTimer = burstDelay;
                    }
                    else
                    {
                        burstTimer = 0f;
                    }
                }
            }
        }


        protected override void Attack(Vector3 towardsTarget)
        {
            burstCounter = burstShots;
            burstTimer = burstDelay;
        }

        protected override void Fire(Vector3 direction, float projectileSpeed = 40f)
        {
            MeshObject projectileMesh = PrimitiveMeshes.Octahedron(Position, 0.5f, Assets.projectile4Texture);
            Scene.AddGameObject(new EnemyProjectile(projectileMesh, direction, projectileSpeed, damage));
        }
    }
}
