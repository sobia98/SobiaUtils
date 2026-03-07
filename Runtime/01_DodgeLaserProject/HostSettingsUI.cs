using Sobia.Utils;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Sobia.LaserProject
{
    public class HostSettingsUI : MonoBehaviour
    {
        [SerializeField] private Slider LifeTime;
        [SerializeField] private Slider MinSpawnInterval;
        [SerializeField] private Slider MaxSpawnInterval;
        [SerializeField] private Button ApplyButton;

        private void Awake()
        {
            SobiaUtils.IsAssigned(LifeTime, nameof(LifeTime), gameObject);
            SobiaUtils.IsAssigned(MinSpawnInterval, nameof(MinSpawnInterval), gameObject);
            SobiaUtils.IsAssigned(MaxSpawnInterval, nameof(MaxSpawnInterval), gameObject);
            SobiaUtils.IsAssigned(ApplyButton, nameof(ApplyButton), gameObject);
        }

        private void Start()
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                LifeTime.interactable = false;
                MinSpawnInterval.interactable = false;
                MaxSpawnInterval.interactable = false;
                ApplyButton.interactable = false;
                return;
            }

            LifeTime.value = LaserSpawner.Instance.LaserSpawnerSettings.Value.Lifetime;
            MinSpawnInterval.value = LaserSpawner.Instance.LaserSpawnerSettings.Value.MinSpawnInterval;
            MaxSpawnInterval.value = LaserSpawner.Instance.LaserSpawnerSettings.Value.MaxSpawnInterval;

            ApplyButton.onClick.AddListener(OnApplySettingsClicked); //another way to submit settings than PlayerSettingsUI
        }

        private void OnApplySettingsClicked()
        {
            LaserSpawnerSettings newSettings = new LaserSpawnerSettings
            {
                SpawnRangeX = LaserSpawner.Instance.LaserSpawnerSettings.Value.SpawnRangeX,
                SpawnYPosition = LaserSpawner.Instance.LaserSpawnerSettings.Value.SpawnYPosition,
                Lifetime = LifeTime.value,
                MinSpawnInterval = MinSpawnInterval.value,
                MaxSpawnInterval = MaxSpawnInterval.value,
            };

            LaserSpawner.Instance.SetNewSettingsServerRpc(newSettings);
        }
    }
}