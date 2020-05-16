using Microsoft.Xna.Framework;
using System;

namespace ASCII_FPS.GameComponents
{
    public class Monster : GameObject
    {
        private readonly Random random;
        private float health;
        private readonly float damage;

        private enum BehaviourState { Idle, Chasing, Attacking }
        private BehaviourState behaviourState;
        private float behaviourCheckTime = 0.2f;

        private float shootTime = 0.3f;

        public float HitRadius { get; }

        private const float alertDistance = 70f;
        private const float attackDistance = 30f;
        private const float speed = 6f;


        public Monster(MeshObject meshObject, float hitRadius, float health, float damage) : base(meshObject)
        {
            random = new Random();
            HitRadius = hitRadius;
            this.health = health;
            this.damage = damage;

            behaviourState = BehaviourState.Idle;
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

            Vector3 towardsPlayer = Vector3.Normalize(Camera.CameraPos - Position);
            if (behaviourState == BehaviourState.Chasing)
            {
                Position += Scene.SmoothMovement(Position, towardsPlayer * deltaTime * speed, HitRadius);
            }
            if (behaviourState == BehaviourState.Chasing || behaviourState == BehaviourState.Attacking)
            {
                shootTime -= deltaTime;
                if (shootTime < 0f && !ASCII_FPS.playerStats.dead)
                {
                    ASCII_FPS.tsch.Play();
                    shootTime = 0.3f;
                    MeshObject projectileMesh = PrimitiveMeshes.Octahedron(Position, 0.5f, ASCII_FPS.projectileTexture);
                    Scene.AddGameObject(new EnemyProjectile(projectileMesh, towardsPlayer, 40f, damage));
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
        }



        private BehaviourState StateCheck()
        {
            float distance = Vector3.Distance(Position, Camera.CameraPos);
            if (distance < alertDistance)
            {
                if (Scene.CheckMovement(Position, Camera.CameraPos - Position, 0f))
                {
                    if (distance < attackDistance)
                    {
                        return BehaviourState.Attacking;
                    }
                    else
                    {
                        return BehaviourState.Chasing;
                    }
                }
                return BehaviourState.Idle;
            }
            return BehaviourState.Idle;
        }
    }
}
