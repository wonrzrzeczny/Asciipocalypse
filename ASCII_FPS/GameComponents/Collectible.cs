using System.IO;

namespace ASCII_FPS.GameComponents
{
    public class Collectible : GameObject
    {
        public enum Type { Health, Armor, Skill }
        public Type type;

        public Collectible(MeshObject meshObject, Type type) : base(meshObject)
        {
            this.type = type;
        }

        public override void Update(float deltaTime) { }

        public void PickUp(PlayerStats playerStats)
        {
            Destroy = true;
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
        }
    }
}
