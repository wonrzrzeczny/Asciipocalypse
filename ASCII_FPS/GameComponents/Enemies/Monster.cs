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

        protected enum BehaviourState { Idle, Chasing, Attacking, Searching }
        protected BehaviourState behaviourState;
        protected float behaviourCheckTime = 0.2f;
        protected Vector3 targetPosition;

        protected float shootTime = 0f;


        public abstract float HitRadius { get; }
        protected abstract float AlertDistance { get; }
        protected abstract float AttackDistance { get; }
        protected abstract float Speed { get; }
        protected abstract float ShootSpeed { get; }

        public bool Invincible { get; protected set; } = false;


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
                float magnitude = Math.Min(deltaTime * Speed, PlayerStats.thickness + HitRadius);
                Position += Scene.SmoothMovement(Position, towardsTarget * magnitude, HitRadius, ObstacleLayerMask.GetMask(ObstacleLayer.Wall));
            }
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

        public void DealDamage(float amount)
        {
            if (Destroy)
            {
                return;
            }

            Assets.oof.Play();
            health -= amount;
            if (health <= 0f)
            {
                Game.PlayerStats.monsters++;
                Game.PlayerStats.totalMonstersKilled++;
                Achievements.UnlockLeveled("Monster", Game.PlayerStats.totalMonstersKilled, Game.HUD);
                if (Game.PlayerStats.monsters == (int)Math.Ceiling(Game.PlayerStats.totalMonsters / 2f))
                {
                    Game.HUD.AddNotification("Next floor has unlocked");
                }

                Destroy = true;
            }

            targetPosition = Camera.CameraPos;
        }


        protected abstract void Attack(Vector3 towardsTarget);

        protected virtual void Fire(Vector3 direction, float projectileSpeed = 40f)
        {
            MeshObject projectileMesh = PrimitiveMeshes.Octahedron(Position, 0.5f, Assets.projectileTexture);
            Scene.AddGameObject(new EnemyProjectile(projectileMesh, direction, projectileSpeed, damage));
        }


        protected BehaviourState StateCheck()
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


        public override void Save(BinaryWriter writer)
        {
            writer.Write(typeof(Loaders.DefaultMonsterLoader).FullName);
            writer.Write(GetType().FullName);

            GameSave.WriteVector3(writer, Position);
            writer.Write(health);
            writer.Write(damage);
        }
    }
}
