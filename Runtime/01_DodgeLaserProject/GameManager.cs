using Sobia.Utils;
using Unity.Netcode;
using UnityEngine;

namespace Sobia.LaserProject
{
    public class GameManager : NetworkBehaviour
    {
        public NetworkList<PlayerData> ActivePlayerTimes;

        public readonly NetworkVariable<PlayerMovementSettings> PlayerMovementSettings = new NetworkVariable<PlayerMovementSettings>(
            new PlayerMovementSettings(15f, 2f),
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }

            ActivePlayerTimes = new NetworkList<PlayerData>();
        }

        private void Start()
        {
            GameUIManager.Instance.LoadHighScore();
            InvokeRepeating(nameof(CalculateAndSyncHighscore), 0.01f, 0.01f);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            ActivePlayerTimes.OnListChanged += OnActivePlayerTimesChanged;

            if (IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;

                string clientName = GetClientName(NetworkObject.OwnerClientId);
                ActivePlayerTimes.Add(new PlayerData(NetworkObject.OwnerClientId, 0f, clientName));
                HandleClientConnected(NetworkManager.Singleton.LocalClientId);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (NetworkManager.Singleton != null && IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
            }

            base.OnNetworkDespawn();
        }

        private void Update()
        {
            if (GameMenuManager.Instance.IsPaused.Value || !IsServer) return;

            float deltaTime = Time.deltaTime;
            for (int i = 0; i < ActivePlayerTimes.Count; i++)
            {
                PlayerData data = ActivePlayerTimes[i];

                data.CurrentTime += deltaTime;
                ActivePlayerTimes[i] = data;
            }
        }

        private void CalculateAndSyncHighscore()
        {
            if (GameMenuManager.Instance.IsPaused.Value || !IsServer) return;

            // Use a simple for loop for the performance-critical part:
            PlayerData bestPlayer = new PlayerData();
            foreach (PlayerData data in ActivePlayerTimes)
            {
                if (data.CurrentTime > bestPlayer.CurrentTime)
                {
                    bestPlayer = data;
                }
            }
            GameUIManager.Instance.CurrentRecordPlayer.Value = bestPlayer;
        }

        public void DestroyAllLasers()
        {
            if (!IsServer) return;

            GameObject[] lasers = GameObject.FindGameObjectsWithTag("Laser");
            foreach (GameObject laser in lasers)
            {
                if (laser.TryGetComponent(out NetworkObject netObj))
                {
                    if (netObj.IsSpawned)
                    {
                        netObj.Despawn();
                    }
                    else
                    {
                        Destroy(laser);
                    }
                }
            }
        }

        public string GetClientName(ulong clientId)
        {
            if (ServerConnectionData.TryGetClientName(clientId, out string name))
            {
                return name;
            }

            return "Unknown" + clientId;
        }

        private void HandleClientConnected(ulong clientId)
        {
            if (!IsServer) return;
            if (NetworkObject.OwnerClientId == clientId) return;
            string clientName = GetClientName(clientId);

            ActivePlayerTimes.Add(new PlayerData(clientId, 0f, clientName));
        }

        private void HandleClientDisconnected(ulong clientId)
        {
            if (!IsServer) return;
            // Find and remove the entry for the disconnecting client
            for (int i = 0; i < ActivePlayerTimes.Count; i++)
            {
                if (ActivePlayerTimes[i].ClientId == clientId)
                {
                    ActivePlayerTimes.RemoveAt(i);
                    break;
                }
            }
        }

        private void OnActivePlayerTimesChanged(NetworkListEvent<PlayerData> changeEvent)
        {
            // Implement UI update logic here: rebuild the scoreboard when the list changes
        }

        [Rpc(SendTo.Server)] // This is called by the client that was hit
        public void RequestPlayerAfterHitServerRpc(ulong clientIdToReset)
        {
            DestroyAllLasers();

            //define the parameters to target ONLY the client who was hit.
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientIdToReset }
                }
            };

            //call the ClientRpc, using the defined parameters to restrict the audience.
            PlayerAfterHitServerClientRpc(clientRpcParams);
        }

        [ClientRpc] // This is received ONLY by the client specified in the params
        public void PlayerAfterHitServerClientRpc(ClientRpcParams clientRpcParams = default)
        {
            // We only want this client to move *itself* and reset the timer if it's the server.
            foreach (PlayerMovementHandler playerController in FindObjectsByType<PlayerMovementHandler>(FindObjectsSortMode.None))
            {
                // Only move the player that this client owns
                if (playerController.IsOwner)
                {
                    playerController.MoveToStartClient();
                    break;
                }
            }
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        public void SetMovementSettingsServerRpc(float newSpeed)
        {
            if (!IsServer) return;

            PlayerMovementSettings newSettings = PlayerMovementSettings.Value;
            newSettings.Speed = newSpeed;

            PlayerMovementSettings.Value = newSettings;
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        public void SetJumpSettingsServerRpc(float newJumpHeight)
        {
            if (!IsServer) return;

            PlayerMovementSettings newSettings = PlayerMovementSettings.Value;
            newSettings.JumpHeight = newJumpHeight;

            PlayerMovementSettings.Value = newSettings;
        }
    }
}