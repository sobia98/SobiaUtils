using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

namespace Sobia.Utils
{
    public class PingDisplayUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI PingText;
        [SerializeField] private float UpdateInterval = 0.5f;

        private UnityTransport Transport;
        private float Timer;

        private void Awake()
        {
            PingText = GetComponent<TextMeshProUGUI>();
            SobiaUtils.IsAssigned(PingText, nameof(PingText), gameObject);
        }

        private void Update()
        {
            if (NetworkManager.Singleton == null)
            {
                PingText.text = "Ping: <color=#D3FF14>Disconnected (No NM)</color>";
                return;
            }

            // 1. Defer Transport Retrieval until the NetworkManager is ready
            if (Transport == null)
            {
                // Only try to get the component once the Netcode has started the process
                if (NetworkManager.Singleton.IsListening || NetworkManager.Singleton.IsClient)
                {
                    Transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                }

                // If transport is still null, we are not connected yet or something is wrong.
                if (Transport == null)
                {
                    PingText.text = "Ping: <color=#D3FF14>Connecting...</color>";
                    return;
                }
            }

            // --- Connection is established, proceed with ping calculation ---

            Timer += Time.unscaledDeltaTime;

            if (Timer >= UpdateInterval)
            {
                Timer = 0f;

                if (NetworkManager.Singleton.IsHost)
                {
                    // Host is the server, ping to self is zero
                    PingText.text = "Ping: <color=#D3FF14>0 ms (Host)</color>";
                }
                else if (NetworkManager.Singleton.IsClient)
                {
                    // Client connected to server
                    ulong serverId = NetworkManager.ServerClientId;
                    ulong pingMs = Transport.GetCurrentRtt(serverId);

                    // You can add a check for 0 ping if the connection is still initializing
                    if (pingMs == 0 && NetworkManager.Singleton.IsConnectedClient)
                    {
                        PingText.text = "Ping: <color=#D3FF14>...</color>"; // Wait for RTT calculation to start
                    }
                    else
                    {
                        PingText.text = $"Ping: <color=#D3FF14>{pingMs} ms</color>";
                    }
                }
                else
                {
                    // Fallback if somehow listening stopped
                    PingText.text = "Ping: <color=#D3FF14>Disconnected</color>";
                }
            }
        }
    }
}