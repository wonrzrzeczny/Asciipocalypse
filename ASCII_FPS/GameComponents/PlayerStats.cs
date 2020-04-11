using System;

namespace ASCII_FPS.GameComponents
{
    public struct PlayerStats
    {
        public float maxHealth;
        public float health;

        public float maxArmor;
        public float armor;
        public float armorProtection;

        public bool dead;

        public void DealDamage(float amount)
        {
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
    }
}
