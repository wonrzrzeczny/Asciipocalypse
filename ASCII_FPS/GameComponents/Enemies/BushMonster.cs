using ASCII_FPS.Scenes;
using Microsoft.Xna.Framework;
using System;

namespace ASCII_FPS.GameComponents.Enemies
{
    public class BushMonster : Monster
    {
        public BushMonster(Vector3 position, float health, float damage)
            : base(new MeshObject(Assets.bushMonsterModel, Assets.bushMonsterTexture, position), health, damage) { }


        public override float HitRadius => 3f;
        protected override float AlertDistance => 40f;
        protected override float AttackDistance => 70f;
        protected override float Speed => 0f;
        protected override float ShootSpeed => 0.3f;


        public override void Update(float deltaTime)
        {
            behaviourCheckTime -= deltaTime;
            if (behaviourCheckTime < 0f)
            {
                behaviourCheckTime = 0.2f;
                float distance = Vector3.Distance(Position, Camera.CameraPos);
                
                if (distance < AttackDistance)
                {
                    bool playerVisible = Scene.CheckMovement(Position, Camera.CameraPos - Position, 0f, ObstacleLayerMask.GetMask(ObstacleLayer.Wall));

                    if (behaviourState == BehaviourState.Idle && playerVisible && distance < AlertDistance)
                    {
                        behaviourState = BehaviourState.Attacking;
                    }
                    else if (behaviourState == BehaviourState.Attacking && !playerVisible)
                    {
                        behaviourState = BehaviourState.Idle;
                    }
                }
                else
                {
                    behaviourState = BehaviourState.Idle;
                }
            }

            if (behaviourState == BehaviourState.Attacking)
            {
                if (Position.Y < -2f)
                {
                    Position += 3f * deltaTime * Vector3.Up;
                }
            }
            else
            {
                if (Position.Y > -4.25f)
                {
                    Position += 3f * deltaTime * Vector3.Down;
                }
            }

            Vector3 towardsTarget = Vector3.Normalize(Camera.CameraPos - Position - Vector3.Up * 2f);
            if (Position.Y > -3f)
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
            Assets.tsch.Play();
            Fire(towardsTarget);
        }

        protected override void Fire(Vector3 direction, float projectileSpeed = 40f)
        {
            MeshObject projectileMesh = PrimitiveMeshes.Octahedron(Position + 2f * Vector3.Up, 0.5f, Assets.projectile3Texture);
            Scene.AddGameObject(new EnemyProjectile(projectileMesh, direction, projectileSpeed, damage, true));
        }
    }
}
