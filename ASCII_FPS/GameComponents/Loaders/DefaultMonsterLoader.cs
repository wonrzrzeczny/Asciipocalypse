using ASCII_FPS.GameComponents.Enemies;
using Microsoft.Xna.Framework;
using System;
using System.IO;

namespace ASCII_FPS.GameComponents.Loaders
{
    public class DefaultMonsterLoader : GameObjectLoader
    {
        public override GameObject Load(BinaryReader reader)
        {
            string typeName = reader.ReadString();
            Vector3 position = GameSave.ReadVector3(reader);
            float health = reader.ReadSingle();
            float damage = reader.ReadSingle();

            Type monsterType = Type.GetType(typeName);
            return (Monster)Activator.CreateInstance(monsterType, position, health, damage);
        }
    }
}
