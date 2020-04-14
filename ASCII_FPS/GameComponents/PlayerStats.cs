using Microsoft.Xna.Framework;
using System;

namespace ASCII_FPS.GameComponents
{
    public struct PlayerStats
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
        public Vector2 exitPosition;

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
