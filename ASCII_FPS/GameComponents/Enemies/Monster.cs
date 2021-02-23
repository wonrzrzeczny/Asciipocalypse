using ASCII_FPS.Scenes;
using Microsoft.Xna.Framework;
using System;
using System.IO;

namespace ASCII_FPS.GameComponents.Enemies
{
    public abstract class Monster : GameObject
    {
        protected float health;
        protected readonly float damage;

        private enum BehaviourState { Idle, Chasing, Attacking, Searching }
        private BehaviourState behaviourState;
        private float behaviourCheckTime = 0.2f;
        private Vector3 targetPosition;

        private float shootTime = 0f;


        public abstract float HitRadius { get; }
        protected abstract float AlertDistance { get; }
        protected abstract float AttackDistance { get; }
        protected abstract float Speed { get; }
        protected abstract float ShootSpeed { get; }


        public Monster(MeshObject meshObject, float health, float damage) : base(meshObject)
        {
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
                Position += Scene.SmoothMovement(Position, towardsTarget * deltaTime * Speed, HitRadius, ObstacleLayerMask.GetMask(ObstacleLayer.Wall));
            }
            if (behaviourState == BehaviourState.Chasing || behaviourState == BehaviourState.Attacking)
            {
                shootTime += deltaTime;
                if (shootTime > ShootSpeed && !Game.PlayerStats.dead)
                {
                    ASCII_FPS.tsch.Play();
                    shootTime = 0f;
                    Attack(towardsTarget);
                }
            }
        }

        public void DealDamage(float amount)
        {
            ASCII_FPS.oof.Play();
            health -= amount;
            if (health <= 0f)
            {
                Game.PlayerStats.monsters++;
                Game.PlayerStats.totalMonstersKilled++;
                Destroy = true;
            }

            targetPosition = Camera.CameraPos;
        }


        protected abstract void Attack(Vector3 towardsTarget);

        protected void Fire(Vector3 direction)
        {
            MeshObject projectileMesh = PrimitiveMeshes.Octahedron(Position, 0.5f, ASCII_FPS.projectileTexture);
            Scene.AddGameObject(new EnemyProjectile(projectileMesh, direction, 40f, damage));
        }


        private BehaviourState StateCheck()
        {
            float distance = Vector3.Distance(Position, Camera.CameraPos);
            if (distance < AlertDistance && Scene.CheckMovement(Position, Camera.CameraPos - Position, 0f, ObstacleLayerMask.GetMask(ObstacleLayer.Wall)))
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
