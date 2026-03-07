using Sobia.Utils;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Sobia.LaserProject
{
    public class LaserSpawner : NetworkBehaviour
    {
        [SerializeField] private GameObject LaserPrefab;
        [SerializeField] private AudioClip LaserSpawnClip;

        public readonly NetworkVariable<LaserSpawnerSettings> LaserSpawnerSettings = new NetworkVariable<LaserSpawnerSettings>(
                new LaserSpawnerSettings(),
                NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Server);

        private bool IsSpawning = false;

        public static LaserSpawner Instance { get; private set; }

        public void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            SobiaUtils.IsAssigned(LaserPrefab, nameof(LaserPrefab), gameObject);
            SobiaUtils.IsAssigned(LaserSpawnClip, nameof(LaserSpawnClip), gameObject);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            LaserSpawnerSettings.OnValueChanged += ApplySettings;

            // Apply initial settings if we are joining late TODO: Recheck later when refactoring
            ApplySettings(LaserSpawnerSettings.Value, LaserSpawnerSettings.Value);

            if (!IsServer) return;

            if (!IsSpawning)
            {
                LaserSpawnerSettings.Value = new LaserSpawnerSettings
                {
                    SpawnRangeX = 10f,
                    SpawnYPosition = 1f,
                    MinSpawnInterval = 1f,
                    MaxSpawnInterval = 3f,
                    Lifetime = 5f
                };

                IsSpawning = true;
                StartCoroutine(SpawnLasersRoutine());
            }
        }

        public override void OnNetworkDespawn()
        {
            LaserSpawnerSettings.OnValueChanged -= ApplySettings;
            base.OnNetworkDespawn();
        }

        private void ApplySettings(LaserSpawnerSettings previous, LaserSpawnerSettings current)
        {
        }

        private IEnumerator SpawnLasersRoutine()
        {
            while (IsSpawning)
            {
                float waitTime = Random.Range(LaserSpawnerSettings.Value.MinSpawnInterval, LaserSpawnerSettings.Value.MaxSpawnInterval);
                yield return new WaitForSeconds(waitTime);
                SpawnLaserObstacle();
            }
        }

        private void SpawnLaserObstacle()
        {
            float randomX = Random.Range(-LaserSpawnerSettings.Value.SpawnRangeX / 2f, LaserSpawnerSettings.Value.SpawnRangeX / 2f);
            Vector3 spawnPosition = new Vector3(randomX, LaserSpawnerSettings.Value.SpawnYPosition, transform.position.z);

            float rotationOption = Random.Range(0, 2);
            float fixedRotationY = rotationOption * 90f;

            Quaternion spawnRotation = Quaternion.Euler(0f, fixedRotationY, 0f);

            // 1. Instantiate LOCALLY on Server
            GameObject newLaserObj = Instantiate(LaserPrefab, spawnPosition, spawnRotation);
            NetworkObject newLaserNetObj = newLaserObj.GetComponent<NetworkObject>();

            if (newLaserNetObj != null)
            {
                // 2. Spawn GLOBALLY on Network
                newLaserNetObj.Spawn();
                StartCoroutine(DespawnLaserAfterDelay(newLaserNetObj, LaserSpawnerSettings.Value.Lifetime));

                // Use the NetworkObjectReference struct for reliable passing
                NetworkObjectReference laserRef = newLaserNetObj;
                PlaySpawnLaserClipClientRpc(laserRef);
            }
            else
            {
                Destroy(newLaserObj);
                Debug.LogError("Laser prefab does not have a NetworkObject component attached.");
            }
        }

        private IEnumerator DespawnLaserAfterDelay(NetworkObject networkLaserObject, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (networkLaserObject != null && networkLaserObject.IsSpawned)
            {
                networkLaserObject.Despawn();
            }
        }

        [ClientRpc]
        private void PlaySpawnLaserClipClientRpc(NetworkObjectReference laserRef)
        {
            // 1. Resolve the reference to get the GameObject/NetworkObject
            if (laserRef.TryGet(out NetworkObject laserNetObj))
            {
                // 2. Get the AudioSource component from the correct GameObject
                AudioSource sfxSource = laserNetObj.GetComponent<AudioSource>();

                // 3. Play the audio locally
                if (sfxSource != null && LaserSpawnClip != null)
                {
                    sfxSource.volume = 0.75f;
                    sfxSource.spatialBlend = 0.7f;
                    sfxSource.PlayOneShot(LaserSpawnClip);
                }
            }
            else
            {
                Debug.LogWarning("Failed to resolve NetworkObjectReference for audio playback.");
            }
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        public void SetNewSettingsServerRpc(LaserSpawnerSettings newSettings)
        {
            LaserSpawnerSettings.Value = newSettings;
        }
    }
}