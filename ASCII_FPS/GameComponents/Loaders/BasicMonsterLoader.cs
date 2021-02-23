using ASCII_FPS.GameComponents.Enemies;
using System.IO;

namespace ASCII_FPS.GameComponents.Loaders
{
    public class BasicMonsterLoader : GameObjectLoader
    {
        public override GameObject Load(BinaryReader reader)
        {
            MeshObject meshObject = MeshObject.Load(reader);
            float health = reader.ReadSingle();
            float damage = reader.ReadSingle();

            return new BasicMonster(meshObject, health, damage);
        }
    }
}
