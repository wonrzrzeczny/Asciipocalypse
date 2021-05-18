using Microsoft.Xna.Framework;
using System.IO;

namespace ASCII_FPS.GameComponents.Loaders
{
    public class LavaPoolLoader : GameObjectLoader
    {
        public override GameObject Load(BinaryReader reader)
        {
            Vector2 v0 = GameSave.ReadVector2(reader);
            Vector2 v1 = GameSave.ReadVector2(reader);

            return LavaPool.Create(v0, v1);
        }
    }
}
