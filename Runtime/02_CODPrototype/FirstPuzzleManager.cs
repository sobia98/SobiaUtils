using Sobia.Utils;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Sobia.CODProject
{
    //Puzzle about two buttons needing to be pressed within 1.5 seconds of each other to activate Platforms
    public class FirstPuzzleManager : NetworkBehaviour, IPuzzleTrigger
    {
        [SerializeField] private List<GameObject> ObjectsToActivate;

        [SerializeField] private int RequiredButtonPresses = 2;
        [SerializeField] private float CoOpWindow = 1.5f;

        private float LastPressTime = -10f;
        private int ButtonsPressedCount = 0;

        public void OnTriggerActivated(ulong clientId)
        {
            ReportButtonPressServerRpc();
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        public void ReportButtonPressServerRpc()
        {
            if (!IsServer)
            {
                Debug.LogError("ReportButtonPressServerRpc called on client!");
                return;
            }

            if (RequiredButtonPresses <= 1)
            {
                TriggerTimedAction();
                ButtonsPressedCount = 0;
                LastPressTime = -10f;
                return;
            }

            float currentTime = Time.time;

            // Check if the first button was pressed too long ago
            if (currentTime - LastPressTime > CoOpWindow)
            {
                // Window expired! Reset the count to 1 (this press is now the "first" press)
                ButtonsPressedCount = 1;
                LastPressTime = currentTime;
                Debug.Log("First button pressed. Window started.");
            }
            else
            {
                // Window is still active!
                ButtonsPressedCount++;

                if (ButtonsPressedCount >= RequiredButtonPresses)
                {
                    Debug.Log("Both buttons pressed within 1.5s! Activating...");
                    TriggerTimedAction();
                    ButtonsPressedCount = 0;
                    LastPressTime = -10f;
                }
            }
        }

        // Call this instead of calling the ClientRpc directly
        public void TriggerTimedAction()
        {
            if (!IsServer) return;
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