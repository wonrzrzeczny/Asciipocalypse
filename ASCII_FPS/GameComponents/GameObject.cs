using Microsoft.Xna.Framework;

namespace ASCII_FPS.GameComponents
{
    public abstract class GameObject
    {
        public MeshObject MeshObject { get; }
        public Scene Scene { get; set; }

        public bool Destroy { get; protected set; }

        public Vector3 Position
        {
            get { return MeshObject.Position; }
            protected set { MeshObject.Position = value; }
        }

        public Camera Camera
        {
            get { return Scene.Camera; }
        }

        public GameObject(MeshObject meshObject)
        {
            MeshObject = meshObject;
        }

        public abstract void Update(float deltaTime);
    }
}
