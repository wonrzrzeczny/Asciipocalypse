namespace ASCII_FPS.GameComponents
{
    public struct PlayerStats
    {
        public float maxHealth;
        public float health;

        public void DealDamage(float amount)
        {
            health -= amount;
        }
    }
}
