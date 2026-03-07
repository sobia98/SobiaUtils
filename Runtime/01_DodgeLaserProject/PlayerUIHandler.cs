using Sobia.Utils;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Sobia.LaserProject
{
    // this handle UI elements related to the player, such as their name and timer
    public class PlayerUIHandler : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private TextMeshProUGUI playerTimerText;

        private NetworkVariable<FixedString64Bytes> ChosenName = new NetworkVariable<FixedString64Bytes>(
            writePerm: NetworkVariableWritePermission.Server);

        private readonly NetworkVariable<FixedString64Bytes> PlayerName = new NetworkVariable<FixedString64Bytes>(
            writePerm: NetworkVariableWritePermission.Server);

        private void Start()
        {
            SobiaUtils.IsAssigned(playerNameText, nameof(playerNameText), gameObject);
            SobiaUtils.IsAssigned(playerTimerText, nameof(playerTimerText), gameObject);

            InvokeRepeating(nameof(UpdatePlayerTimer), 0.01f, 0.01f);
        }

        private void UpdatePlayerTimer()
        {
            if (!IsSpawned) return;

            // Find the player's entry in the synchronized list
            PlayerData playerTimeEntry = GameManager.Instance.ActivePlayerTimes.AsNativeArray()
       .FirstOrDefault(data => data.ClientId == NetworkObject.OwnerClientId);

            playerTimerText.text = playerTimeEntry.CurrentTime.ToString("F2");
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            PlayerName.OnValueChanged += OnPlayerNameChanged;
            OnPlayerNameChanged(PlayerName.Value, PlayerName.Value);

            if (IsServer)
            {
                ChosenName.Value = GameManager.Instance.GetClientName(OwnerClientId); // Get the name from LobbyManager, cant do it in IsOwner because no access to DB as client
            }

            if (IsOwner)
            {
                // Request the server to set the name using the value from the LobbyManager
                RequestNameChangeServerRpc(ChosenName.Value.ToString());
                playerNameText.gameObject.SetActive(true);
            }
        }

        public override void OnNetworkDespawn()
        {
            PlayerName.OnValueChanged -= OnPlayerNameChanged;
        }

        private void OnPlayerNameChanged(FixedString64Bytes oldValue, FixedString64Bytes newValue)
        {
            playerNameText.text = newValue.ToString();
        }

        public void SetPlayerName(string newName)
        {
            PlayerName.Value = newName;
        }

        [ServerRpc]
        private void RequestNameChangeServerRpc(string newName)
        {
            SetPlayerName(newName);
        }
    }
}