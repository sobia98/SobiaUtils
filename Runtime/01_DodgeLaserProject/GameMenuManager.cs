using Sobia.Utils;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sobia.LaserProject
{
    public class GameMenuManager : NetworkBehaviour
    {
        [SerializeField] private GameObject PauseMenuUI;

        private InputAction ToggleMenuAction;

        public NetworkVariable<bool> IsPaused = new NetworkVariable<bool>(
             false,
             NetworkVariableReadPermission.Everyone,
             NetworkVariableWritePermission.Server);

        public static GameMenuManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            SobiaUtils.IsAssigned(PauseMenuUI, nameof(PauseMenuUI), gameObject);

            ToggleMenuAction = InputManager.Instance.InputActions.FindAction("Menu/ToggleMenu");
            if (ToggleMenuAction == null)
            {
                Debug.LogError("ToggleMenu action not found in the InputActionAsset.");
                return;
            }

            ToggleMenuAction.performed += OnToggleMenu;
        }

        private void Start()
        {
            ButtonManagerUI.Instance.ContinueGame.onClick.AddListener(() => OnClickContinue());
            ButtonManagerUI.Instance.RestartGame.onClick.AddListener(() => OnClickRestartGame());
            ButtonManagerUI.Instance.ResetScore.onClick.AddListener(() => GameUIManager.Instance.ResetHighScore());
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            IsPaused.OnValueChanged += OnPauseStateChanged;

            //when joining a game already in pause, set the correct state
            if (IsPaused.Value)
            {
                SetPauseState(IsPaused.Value);
            }

            if (!IsServer)
            {
                ButtonManagerUI.Instance.RestartGame.interactable = false;
                ButtonManagerUI.Instance.ResetScore.interactable = false;
            }
        }

        public override void OnNetworkDespawn()
        {
            IsPaused.OnValueChanged -= OnPauseStateChanged;
            base.OnNetworkDespawn();
        }

        private void OnEnable()
        {
            ToggleMenuAction?.Enable();
        }

        private void OnDisable()
        {
            ToggleMenuAction.performed -= OnToggleMenu;
            ToggleMenuAction?.Disable();
        }

        private void OnPauseStateChanged(bool previousValue, bool newValue)
        {
            SetPauseState(newValue);
        }

        private void SetPauseState(bool isPausedState)
        {
            if (isPausedState)
            {
                Time.timeScale = 0f;
                PauseMenuUI.SetActive(true);
            }
            else
            {
                Time.timeScale = 1f;
                PauseMenuUI.SetActive(false);
            }
        }

        private void OnToggleMenu(InputAction.CallbackContext context)
        {
            RequestPauseServerRpc(!IsPaused.Value);
        }

        public void OnClickContinue()
        {
            RequestPauseServerRpc(false);
        }

        public void OnClickRestartGame()
        {
            RequestPauseServerRpc(false);
            RestartGameClientRpc();
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        public void RequestPauseServerRpc(bool newState)
        {
            IsPaused.Value = newState;
        }

        [ClientRpc]
        public void RestartGameClientRpc()
        {
            GameManager.Instance.DestroyAllLasers();
            if (IsServer)
            {
                for (int i = 0; i < GameManager.Instance.ActivePlayerTimes.Count; i++)
                {
                    PlayerData data = GameManager.Instance.ActivePlayerTimes[i];

                    data.CurrentTime = 0f;
                    GameManager.Instance.ActivePlayerTimes[i] = data;
                }
            }

            //get all  gameobjects that has PlayerController component attached to it
            foreach (PlayerMovementHandler playerController in FindObjectsByType<PlayerMovementHandler>(FindObjectsSortMode.None))
            {
                playerController.MoveToStartClient();
            }
        }
    }
}