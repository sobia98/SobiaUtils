using Sobia.Utils;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Sobia.CODProject
{
    public class SecondPuzzleManager : NetworkBehaviour, IPuzzleTrigger
    {
        [Header("Puzzle Settings")]
        [SerializeField] private List<GameObject> ObjectsToActivate;

        [SerializeField] private float CoOpWindow = 1.5f;
        [SerializeField] private int RequiredPlayers = 2;

        private int _backflipsInWindowCount = 0;
        private float _lastBackflipTime = -10f;
        private HashSet<ulong> _playersWhoBackflipped = new HashSet<ulong>();

        public void OnTriggerActivated(ulong clientId)
        {
            ReportBackflipServerRpc(clientId);
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        public void ReportBackflipServerRpc(ulong clientId)
        {
            if (!IsServer)
            {
                Debug.LogError("ReportButtonPressServerRpc called on client!");
                return;
            }

            if (RequiredPlayers <= 1)
            {
                TriggerTimedAction();
                ResetManager();
                return;
            }

            float currentTime = Time.time;

            if (currentTime - _lastBackflipTime > CoOpWindow)
            {
                // Start new window
                _backflipsInWindowCount = 1;
                _playersWhoBackflipped.Clear();
                _playersWhoBackflipped.Add(clientId);
                _lastBackflipTime = currentTime;
                Debug.Log($"[Manager {name}] New window started by Player {clientId}");
            }
            else if (!_playersWhoBackflipped.Contains(clientId))
            {
                // Add to existing window
                _backflipsInWindowCount++;
                _playersWhoBackflipped.Add(clientId);
                Debug.Log($"[Manager {name}] Success! {_backflipsInWindowCount}/{RequiredPlayers}");

                if (_backflipsInWindowCount >= RequiredPlayers)
                {
                    TriggerTimedAction();
                    ResetManager();
                }
            }
        }

        private void ResetManager()
        {
            _backflipsInWindowCount = 0;
            _playersWhoBackflipped.Clear();
            _lastBackflipTime = -10f;
        }

        public void TriggerTimedAction()
        {
            if (!IsServer) return;
            Debug.Log($"PUZZLE {name} CLEARED!");
            TriggerActionClientRpc();
        }

        [ClientRpc]
        private void TriggerActionClientRpc()
        {
            foreach (var obj in ObjectsToActivate)
            {
                if (obj.TryGetComponent(out IActivatable activatable))
                {
                    activatable.Activate();
                }
            }
        }
    }
}