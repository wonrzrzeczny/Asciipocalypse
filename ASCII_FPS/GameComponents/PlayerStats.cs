using Microsoft.Xna.Framework;
using System;
using System.IO;

namespace ASCII_FPS.GameComponents
{
    public class PlayerStats
    {
        public const float thickness = 1f;

        public float maxHealth;
        public float health;

        public float maxArmor;
        public float armor;
        public float armorProtection;

        public int skillPoints;
        public int skillMaxHealth;
        public int skillMaxArmor;
        public int skillArmorProtection;
        public int skillShootingSpeed;

        public bool dead;
        public bool hit;
        public float hitTime;

        public float shootTime;

        public int floor;
        public int totalMonsters;
        public int monsters;
        public int totalMonstersKilled;
        public Vector2 exitPosition;

        public void Save(BinaryWriter writer)
        {
            writer.Write(maxHealth);
            writer.Write(health);

            writer.Write(maxArmor);
            writer.Write(armor);
            writer.Write(armorProtection);

            writer.Write(skillPoints);
            writer.Write(skillMaxHealth);
            writer.Write(skillMaxArmor);
            writer.Write(skillArmorProtection);
            writer.Write(skillShootingSpeed);

            writer.Write(floor);
            writer.Write(totalMonsters);
            writer.Write(monsters);
            writer.Write(totalMonstersKilled);
            GameSave.WriteVector2(writer, exitPosition);
        }

        public void Load(BinaryReader reader)
        {
            maxHealth = reader.ReadSingle();
            health = reader.ReadSingle();

            maxArmor = reader.ReadSingle();
            armor = reader.ReadSingle();
            armorProtection = reader.ReadSingle();

            skillPoints = reader.ReadInt32();
            skillMaxHealth = reader.ReadInt32();
            skillMaxArmor = reader.ReadInt32();
            skillArmorProtection = reader.ReadInt32();
            skillShootingSpeed = reader.ReadInt32();

            floor = reader.ReadInt32();
            totalMonsters = reader.ReadInt32();
            monsters = reader.ReadInt32();
            totalMonstersKilled = reader.ReadInt32();
            exitPosition = GameSave.ReadVector2(reader);

        }


        public void DealDamage(float amount)
        {
            ASCII_FPS.ouch.Play();

            hit = true;
            hitTime = 0.1f;
            float blocked = Math.Min(amount, armor);
            float unblocked = amount - blocked;

            armor -= blocked;
            health -= unblocked + blocked * (1f - armorProtection);

            if (health <= 0f)
            {
                health = 0f;
                dead = true;
            }
        }

        public void AddHealth(float amount)
        {
            if (!dead)
            {
                health += amount;
                if (health > maxHealth)
                    health = maxHealth;
            }
        }

        public void AddArmor(float amount)
        {
            if (!dead)
            {
                armor += amount;
                if (armor > maxArmor)
                    armor = maxArmor;
            }
        }
    }
}
