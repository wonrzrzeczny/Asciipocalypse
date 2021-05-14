using Microsoft.Xna.Framework;

namespace ASCII_FPS.GameComponents.Enemies
{
    public class ShotgunDude : Monster
    {
        public ShotgunDude(Vector3 position, float health, float damage)
            : base(PrimitiveMeshes.Octahedron(position, 3f, Assets.shotgunDudeTexture), health, damage) { }


        public override float HitRadius => 3f;
        protected override float AlertDistance => 70f;
        protected override float AttackDistance => 30f;
        protected override float Speed => 8f;
        protected override float ShootSpeed => 0.5f;


        protected override void Attack(Vector3 towardsTarget)
        {
            Assets.tsch.Play();
            Vector3 orth = Vector3.Cross(towardsTarget, Vector3.Up);
            Vector3 left = towardsTarget - orth * 0.375f;
            Vector3 right = towardsTarget + orth * 0.375f;
            Fire(towardsTarget);
            Fire(left);
            Fire(right);
        }
    }
}
