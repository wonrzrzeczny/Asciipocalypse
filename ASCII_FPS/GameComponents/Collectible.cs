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

        public void PickUp()
        {
            Destroy = true;
            switch (type)
            {
                case Type.Health:
                    ASCII_FPS.playerStats.AddHealth(ASCII_FPS.playerStats.maxHealth * 0.3f);
                    break;
                case Type.Armor:
                    ASCII_FPS.playerStats.AddArmor(ASCII_FPS.playerStats.maxArmor * 0.3f);
                    break;
                case Type.Skill:
                    ASCII_FPS.playerStats.skillPoints++;
                    break;
            }
        }
    }
}
