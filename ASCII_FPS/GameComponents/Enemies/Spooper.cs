using ASCII_FPS.Scenes;
using Microsoft.Xna.Framework;
using System;

namespace ASCII_FPS.GameComponents.Enemies
{
    public class Spooper : Monster
    {
        public Spooper(Vector3 position, float health, float damage)
            : base(new MeshObject(Assets.spooperModel, Assets.spooperTexture, position), health, damage)
        {
            random = new Random();
            velocity = Vector3.Zero;
        }


        private readonly Random random;
        private Vector3 velocity;


        public override float HitRadius => 2f;
        protected override float AlertDistance => 90f;
        protected override float AttackDistance => 5f;
        protected override float Speed => 30f;
        protected override float ShootSpeed => 0.5f;

        private const float acceleration = 60f;


        public override void Update(float deltaTime)
        {
            MeshObject.Rotation += deltaTime * (float)Math.PI * 0.3f;

            behaviourCheckTime -= deltaTime;
            if (behaviourCheckTime < 0f)
            {
                behaviourCheckTime = 0.2f;
                behaviourState = StateCheck();
            }

            if (behaviourState == BehaviourState.Chasing || behaviourState == BehaviourState.Searching)
            {
                float distance = (targetPosition - Position).Length();
                Vector3 desiredVelocity = (targetPosition - Position) / distance * Math.Min(Speed, 8f * distance);
                Vector3 correction = desiredVelocity - velocity;

                velocity += Vector3.Normalize(deltaTime * acceleration * Vector3.Normalize(correction));
            }
            
            if (velocity.Length() > 0f)
            {
                velocity = Vector3.Normalize(velocity) * Math.Min(Speed, velocity.Length());
            }
            Position += Scene.SmoothMovement(Position, velocity * deltaTime, HitRadius, ObstacleLayerMask.GetMask(ObstacleLayer.Wall));

            Vector3 towardsTarget = Vector3.Normalize(targetPosition - Position);
            if (behaviourState == BehaviourState.Chasing || behaviourState == BehaviourState.Attacking)
            {
                shootTime += deltaTime;
                if (shootTime > ShootSpeed && !Game.PlayerStats.dead)
                {
                    shootTime = 0f;
                    Attack(towardsTarget);
                }
            }
        }


        protected override void Attack(Vector3 towardsTarget)
        {
            float volume = 1f - Vector3.Distance(Position, Camera.CameraPos) / 100f;
            Assets.beep.Play(volume, (float)random.NextDouble(), 0f);
            if (Vector3.Distance(Position, Camera.CameraPos) < AttackDistance)
            {
                Game.PlayerStats.DealDamage(damage);
            }
        }
    }
}
