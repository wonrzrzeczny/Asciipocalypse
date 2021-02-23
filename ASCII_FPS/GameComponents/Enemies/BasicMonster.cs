using Microsoft.Xna.Framework;

namespace ASCII_FPS.GameComponents.Enemies
{
    public class BasicMonster : Monster
    {
        public BasicMonster(MeshObject meshObject, float health, float damage)
            : base(meshObject, health, damage) { }


        public override float HitRadius => 3f;
        protected override float AlertDistance => 70f;
        protected override float AttackDistance => 30f;
        protected override float Speed => 12f;
        protected override float ShootSpeed => 0.3f;


        protected override void Attack(Vector3 towardsTarget)
        {
            Fire(towardsTarget);
        }
    }
}
