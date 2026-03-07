using UnityEngine;

namespace Sobia.SigmaboyProject
{
    [System.Serializable]
    public class UpgradeStat
    {
        public enum CalculationMode
        { Linear, Exponential, Decay, Incremental, Decremental, SpawnAmount, HeartReturn, Diversity, Probability }

        public int _level = 1;
        public float _costMultiplier = 1.35f;
        public bool _isMax = false;
        public bool _isMaxActive = false;

        public string _statName;
        public float _baseValue;
        public float _perLevelStep; // The "Power" of the upgrade
        public float _maxValue;
        public int _baseCost;
        public CalculationMode _calcMode;
        private readonly float[] upgradeCostsRadius = { 1, 8, 15, 95 };
        private readonly float[] upgradeCostsSpawnAmount = { 4, 120, 650, 1400, 5000, 15000 };
        private readonly float[] upgradeCostsHeartReturn = { 4, 32, 280, 3500 };
        private readonly float[] upgradeCostsSpawnSize = { 12, 200 };
        private readonly float[] upgradeCostsDiversity = { 50, 400, 1200, 30000 };
        private readonly float[] upgradeCostsProbability = { 800, 70000 };

        // The Constructor
        public UpgradeStat(string statName, float baseValue, float perLevelStep, float maxValue, int baseCost, CalculationMode calcMode = CalculationMode.Linear)
        {
            _statName = statName;
            _baseValue = baseValue;
            _perLevelStep = perLevelStep;
            _maxValue = maxValue;
            _baseCost = baseCost;
            _calcMode = calcMode;
        }

        // THE GETTER: Calculates current value based on the level and mode
        public float GetValue()
        {
            //spawn
            float a = 884.6368f;
            float b = 32.45646f;
            float c = 7.851904f;
            float d = 2.607406f;
            //heart
            float e = 30.95489f;
            float f = 0.9005389f;
            float g = 5.071182f;
            float h = 3.515504f;
            //diversity
            float m = 17281.97f;
            float n = 0.1467759f;
            float p = 5.943557f;
            float q = 26.81245f;
            //probability
            float r = 725502.2f;
            float s = 0.2228938f;
            float t = 7.16824f;
            float u = 14.36862f;

            switch (_calcMode)
            {
                case CalculationMode.Linear:
                    // Formula: 10 + (Level 5 * 2) = 20
                    return _isMax ? _maxValue : _baseValue + (_level * _perLevelStep);

                case CalculationMode.Exponential:
                    // Formula: 10 * (1.1 ^ Level 5) = 16.1
                    return _isMax ? _maxValue : _baseValue * Mathf.Pow(_perLevelStep, _level);

                case CalculationMode.Incremental:
                    // Formula: 10 + (Level 5 * 2) = 20
                    return _isMax ? _maxValue : _baseValue + (_level * _perLevelStep);

                case CalculationMode.Decremental:
                    // Formula: 10 - (Level 5 * 1) = 5
                    return _isMax ? _maxValue : _baseValue - (_level * _perLevelStep);

                case CalculationMode.SpawnAmount:
                    return a + (b - a) / (1f + Mathf.Pow(_level / c, d));

                case CalculationMode.HeartReturn:
                    return e + (f - e) / (1f + Mathf.Pow(_level / g, h));

                case CalculationMode.Diversity:
                    return m + (n - m) / (1f + Mathf.Pow(_level / p, q));

                case CalculationMode.Probability:
                    return r + (s - r) / (1f + Mathf.Pow(_level / t, u));

                default:
                    return _baseValue;
            }
        }

        public int GetUpgradeCost()
        {
            return Mathf.RoundToInt(_baseCost * Mathf.Pow(_costMultiplier, _level - 1));
        }

        public void Upgrade()
        {
            _level++;
        }

        public float GetUpgradeCostRadius()
        {
            int index = DataManager.Instance.radius._level - 1;
            if (index < upgradeCostsRadius.Length) //maxlevel
            {
                return upgradeCostsRadius[index];
            }
            return -1; // Or handle max level
        }

        public float GetUpgradeCostSpawnAmount()
        {
            int index = DataManager.Instance.spawn._level - 1;
            if (index < upgradeCostsSpawnAmount.Length) //maxlevel
            {
                return upgradeCostsSpawnAmount[index];
            }
            return -1; // Or handle max level
        }

