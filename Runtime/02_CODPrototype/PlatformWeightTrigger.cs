using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Sobia.CODProject
{
    public class PlatformWeightTrigger : NetworkBehaviour
    {
        [SerializeField] private int PlayerWeightThreshold = 2;

        private List<ulong> PlayersOnPlatform = new List<ulong>();
        private PlatformRotator Rotator;

        private void Awake()
        {
            Rotator = GetComponent<PlatformRotator>();
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && other.TryGetComponent(out NetworkObject netObj))
            {
                if (netObj.IsOwner)
                {
                    ReportPlatformTouchServerRpc(netObj.OwnerClientId);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && other.TryGetComponent(out NetworkObject netObj))
            {
                if (netObj.IsOwner)
                {
                    PlayerLeftPlatformServerRpc(netObj.OwnerClientId);
                }
            }
        }

        private void HandleClientDisconnect(ulong clientId)
        {
            if (PlayersOnPlatform.Contains(clientId))
            {
                PlayersOnPlatform.Remove(clientId);
                CheckWeight();
            }
        }

        private void CheckWeight()
        {
            if (PlayersOnPlatform.Count > PlayerWeightThreshold)
            {
                DeactivatePlatformClientRpc();
            }
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        private void ReportPlatformTouchServerRpc(ulong clientId)
        {
            if (!PlayersOnPlatform.Contains(clientId))
            {
                PlayersOnPlatform.Add(clientId);
                CheckWeight();
            }
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        private void PlayerLeftPlatformServerRpc(ulong clientId)
        {
            if (PlayersOnPlatform.Contains(clientId))
            {
                PlayersOnPlatform.Remove(clientId);
                CheckWeight();
            }
        }

        [ClientRpc]
        private void DeactivatePlatformClientRpc()
        {
            Rotator.Deactivate();
        }
    }
}