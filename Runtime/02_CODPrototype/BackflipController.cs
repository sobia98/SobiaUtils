using Sobia.Utils;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Sobia.CODProject
{
    public class BackflipController : NetworkBehaviour
    {
        private NetworkVariable<ulong> _playerOnStairId = new NetworkVariable<ulong>(
        999,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

        private readonly HashSet<ulong> _playersInTrigger = new HashSet<ulong>();

        [SerializeField] private GameObject PuzzleReceiver;
        private IPuzzleTrigger puzzleTrigger;

        private void Awake()
        {
            if (PuzzleReceiver is null)
            {
                Debug.LogError("PuzzleReceiver is not assigned in the inspector.");
            }

            if (PuzzleReceiver.TryGetComponent<IPuzzleTrigger>(out IPuzzleTrigger trigger))
            {
                puzzleTrigger = trigger;
            }

            if (puzzleTrigger is null)
            {
                Debug.LogError("PuzzleReceiver does not have a component that implements IPuzzleTrigger.");
            }
        }

        // This is the method the PlayerController will call
        public void PlayerDidBackflip(ulong clientId)
        {
            if (_playerOnStairId.Value == clientId)
            {
                puzzleTrigger.OnTriggerActivated(clientId);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && other.TryGetComponent(out NetworkObject netObj))
            {
                if (netObj.IsLocalPlayer)
                {
                    ReportPlayerOnPositionServerRpc(netObj.OwnerClientId);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && other.TryGetComponent(out NetworkObject netObj))
            {
                if (netObj.IsLocalPlayer)
                {
                    ReportPlayerOffPositionServerRpc(netObj.OwnerClientId);
                }
            }
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        private void ReportPlayerOnPositionServerRpc(ulong clientId)
        {
            _playersInTrigger.Add(clientId);
            if (_playerOnStairId.Value == 999) //if first player
            {
                _playerOnStairId.Value = clientId;
            }
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        private void ReportPlayerOffPositionServerRpc(ulong clientId)
        {
            _playersInTrigger.Remove(clientId);

            if (_playerOnStairId.Value == clientId)
            {
                if (_playersInTrigger.Count > 0)
                {
                    // The "Main" guy left, but someone else is still there.
                    // Pick a new "Main" player from the remaining set.
                    var enumerator = _playersInTrigger.GetEnumerator();
                    enumerator.MoveNext();
                    _playerOnStairId.Value = enumerator.Current;
                }
                else
                {
                    _playerOnStairId.Value = 999;
                }
            }
        }
    }
}