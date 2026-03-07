using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Sobia.Utils;

namespace Sobia.CrackingEggs
{
    public enum GameState
    {
        Start,
        Finished,
        Paused,
        Upgrading,
        NONE,
    }

    public class SessionManager : MonoBehaviour, ILoggableState
    {
        [SerializeField] private TextMeshProUGUI TimerText;
        [SerializeField] private TextMeshProUGUI EggsCollectedText;
        public float GameDuration = 15f;

        private Coroutine GameFlowCoroutine;
        private float CurrentTimer;
        [SerializeField] private MoonMovement MoonMovement;

        [HideInInspector] public GameState CurrentGameState = GameState.NONE;
        [HideInInspector] public GameState PrevGameState = GameState.NONE;

        public static event Action OnGameEnd;

        public static event Action OnGameStart;

        public static SessionManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (TimerText is null)
            {
                Debug.LogError("TimerText is not assigned in SessionManager script.");
                return;
            }

            if (EggsCollectedText is null)
            {
                Debug.LogError("EggsCollectedText is not assigned in SessionManager script.");
                return;
            }

            if (MoonMovement == null)
            {
                Debug.Log("MoonMovement is not assigned");
                return;
            }

            CurrentTimer = GameDuration;
        }

        private void OnEnable()
        {
            PlayerController.OnEggCollected += UpdateEggUI;
        }

        private void OnDisable()
        {
            PlayerController.OnEggCollected -= UpdateEggUI;
        }

        public void TransitionTo(GameState newState)
        {
            CurrentGameState = newState;

            switch (newState)
            {
                case GameState.Start:
                    if (GameFlowCoroutine is not null)
                    {
                        StopCoroutine(GameFlowCoroutine);
                    }

                    GameFlowCoroutine = StartCoroutine(HandleStart());
                    break;

                case GameState.Finished:
                    HandleFinished();
                    break;

                case GameState.Paused:
                    HandlePaused();
                    break;

                case GameState.Upgrading:
                    HandleUpgrading();
                    break;

                case GameState.NONE:
                    break;
            }
        }

        private void HandlePaused()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            PanelManager.Instance.MenuPanel.Show();
            PanelManager.Instance.SettingsPanel.Hide();
            Time.timeScale = 0f;
        }

        private IEnumerator HandleStart()
        {
            TriggerGameStart();
            Time.timeScale = 1f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.None;

            if (PrevGameState != GameState.Paused)
            {
                MoonMovement.MoveMoon();
            }

            while (CurrentTimer > 0)
            {
                // Subtract the time passed since the last frame
                CurrentTimer -= Time.deltaTime;

                TimerText.text = $"Time: {Mathf.Max(0, CurrentTimer):F1} s";
                yield return null;
            }

            CurrentTimer = GameDuration;
            TimerText.text = "Time: 0.0 s";
            TriggerGameEnd();
        }

        public void TriggerGameStart()
        {
            PanelManager.Instance.MenuPanel.Hide();
            OnGameStart?.Invoke();
        }

        private void TriggerGameEnd()
        {
            OnGameEnd?.Invoke();
            PrevGameState = CurrentGameState;
            TransitionTo(GameState.Finished);
        }

        private void HandleFinished()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            PanelManager.Instance.MenuPanel.Show();
        }

        private void HandleUpgrading()
        {
            //UpgradeMenuUI.Show();
        }

        public void UpdateEggUI(int count)
        {
            EggsCollectedText.text = "Eggs: " + count;

            //AudioManager.Instance.PlayEggCollect();
            EggsCollectedText.transform.DOKill(); // Kill any ongoing animations
            EggsCollectedText.transform.localScale = Vector3.one; // Reset scale
            EggsCollectedText.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.2f);
        }

        public void LogEveryX()
        {
            Debug.Log("Current: " + CurrentGameState);
            Debug.Log("Prev: " + PrevGameState);
        }
    }
}