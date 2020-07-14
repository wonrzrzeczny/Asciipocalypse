using System.IO;

namespace ASCII_FPS.GameComponents.Loaders
{
    public class MonsterLoader : GameObjectLoader
    {
        public override GameObject Load(BinaryReader reader)
        {
            MeshObject meshObject = MeshObject.Load(reader);
            float health = reader.ReadSingle();
            float damage = reader.ReadSingle();
            float hitRadius = reader.ReadSingle();

            return new Monster(meshObject, hitRadius, health, damage);
        }
    }
}
