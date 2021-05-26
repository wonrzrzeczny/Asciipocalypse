using Microsoft.Xna.Framework;
using System;
using System.IO;

namespace ASCII_FPS.GameComponents
{
    public class PlayerStats
    {
        public const float thickness = 1f;

        public int difficulty;

        public float maxHealth;
        public float health;
        public float tempHealth;

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
        public bool onFire;
        public float hitTime;

        public float shootTime;

        public int floor;
        public int totalMonsters;
        public int monsters;
        public int totalMonstersKilled;
        public bool fullClear = true;
        public int totalBarrels;
        public Vector2 exitPosition;

        public int seed;

        public void Save(BinaryWriter writer)
        {
            writer.Write(difficulty);

            writer.Write(maxHealth);
            writer.Write(health);
            writer.Write(tempHealth);

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
            writer.Write(fullClear);
            writer.Write(totalBarrels);
            GameSave.WriteVector2(writer, exitPosition);

            writer.Write(seed);
        }

        public void Load(BinaryReader reader)
        {
            difficulty = reader.ReadInt32();

            maxHealth = reader.ReadSingle();
            health = reader.ReadSingle();
            tempHealth = reader.ReadSingle();

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
            fullClear = reader.ReadBoolean();
            totalBarrels = reader.ReadInt32();

            exitPosition = GameSave.ReadVector2(reader);

            seed = reader.ReadInt32();
        }


        public void DealDamage(float amount, bool effects = true)
        {
            amount *= MathF.Pow(2f, difficulty);

            if (effects)
            {
                Assets.ouch.Play();
                hit = true;
                hitTime = 0.1f;
            }

            float blocked = Math.Min(amount, armor);
            float unblocked = amount - blocked;

            armor -= blocked;
            float realDamage = unblocked + blocked * (1f - armorProtection);

            if (tempHealth >= realDamage)
            {
                tempHealth -= realDamage;
                return;
            }

            tempHealth = 0f;
            realDamage -= tempHealth;
            health -= realDamage;

            if (health <= 0f)
            {
                health = 0f;
                dead = true;
            }
        }

        public void Poison(float amount)
        {
            amount *= MathF.Pow(2f, difficulty);
            amount = Math.Min(amount, health - 1);
            health -= amount;
            tempHealth += amount;
        }

        public void AddHealth(float amount)
        {
            if (!dead)
            {
                health += amount;
                if (health > maxHealth)
                {
                    health = maxHealth;
                }
                if (health + tempHealth > maxHealth)
                {
                    tempHealth = maxHealth - health;
                }
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
