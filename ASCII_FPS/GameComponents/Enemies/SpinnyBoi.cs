using Microsoft.Xna.Framework;
using System;

namespace ASCII_FPS.GameComponents.Enemies
{
    public class SpinnyBoi : Monster
    {
        public SpinnyBoi(MeshObject meshObject, float health, float damage)
            : base(meshObject, health, damage) { }


        public override float HitRadius => 5f;
        protected override float AlertDistance => 60f;
        protected override float AttackDistance => 40f;
        protected override float Speed => 1f;
        protected override float ShootSpeed => 0.6f;


        protected override void Attack(Vector3 towardsTarget)
        {
            for (int i = 0; i < 16; i++)
            {
                float delta = i * 2f * (float)Math.PI / 16f + MeshObject.Rotation;
                Vector3 direction = Vector3.Transform(towardsTarget, Mathg.RotationMatrix(delta));
                Fire(direction, 20f);
            }
        }
    }
}
