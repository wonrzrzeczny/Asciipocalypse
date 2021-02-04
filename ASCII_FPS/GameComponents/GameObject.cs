using ASCII_FPS.GameComponents.Loaders;
using ASCII_FPS.Scenes;
using Microsoft.Xna.Framework;
using System.IO;

namespace ASCII_FPS.GameComponents
{
    public abstract class GameObject
    {
        public MeshObject MeshObject { get; }
        public ASCII_FPS Game { get; set; }

        public bool Destroy { get; protected set; }

        public Vector3 Position
        {
            get { return MeshObject.Position; }
            protected set { MeshObject.Position = value; }
        }

        public Scene Scene
        {
            get { return Game.Scene; }
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



        public abstract void Save(BinaryWriter writer);
        
        public static GameObject Load(BinaryReader reader)
        {
            string typeName = reader.ReadString();
            System.Type type = System.Type.GetType(typeName);
            GameObjectLoader loader = (GameObjectLoader)System.Activator.CreateInstance(type);
            return loader.Load(reader);
        }
    }
}
