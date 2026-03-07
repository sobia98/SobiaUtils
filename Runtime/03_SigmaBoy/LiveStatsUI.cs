using TMPro;
using UnityEngine;

namespace Sobia.SigmaboyProject
{
    public class LiveStatsUI : MonoBehaviour
    {
        [Header("UI References")]
        public TextMeshProUGUI statsText; // Drag your UI Text component here

        public void RefreshStats()
        {
            if (DataManager.Instance == null) return;

            // Fetching data from your DataManager
            float radius = DataManager.Instance.radius.GetValue();
            int spawnCount = (int)DataManager.Instance.spawn.GetValue();
            float multiplier = DataManager.Instance.heartReturn.GetValue();

            // Use the same formula as in GirlNPC to show the chance to date a girl.
            // Example shown here: "Normal" tier girl with baseCatchChance = 0.60f.
            float globalProbBonus = DataManager.Instance.probability.GetValue();
            float catchProbabilityNormal = Mathf.Min(GameConstants.NORMAL_CHANCE + globalProbBonus, GameConstants.CATCH_PROBABILITY_CAP) * 100f;
            float catchProbabilityAverage = Mathf.Min(GameConstants.AVERAGE_CHANCE + globalProbBonus, GameConstants.CATCH_PROBABILITY_CAP) * 100f;
            float catchProbabilityBaddie = Mathf.Min(GameConstants.BADDIE_CHANCE + globalProbBonus, GameConstants.CATCH_PROBABILITY_CAP) * 100f;

            float girlMovementSpeed = DataManager.Instance.girlMovementSpeed.GetValue();
            float spawnAreaSizeX = DataManager.Instance.spawnAreaSizeX.GetValue();
            float spawnAreaSizeY = DataManager.Instance.spawnAreaSizeY.GetValue();
            float[] w = DataManager.Instance.GetSpawnWeights();
            float total = w[0] + w[1] + w[2];
            float uglyChance = (w[0] / total) * 100f;
            float normalChance = (w[1] / total) * 100f;
            float baddieChance = (w[2] / total) * 100f;

            // Format the string for the UI
            string report = "<b>--- LIVE STATS ---</b>\n";
            report += $"Aura Radius: {radius:F1}m\n";
            report += $"Spawn Amount: {spawnCount}\n";
            report += $"Heart Multiplier: x{multiplier}\n";
            report += $"Success Rate (Normal girl): {catchProbabilityNormal:F0}%\n";
            report += $"Success Rate (average girl): {catchProbabilityAverage:F0}%\n";
            report += $"Success Rate (baddie girl): {catchProbabilityBaddie:F0}%\n";
            report += $"Uglys Spawn Rate: {uglyChance:F1}%\n";
            report += $"Normal Spawn Rate: {normalChance:F1}%\n";
            report += $"Baddies Spawn Rate: {baddieChance:F1}%\n";
            report += $"Movement Speed Multiplayer: {girlMovementSpeed:F1}\n";
            report += $"Spawn Area Size X: {spawnAreaSizeX:F1}m\n";
            report += $"Spawn Area Size Y: {spawnAreaSizeY:F1}m\n";

            statsText.text = report;
        }
    }
}