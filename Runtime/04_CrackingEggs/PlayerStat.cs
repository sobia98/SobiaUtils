namespace Sobia.CrackingEggs
{
    [System.Serializable]
    public class PlayerStats
    {
        public float BaseValue;
        public string StatName;
        public float Multiplier = 1.0f;
        public float Bonus = 0f;
        public int Level = 0;

        // This is now a Property. It calculates 'live' every time you access it.
        public float CurrentValue
        {
            get
            {
                return BaseValue + (Level * Multiplier) + Bonus;
            }
        }

        public PlayerStats(string statName, float baseValue)
        {
            StatName = statName;
            BaseValue = baseValue;
        }
    }
}