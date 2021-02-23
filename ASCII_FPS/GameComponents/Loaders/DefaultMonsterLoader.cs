using ASCII_FPS.GameComponents.Enemies;
using System;
using System.IO;

namespace ASCII_FPS.GameComponents.Loaders
{
    public class DefaultMonsterLoader : GameObjectLoader
    {
        public override GameObject Load(BinaryReader reader)
        {
            string typeName = reader.ReadString();
            MeshObject meshObject = MeshObject.Load(reader);
            float health = reader.ReadSingle();
            float damage = reader.ReadSingle();

            Type monsterType = Type.GetType(typeName);
            return (Monster)Activator.CreateInstance(monsterType, meshObject, health, damage);
        }
    }
}
