using System.IO;

namespace ASCII_FPS.GameComponents.Loaders
{
    public abstract class GameObjectLoader
    {
        public abstract GameObject Load(BinaryReader reader);
    }
}