        public float GetUpgradeCostHeartReturn()
        {
            int index = DataManager.Instance.heartReturn._level - 1;
            if (index < upgradeCostsHeartReturn.Length) //maxlevel
            {
                return upgradeCostsHeartReturn[index];
            }
            return -1; // Or handle max level
        }

        public float GetUpgradeCostSpawnSize()
        {
            int index = DataManager.Instance.spawnAreaSizeX._level - 1;
            if (index < upgradeCostsSpawnSize.Length) //maxlevel
            {
                return upgradeCostsSpawnSize[index];
            }
            return -1; // Or handle max level
        }

        public float GetUpgradeCostDiversity()
        {
            int index = DataManager.Instance.diversity._level - 1;
            if (index < upgradeCostsDiversity.Length) //maxlevel
            {
                return upgradeCostsDiversity[index];
            }
            return -1; // Or handle max level
        }

        public float GetUpgradeCostProbability()
        {
            int index = DataManager.Instance.probability._level - 1;
            if (index < upgradeCostsProbability.Length) //maxlevel
            {
                return upgradeCostsProbability[index];
            }
            return -1; // Or handle max level
        }
    }

    public class DataManager : MonoBehaviour
    {
        public static DataManager Instance;

        [Header("Currency")]
        public int totalHearts = 1000000;

        public UpgradeStat radius = new UpgradeStat(
            statName: "Radius",
            baseValue: 3f,
            perLevelStep: 1f,
            maxValue: 999f,
            baseCost: 1,
            calcMode: UpgradeStat.CalculationMode.Incremental);

        public UpgradeStat spawn = new UpgradeStat(
            statName: "Spawn Amount",
            baseValue: 30f,
            perLevelStep: 10f,
            maxValue: 120f,
            baseCost: 1,
            calcMode: UpgradeStat.CalculationMode.SpawnAmount);

        public UpgradeStat probability = new UpgradeStat(
            statName: "Probability",
            baseValue: 0.05f,
            perLevelStep: 1.5f,
            maxValue: 0.5f,
            baseCost: 1,
            calcMode: UpgradeStat.CalculationMode.Exponential);

        public UpgradeStat diversity = new UpgradeStat(
            statName: "Diversity",
            baseValue: 0.05f,
            perLevelStep: 1.5f,
            maxValue: 1f,
            baseCost: 1,
            calcMode: UpgradeStat.CalculationMode.Diversity);

        public UpgradeStat girlMovementSpeed = new UpgradeStat(
            statName: "Girl Movement Speed",
            baseValue: 1f,
            perLevelStep: 0.9f,
            maxValue: 1f,
            baseCost: 1,
            calcMode: UpgradeStat.CalculationMode.Exponential);

        public UpgradeStat heartReturn = new UpgradeStat(
            statName: "Heart Return",
            baseValue: 0f,
            perLevelStep: 1f,
            maxValue: 10f,
            baseCost: 1,
            calcMode: UpgradeStat.CalculationMode.HeartReturn);

        public UpgradeStat spawnAreaSizeX = new UpgradeStat(
            statName: "Spawn Area Size X",
            baseValue: 20f,
            perLevelStep: 1f,
            maxValue: 15f,
            baseCost: 1,
            calcMode: UpgradeStat.CalculationMode.Decremental);

        public UpgradeStat spawnAreaSizeY = new UpgradeStat(
            statName: "Spawn Area Size Y",
            baseValue: 10f,
            perLevelStep: 1f,
            maxValue: 5f,
            baseCost: 1,
            calcMode: UpgradeStat.CalculationMode.Decremental);

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }

        public float[] GetSpawnWeights()
        {
            float uglyWeight = 100f;
            float averageWeight = 0f;
            float baddieWeight = 0f;

            if (diversity._level > 1)
            {
                // 2. Increase the "Good" weights directly based on level
                // Every level adds a fixed "chunk" of probability
                averageWeight = diversity._level * 5f;
                baddieWeight = diversity._level * 2f;

                // 3. Shrink the "Ugly" weight so it doesn't stay dominant
                // We subtract the new weights from the 100 base
                uglyWeight = Mathf.Max(10f, 100f - (averageWeight + baddieWeight));
            }

            float totalWeight = baddieWeight + averageWeight + uglyWeight;

            // These will always equal 1.0 total
            float baddieProb = baddieWeight / totalWeight;
            float averageProb = averageWeight / totalWeight;
            float uglyProb = uglyWeight / totalWeight;

            return new float[] { uglyProb, averageProb, baddieProb };
        }
    }
}