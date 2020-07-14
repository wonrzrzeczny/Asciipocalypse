using Microsoft.Xna.Framework;
using System.IO;

namespace ASCII_FPS.GameComponents.Loaders
{
    public class EnemyProjectileLoader : GameObjectLoader
    {
        public override GameObject Load(BinaryReader reader)
        {
            MeshObject meshObject = MeshObject.Load(reader);
            Vector3 direction = GameSave.ReadVector3(reader);
            float speed = reader.ReadSingle();
            float damage = reader.ReadSingle();

            return new EnemyProjectile(meshObject, direction, speed, damage);
        }
    }
}
