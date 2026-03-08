using log4net.Util;
using TMPro;
using Unity.Netcode;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace Sobia.Utils
{
    /// <summary>
    /// Assign it to NetworkManager object in the scene.
    /// Assign OnClick Functions from Buttons
    /// </summary>
    public class NetworkLobby : MonoBehaviour
    {
        [SerializeField] private TMP_InputField NameInputField;
        [SerializeField] private TMP_InputField JoinCodeInputField;
        [SerializeField] private TMP_Text JoinCodeDisplay;
        [SerializeField] private TMP_Text SystemMessage;

        public static string CurrentJoinCode { get; private set; } = "";
        private static string LocalPlayerName { get; set; } = "Player"; // Default name

        private void Awake()
        {
            SobiaUtils.IsAssigned(NameInputField, nameof(NameInputField), gameObject);
            SobiaUtils.IsAssigned(JoinCodeDisplay, nameof(JoinCodeDisplay), gameObject);
            SobiaUtils.IsAssigned(JoinCodeInputField, nameof(JoinCodeInputField), gameObject);
            SobiaUtils.IsAssigned(SystemMessage, nameof(SystemMessage), gameObject);
        }

        private void Start()
        {
            NameInputField.text = LocalPlayerName;
            NameInputField.onValueChanged.AddListener(UpdateLocalPlayerName);
        }

        private void UpdateLocalPlayerName(string newName)
        {
            LocalPlayerName = newName;
        }

        public async void StartHostRelay(int maxConnections = 6)
        {
            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);

                try
                {
                    var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
                    transport.SetHostRelayData(
                        allocation.RelayServer.IpV4,
                        (ushort)allocation.RelayServer.Port,
                        allocation.AllocationIdBytes,
                        allocation.Key,
                        allocation.ConnectionData
                    );

#if UNITY_WEBGL
                    transport.UseWebSockets = true;
                    string connectionType = "wss";
#else
                    transport.UseWebSockets = false;
                    string connectionType = "dtls";
#endif

                    var relayServerData = AllocationUtils.ToRelayServerData(allocation, connectionType);
                    transport.SetRelayServerData(relayServerData);

                    NetworkManager.Singleton.ConnectionApprovalCallback = ConnectionApprovalCallback;
                    byte[] payload = System.Text.Encoding.UTF8.GetBytes(LocalPlayerName);
                    NetworkManager.Singleton.NetworkConfig.ConnectionData = payload;
                    NetworkManager.Singleton.StartHost();

                    gameObject.SetActive(false);
                    SystemMessage.enabled = false;
                    CurrentJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                    JoinCodeDisplay.text = $"Join Code: <color=#D3FF14>{CurrentJoinCode}</color>";
                    Debug.Log($"Relay Allocation created with Join Code: {CurrentJoinCode}");
                }
                catch (RelayServiceException e)
                {
                    SystemMessage.text = "Failed to get Joincode from Relay Service!";
                    Debug.LogError($"Relay Host failed: {e.Message}");
                }
            }
            catch (RelayServiceException e)
            {
                SystemMessage.text = "Failed to start Relay Host!";
                Debug.LogError($"Relay Host failed: {e.Message}");
            }
        }

        public async void StartClientRelay()
        {
            CurrentJoinCode = JoinCodeInputField.text.Trim();

            if (string.IsNullOrEmpty(CurrentJoinCode))
            {
                SystemMessage.text = "Please enter a Join Code.";
                Debug.LogError($"Please enter a Join Code.");
                return;
            }

            try
            {
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(CurrentJoinCode);
                var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
                transport.SetClientRelayData(
                    joinAllocation.RelayServer.IpV4,
                    (ushort)joinAllocation.RelayServer.Port,
                    joinAllocation.AllocationIdBytes,
                    joinAllocation.Key,
                    joinAllocation.ConnectionData,
                    joinAllocation.HostConnectionData
                );

#if UNITY_WEBGL
                transport.UseWebSockets = true;
                string connectionType = "wss"; // Required for Browser
#else
                transport.UseWebSockets = false;
                string connectionType = "dtls";
#endif

                // Use AllocationUtils to handle the JoinAllocation data correctly
                var relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, connectionType);
                transport.SetRelayServerData(relayServerData);

                byte[] payload = System.Text.Encoding.UTF8.GetBytes(LocalPlayerName);
                NetworkManager.Singleton.NetworkConfig.ConnectionData = payload;
                NetworkManager.Singleton.StartClient();

                gameObject.SetActive(false);
                SystemMessage.enabled = false;
                JoinCodeDisplay.text = $"Join Code: <color=#D3FF14>{CurrentJoinCode}</color>";
                Debug.Log($"Relay Joining with Code: {CurrentJoinCode}");
            }
            catch (RelayServiceException e)
            {
                SystemMessage.text = "Join Code is not valid!";
                Debug.LogError($"Failed to join Relay: {e.Message}");
            }
        }

        /// <summary>
        /// need to checkbox Connection Apporval on Network Manager
        /// </summary>
        private void ConnectionApprovalCallback(
            NetworkManager.ConnectionApprovalRequest request,
            NetworkManager.ConnectionApprovalResponse response)
        {
            byte[] connectionData = request.Payload;
            ulong clientId = request.ClientNetworkId;

            string clientName = "Player" + clientId; // Default fallback

            //unpackage name
            if (connectionData != null && connectionData.Length > 0)
            {
                clientName = System.Text.Encoding.UTF8.GetString(connectionData);
            }

            ServerConnectionData.AddClientName(clientId, clientName);

            // 4. Approve the connection (usually always true for now)
            response.Approved = true;
            response.CreatePlayerObject = true;
        }

        public void StartHostLocal()
        {
            var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();

#if UNITY_WEBGL
            transport.UseWebSockets = true;
#else
            transport.UseWebSockets = false;
#endif
            NetworkManager.Singleton.ConnectionApprovalCallback = ConnectionApprovalCallback;
            CurrentJoinCode = "Local";
            byte[] payload = System.Text.Encoding.UTF8.GetBytes(LocalPlayerName);
            NetworkManager.Singleton.NetworkConfig.ConnectionData = payload;
            NetworkManager.Singleton.StartHost();

            gameObject.SetActive(false);
            SystemMessage.enabled = false;
            JoinCodeDisplay.text = $"Join Code: <color=#D3FF14>{CurrentJoinCode}</color>";
            Debug.Log($"Starting {(transport.UseWebSockets ? "WebSocket" : "UDP")} Local Host...");
        }

        public void StartClientLocal()
        {
            var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();

#if UNITY_WEBGL
            transport.UseWebSockets = true;
#else
            transport.UseWebSockets = false;
#endif
            CurrentJoinCode = "Local";
            byte[] payload = System.Text.Encoding.UTF8.GetBytes(LocalPlayerName);
            NetworkManager.Singleton.NetworkConfig.ConnectionData = payload;
            NetworkManager.Singleton.StartClient();

            gameObject.SetActive(false);
            SystemMessage.enabled = false;
            JoinCodeDisplay.text = $"Join Code: <color=#D3FF14>{CurrentJoinCode}</color>";
            Debug.Log($"Starting {(transport.UseWebSockets ? "WebSocket" : "UDP")} Local Client...");
        }
    }
}