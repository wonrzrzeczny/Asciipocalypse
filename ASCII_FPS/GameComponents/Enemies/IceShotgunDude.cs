using Microsoft.Xna.Framework;

namespace ASCII_FPS.GameComponents.Enemies
{
    public class IceShotgunDude : Monster
    {
        public IceShotgunDude(Vector3 position, float health, float damage)
            : base(PrimitiveMeshes.Octahedron(position, 3f, Assets.iceShotgunDudeTexture), health, damage) { }


        public override float HitRadius => 3f;
        protected override float AlertDistance => 70f;
        protected override float AttackDistance => 30f;
        protected override float Speed => 8f;
        protected override float ShootSpeed => 0.75f;


        protected override void Attack(Vector3 towardsTarget)
        {
            Assets.pew.Play(1f, 0f, 0f);
            Assets.pew.Play(1f, -0.5f, 0f);
            Assets.pew.Play(1f, -1f, 0f);

            for (int i = -5; i <= 5; i++)
            {
                float delta = i * 0.4f / 5;
                Vector3 direction = Vector3.Transform(towardsTarget, Mathg.RotationMatrix(delta));
                Fire(direction);
            }
        }

        protected override void Fire(Vector3 direction, float projectileSpeed = 40f)
        {
            MeshObject projectileMesh = PrimitiveMeshes.Octahedron(Position, 0.5f, Assets.projectile4Texture);
            Scene.AddGameObject(new EnemyProjectile(projectileMesh, direction, projectileSpeed, damage));
        }
    }
}
