using System.IO;

namespace ASCII_FPS.GameComponents.Loaders
{
    public class CollectibleLoader : GameObjectLoader
    {
        public override GameObject Load(BinaryReader reader)
        {
            MeshObject meshObject = MeshObject.Load(reader);
            Collectible.Type type = (Collectible.Type)reader.ReadInt32();
            int roomX = reader.ReadInt32();
            int roomY = reader.ReadInt32();

            return new Collectible(meshObject, type, roomX, roomY);
        }
    }
}
