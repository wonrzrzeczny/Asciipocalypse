using Microsoft.Xna.Framework;
using System.IO;

namespace ASCII_FPS.GameComponents
{
    public class Collectible : GameObject
    {
        public enum Type { Health, Armor, Skill }
        private Type type;

        private int roomX, roomY;

        public Collectible(MeshObject meshObject, Type type, int roomX, int roomY) : base(meshObject)
        {
            this.type = type;
            this.roomX = roomX;
            this.roomY = roomY;
        }

        public override void Update(float deltaTime) { }

        public void PickUp(PlayerStats playerStats)
        {
            Destroy = true;
            Game.PlayerStats.totalBarrels++;
            Achievements.UnlockLeveled("Barrel", Game.PlayerStats.totalBarrels);

            Scene.Collectibles[roomX, roomY] = null;
            switch (type)
            {
                case Type.Health:
                    playerStats.AddHealth(playerStats.maxHealth * 0.3f);
                    break;
                case Type.Armor:
                    playerStats.AddArmor(playerStats.maxArmor * 0.3f);
                    break;
                case Type.Skill:
                    playerStats.skillPoints++;
                    break;
            }
        }

        public override void Save(BinaryWriter writer)
        {
            writer.Write(typeof(Loaders.CollectibleLoader).FullName);

            MeshObject.Save(writer);
            writer.Write((int)type);
            writer.Write(roomX);
            writer.Write(roomY);
        }
    }
}
