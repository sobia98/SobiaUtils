using Sobia.Utils;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Sobia.LaserProject
{
    public class GameUIManager : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI HighRecordText;
        [SerializeField] private TextMeshProUGUI CurrentRecordText1;
        [SerializeField] private TextMeshProUGUI CurrentRecordText2;

        private const string HIGH_SCORE_KEY = "HighScore";
        private const string HIGH_SCORE_NAME_KEY = "HighScoreName";

        private NetworkVariable<float> HighestTime = new NetworkVariable<float>(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

        private NetworkVariable<FixedString64Bytes> HighestName = new NetworkVariable<FixedString64Bytes>(
            "",
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        public NetworkVariable<PlayerData> CurrentRecordPlayer = new NetworkVariable<PlayerData>(
         new PlayerData(0, 0f, "no one"),
         NetworkVariableReadPermission.Everyone,
         NetworkVariableWritePermission.Server);

        public static GameUIManager Instance { get; private set; }

        public void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }

            SobiaUtils.IsAssigned(HighRecordText, nameof(HighRecordText), gameObject);
            SobiaUtils.IsAssigned(CurrentRecordText1, nameof(CurrentRecordText1), gameObject);
            SobiaUtils.IsAssigned(CurrentRecordText2, nameof(CurrentRecordText2), gameObject);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            HighestTime.OnValueChanged += (previous, current) => UpdateHighScoreDisplay();
            HighestName.OnValueChanged += (previous, current) => UpdateHighScoreDisplay();
            CurrentRecordPlayer.OnValueChanged += (previous, current) => UpdateCurrentTimeDisplay(previous, current);

            UpdateCurrentTimeDisplay(CurrentRecordPlayer.Value, CurrentRecordPlayer.Value);

            if (IsServer)
            {
                CurrentRecordPlayer.Value = GameManager.Instance.ActivePlayerTimes[0];
            }

            LoadHighScore();
        }

        public override void OnNetworkDespawn()
        {
            HighestTime.OnValueChanged -= (previous, current) => UpdateHighScoreDisplay();
            HighestName.OnValueChanged -= (previous, current) => UpdateHighScoreDisplay();
            CurrentRecordPlayer.OnValueChanged -= UpdateCurrentTimeDisplay;
            base.OnNetworkDespawn();
        }

        private void UpdateHighScoreDisplay()
        {
            string highTimeString = HighestTime.Value.ToString("F2");
            HighRecordText.text = $"Best Record <color=#D3FF14>{HighestName.Value}</color> with <color=#D3FF14>{highTimeString}</color>";
        }

        public void LoadHighScore()
        {
            if (IsServer)
            {
                HighestTime.Value = PlayerPrefs.GetFloat(HIGH_SCORE_KEY, 0f);
                HighestName.Value = PlayerPrefs.GetString(HIGH_SCORE_NAME_KEY, "Player");
            }
            UpdateHighScoreDisplay();
        }

        public void ResetHighScore()
        {
            if (!IsServer) return;
            PlayerPrefs.DeleteKey(HIGH_SCORE_KEY);
            PlayerPrefs.DeleteKey(HIGH_SCORE_NAME_KEY);
            PlayerPrefs.Save();
            LoadHighScore();
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        public void ReportNewHighScoreServerRpc(ulong triggeringClientId)
        {
            if (!IsServer) return;

            int playerIndex = -1;
            PlayerData playerTimeEntry = default;

            for (int i = 0; i < GameManager.Instance.ActivePlayerTimes.Count; i++)
            {
                if (GameManager.Instance.ActivePlayerTimes[i].ClientId == triggeringClientId)
                {
                    playerIndex = i;
                    playerTimeEntry = GameManager.Instance.ActivePlayerTimes[i];
                    break;
                }
            }

            // Check if the player was found in the list
            if (playerIndex == -1)
            {
                Debug.LogWarning($"High Score Reported by unknown Client ID: {triggeringClientId}");
                return;
            }

            // --- High Score Check ---
            if (playerTimeEntry.CurrentTime > HighestTime.Value)
            {
                HighestTime.Value = playerTimeEntry.CurrentTime;
                HighestName.Value = playerTimeEntry.ClientName;
                CurrentRecordPlayer.Value = playerTimeEntry;

                PlayerPrefs.SetFloat(HIGH_SCORE_KEY, HighestTime.Value);
                PlayerPrefs.SetString(HIGH_SCORE_NAME_KEY, HighestName.Value.ToString());
                PlayerPrefs.Save();
            }

            playerTimeEntry.CurrentTime = 0f;
            GameManager.Instance.ActivePlayerTimes[playerIndex] = playerTimeEntry;
        }

        private void UpdateCurrentTimeDisplay(PlayerData oldData, PlayerData newData)
        {
            // The data is already synchronized via the NetworkVariable
            string timeString = newData.CurrentTime.ToString("F2");

            string leaderName = newData.ClientName.ToString();

            if (oldData.ClientId != newData.ClientId)
            {
                //AudioManager.Instance.PlayLeaderClip();
            }
            CurrentRecordText1.text = $"Leading: <color=#D3FF14>{leaderName}</color> with <color=#D3FF14>{timeString}</color>";
        }
    }
}