namespace Sobia.CrackingEggs
{
    [System.Serializable]
    public class PlayerModel
    {
        public PlayerStats CrackDamage = new PlayerStats("Crack Damage", 1.0f);
        public PlayerStats CrackSpeed = new PlayerStats("Crack Speed", 1.0f);
        public PlayerStats CrackRadius = new PlayerStats("Crack Radius", 1.0f);

        public void AddDamage(float amount)
        {
            CrackDamage.Bonus += amount;
        }

        public void AddSpeed(float amount)
        {
            CrackSpeed.Bonus += amount;
        }

        public void AddRadius(float amount)
        {
            CrackRadius.Bonus += amount;
        }

        public void LevelUpDamage()
        {
            CrackDamage.Level += 1;
        }

        public void LevelUpSpeed()
        {
            CrackSpeed.Level += 1;
        }

        public void LevelUpRadius()
        {
            CrackRadius.Level += 1;
        }
    }
}