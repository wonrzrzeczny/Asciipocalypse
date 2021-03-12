using Microsoft.Xna.Framework;
using System;

namespace ASCII_FPS.GameComponents.Enemies
{
    public class SpinnyBoi : Monster
    {
        public SpinnyBoi(Vector3 position, float health, float damage)
            : base(PrimitiveMeshes.TetraStar(position, 3f, ASCII_FPS.spinnyBoiTexture), health, damage) { }


        public override float HitRadius => 5f;
        protected override float AlertDistance => 80f;
        protected override float AttackDistance => 80f;
        protected override float Speed => 1f;
        protected override float ShootSpeed => 0.6f;


        private int counter = 0;


        protected override void Attack(Vector3 towardsTarget)
        {
            counter++;
            ASCII_FPS.btsch.Play();
            float dispersion = (float)Math.Atan(0.75f);
            for (int i = 0; i < 16; i++)
            {
                float delta = i * 2f * (float)Math.PI / 16f + (counter % 3 - 1) * dispersion;
                Vector3 direction = Vector3.Transform(towardsTarget, Mathg.RotationMatrix(delta));
                Fire(direction, 20f);
            }
        }

        protected override void Fire(Vector3 direction, float projectileSpeed = 40f)
        {
            MeshObject projectileMesh = PrimitiveMeshes.Octahedron(Position, 0.5f, ASCII_FPS.projectile2Texture);
            Scene.AddGameObject(new EnemyProjectile(projectileMesh, direction, projectileSpeed, damage));
        }
    }
}
