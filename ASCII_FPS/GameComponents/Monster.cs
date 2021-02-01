using Microsoft.Xna.Framework;
using System;
using System.IO;

namespace ASCII_FPS.GameComponents
{
    public class Monster : GameObject
    {
        private readonly Random random;
        private float health;
        private readonly float damage;

        private enum BehaviourState { Idle, Chasing, Attacking, Searching }
        private BehaviourState behaviourState;
        private float behaviourCheckTime = 0.2f;
        private Vector3 targetPosition;

        private float shootTime = 0.3f;

        public float HitRadius { get; }

        public float AlertDistance { get; set; } = 70f;
        public float AttackDistance { get; set; } = 30f;
        public float Speed { get; set; } = 12f;


        public Monster(MeshObject meshObject, float hitRadius, float health, float damage) : base(meshObject)
        {
            random = new Random();
            HitRadius = hitRadius;
            this.health = health;
            this.damage = damage;

            behaviourState = BehaviourState.Idle;
            targetPosition = Position;
        }
        
        public override void Update(float deltaTime)
        {
            MeshObject.Rotation += deltaTime * (float)Math.PI * 0.3f;

            behaviourCheckTime -= deltaTime;
            if (behaviourCheckTime < 0f)
            {
                behaviourCheckTime = 0.2f;
                behaviourState = StateCheck();
            }

            Vector3 towardsTarget = Vector3.Normalize(targetPosition - Position);
            if (behaviourState == BehaviourState.Chasing || behaviourState == BehaviourState.Searching)
            {
                Position += Scene.SmoothMovement(Position, towardsTarget * deltaTime * Speed, HitRadius);
            }
            if (behaviourState == BehaviourState.Chasing || behaviourState == BehaviourState.Attacking)
            {
                shootTime -= deltaTime;
                if (shootTime < 0f && !ASCII_FPS.playerStats.dead)
                {
                    ASCII_FPS.tsch.Play();
                    shootTime = 0.3f;
                    MeshObject projectileMesh = PrimitiveMeshes.Octahedron(Position, 0.5f, ASCII_FPS.projectileTexture);
                    Scene.AddGameObject(new EnemyProjectile(projectileMesh, towardsTarget, 40f, damage));
                }
            }
        }

        public void DealDamage(float amount)
        {
            ASCII_FPS.oof.Play();
            health -= amount;
            if (health <= 0f)
            {
                ASCII_FPS.playerStats.monsters++;
                ASCII_FPS.playerStats.totalMonstersKilled++;
                Destroy = true;
            }

            targetPosition = Camera.CameraPos;
        }

        public override void Save(BinaryWriter writer)
        {
            writer.Write(typeof(Loaders.MonsterLoader).FullName);

            MeshObject.Save(writer);
            writer.Write(health);
            writer.Write(damage);
            writer.Write(HitRadius);
        }



        private BehaviourState StateCheck()
        {
            float distance = Vector3.Distance(Position, Camera.CameraPos);
            if (distance < AlertDistance && Scene.CheckMovement(Position, Camera.CameraPos - Position, 0f))
            {
                targetPosition = Camera.CameraPos;
                if (distance < AttackDistance)
                {
                    return BehaviourState.Attacking;
                }
                else
                {
                    return BehaviourState.Chasing;
                }
            }

            if (Vector3.Distance(targetPosition, Position) > HitRadius)
            {
                return BehaviourState.Searching;
            }
            return BehaviourState.Idle;
        }
    }
}
