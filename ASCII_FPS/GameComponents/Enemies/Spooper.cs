using Microsoft.Xna.Framework;
using System;

namespace ASCII_FPS.GameComponents.Enemies
{
    public class Spooper : Monster
    {
        public Spooper(MeshObject meshObject, float health, float damage)
            : base(meshObject, health, damage) { }


        public override float HitRadius => 2f;
        protected override float AlertDistance => 90f;
        protected override float AttackDistance => 5f;
        protected override float Speed => 30f;
        protected override float ShootSpeed => 0.5f;


        protected override void Attack(Vector3 towardsTarget)
        {
            if (Vector3.Distance(Position, Camera.CameraPos) < AttackDistance)
            {
                Game.PlayerStats.DealDamage(damage);
            }
        }
    }
}
