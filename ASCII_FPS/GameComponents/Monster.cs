using Microsoft.Xna.Framework;
using System;

namespace ASCII_FPS.GameComponents
{
    public class Monster : GameObject
    {
        private readonly Scene scene;
        private readonly Camera camera;
        private readonly MeshObject meshObject;
        private readonly Random random;
        private float health;
        
        private enum BehaviourState { Idle, Chasing, Attacking }
        private BehaviourState behaviourState;
        private float behaviourCheckTime = 0.2f;

        private float shootTime = 0.3f;

        public Vector3 Position
        {
            get { return meshObject.Position; }
            set { meshObject.Position = value; }
        }

        public float HitRadius { get; }

        private const float alertDistance = 50f;
        private const float attackDistance = 10f;
        private const float speed = 6f;


        public Monster(Scene scene, Camera camera, MeshObject meshObject, float hitRadius, float health)
        {
            random = new Random();
            this.scene = scene;
            this.camera = camera;
            this.meshObject = meshObject;
            HitRadius = hitRadius;
            this.health = health;

            behaviourState = BehaviourState.Idle;
        }
        
        public override void Update(float deltaTime)
        {
            meshObject.Rotation += deltaTime * (float)Math.PI * 0.3f;

            behaviourCheckTime -= deltaTime;
            if (behaviourCheckTime < 0f)
            {
                behaviourCheckTime = 0.2f;
                behaviourState = StateCheck();
            }

            Vector3 towardsPlayer = Vector3.Normalize(camera.CameraPos - Position);
            if (behaviourState == BehaviourState.Chasing)
            {
                Position += scene.SmoothMovement(Position, towardsPlayer * deltaTime * speed, HitRadius);
            }
            if (behaviourState == BehaviourState.Chasing || behaviourState == BehaviourState.Attacking)
            {
                shootTime -= deltaTime;
                if (shootTime < 0f)
                {
                    shootTime = 0.3f;
                    MeshObject projectileMesh = PrimitiveMeshes.Octahedron(Position, 0.5f, ASCII_FPS.monsterTexture);
                    scene.AddDynamicMesh(projectileMesh);
                    scene.AddGameObject(new EnemyProjectile(scene, camera, projectileMesh, towardsPlayer, 25f, 2f));
                }
            }
        }

        public void DealDamage(float amount)
        {
            health -= amount;
            if (health <= 0f)
            {
                Destroy = true;
                scene.RemoveDynamicMesh(meshObject);
            }
        }



        private BehaviourState StateCheck()
        {
            float distance = Vector3.Distance(Position, camera.CameraPos);
            if (distance < alertDistance)
            {
                if (scene.CheckMovement(Position, camera.CameraPos - Position, 0f))
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
