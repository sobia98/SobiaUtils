using UnityEngine;

namespace Sobia.SigmaboyProject
{
    public class UpgradeManager : MonoBehaviour
    {
        [Header("Audio Settings")]
        public AudioSource audioSource;

        public AudioClip[] upgradeSounds;

        [Header("References")]
        public SessionManager sessionManager;

        private void Start()
        {
            UpdateUI();
        }

        public void UpgradeRadius()
        {
            int cost = (int)DataManager.Instance.radius.GetUpgradeCostRadius();
            if (DataManager.Instance.totalHearts >= cost && cost > 0)
            {
                DataManager.Instance.totalHearts -= cost;
                DataManager.Instance.radius.Upgrade();
                FinishUpgrade();
            }
        }

        public void UpgradeSpawn()
        {
            int cost = (int)DataManager.Instance.spawn.GetUpgradeCostSpawnAmount();
            if (DataManager.Instance.totalHearts >= cost && cost > 0)
            {
                DataManager.Instance.totalHearts -= cost;
                DataManager.Instance.spawn.Upgrade();
                FinishUpgrade();
            }
        }

        public void UpgradeHeartReturn()
        {
            int cost = (int)DataManager.Instance.heartReturn.GetUpgradeCostHeartReturn();
            if (DataManager.Instance.totalHearts >= cost && cost > 0)
            {
                DataManager.Instance.totalHearts -= cost;
                DataManager.Instance.heartReturn.Upgrade();
                FinishUpgrade();
            }
        }

        public void UpgradeSpawnAreaSize()
        {
            int cost = (int)DataManager.Instance.spawnAreaSizeX.GetUpgradeCostSpawnSize();
            if (DataManager.Instance.totalHearts >= cost && cost > 0)
            {
                DataManager.Instance.totalHearts -= cost;
                DataManager.Instance.spawnAreaSizeX.Upgrade();
                DataManager.Instance.spawnAreaSizeY.Upgrade();
                FinishUpgrade();
            }
        }

        public void UpgradeProbablity()
        {
            int cost = (int)DataManager.Instance.probability.GetUpgradeCostProbability();
            if (DataManager.Instance.totalHearts >= cost && cost > 0)
            {
                DataManager.Instance.totalHearts -= cost;
                DataManager.Instance.probability.Upgrade();
                FinishUpgrade();
            }
        }

        public void UpgradeDiversity()
        {
            int cost = (int)DataManager.Instance.diversity.GetUpgradeCostDiversity();
            if (DataManager.Instance.totalHearts >= cost && cost > 0)
            {
                DataManager.Instance.totalHearts -= cost;
                DataManager.Instance.diversity.Upgrade();
                FinishUpgrade();
            }
        }

        public void UpgradeGirlMovementSpeed()
        {
            int cost = DataManager.Instance.girlMovementSpeed.GetUpgradeCost();
            if (DataManager.Instance.totalHearts >= cost)
            {
                DataManager.Instance.totalHearts -= cost;
                DataManager.Instance.girlMovementSpeed.Upgrade();
                FinishUpgrade();
            }
        }

        private void FinishUpgrade()
        {
            //ButtonsManager.Instance.CheckOnButtons();
            UpdateUI();
            PlayRandomUpgradeSound();
        }

        public void UpdateUI()
        {
            sessionManager.UpdateTotalLovesUI();
            FindFirstObjectByType<LiveStatsUI>().RefreshStats();
        }

        public void PlayRandomUpgradeSound()
        {
            if (upgradeSounds.Length > 0 && audioSource != null)
            {
                // Pick a random index from 0 to 9
                int randomIndex = Random.Range(0, upgradeSounds.Length);

                // Play the clip once
                audioSource.PlayOneShot(upgradeSounds[randomIndex]);
            }
        }
    }
}