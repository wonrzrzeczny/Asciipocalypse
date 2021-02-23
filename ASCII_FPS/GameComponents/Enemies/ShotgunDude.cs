using Microsoft.Xna.Framework;

namespace ASCII_FPS.GameComponents.Enemies
{
    public class ShotgunDude : Monster
    {
        public ShotgunDude(MeshObject meshObject, float health, float damage)
            : base(meshObject, health, damage) { }


        public override float HitRadius => 3f;
        protected override float AlertDistance => 70f;
        protected override float AttackDistance => 30f;
        protected override float Speed => 8f;
        protected override float ShootSpeed => 0.5f;


        protected override void Attack(Vector3 towardsTarget)
        {
            Vector3 left = Vector3.Transform(towardsTarget, Mathg.RotationMatrix(-0.25f));
            Vector3 right = Vector3.Transform(towardsTarget, Mathg.RotationMatrix(0.25f));
            Fire(towardsTarget);
            Fire(left);
            Fire(right);
        }
    }
}
