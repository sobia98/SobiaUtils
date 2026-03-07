using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Sobia.Utils;

namespace Sobia.CrackingEggs
{
    public class PanelManager : MonoBehaviour
    {
        [SerializeField] public GameObject WhatToStartFirst;

        [Header("Panels")]
        [SerializeField] public MainMenuPanel MainMenuPanel;

        [SerializeField] public PlayPanel PlayPanel;
        [SerializeField] public SettingsPanel SettingsPanel;
        [SerializeField] public MenuPanel MenuPanel;

        [Header("Player & Eggs")]
        [SerializeField] public GameObject Player;

        [SerializeField] public GameObject ParentOfEggs;
        [SerializeField] public GameObject ParentOfTexts;

        [Header("Transition Scene")]
        [SerializeField] public Animator VortexAnimator;

        [SerializeField] public Image VortexImage;

        public static PanelManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            ValidateAssigned();
        }

        private void Start()
        {
            if (WhatToStartFirst.name == "PlayPanel")
            {
                PlayPanelSetup();
            }
            else if (WhatToStartFirst.name == "SettingsPanel")
            {
                OpenSettingsPanel();
            }
            else
            {
                MainMenuPanelSetup();
            }
        }

        private void MainMenuPanelSetup()
        {
            OpenMainMenuPanel();
            SpawnerManager.Instance.StopSpawning();
            PlayerController.OnEggCollected -= SessionManager.Instance.UpdateEggUI;
            Player.SetActive(false);
            ParentOfTexts.SetActive(false);

            foreach (Transform child in ParentOfEggs.transform)
            {
                if (child.TryGetComponent<Egg>(out Egg egg))
                {
                    egg.SetEggVisibility(false);
                }
            }
        }

        public void OpenMainMenuPanel()
        {
            MainMenuPanel.Show();
            PlayPanel.Hide();
            SettingsPanel.Hide(); ;
        }

        public void OpenPlayPanel()
        {
            MainMenuPanel.Hide();
            PlayPanel.Show();
            SettingsPanel.Hide();
        }

        public void OpenSettingsPanel()
        {
            MainMenuPanel.Hide();
            PlayPanel.Hide();
            SettingsPanel.Show();
        }

        public void TriggerSceneAnimationMenuToPlay()
        {
            StartCoroutine(PlaySceneAnimationMenuToPlay());
        }

        private IEnumerator PlaySceneAnimationMenuToPlay()
        {
            VortexAnimator.SetTrigger("VortexStart");
            VortexImage.raycastTarget = true;
            InputManager.Instance.DisableInput();
            yield return new WaitForSecondsRealtime(1f);

            PlayPanelSetup();

            VortexAnimator.SetTrigger("VortexEnd");
            yield return new WaitForSecondsRealtime(1f);
            VortexImage.raycastTarget = false;
            InputManager.Instance.EnableInput();
        }

        private void PlayPanelSetup()
        {
            OpenPlayPanel();
            SpawnerManager.Instance.StartSpawning();
            PlayerController.OnEggCollected += SessionManager.Instance.UpdateEggUI;

            SessionManager.Instance.PrevGameState = SessionManager.Instance.CurrentGameState;
            SessionManager.Instance.TransitionTo(GameState.Start);
            Player.SetActive(true);
            ParentOfTexts.SetActive(true);

            foreach (Transform child in ParentOfEggs.transform)
            {
                if (child.TryGetComponent<Egg>(out Egg egg))
                {
                    egg.SetEggVisibility(true);
                }
            }
        }

        public void TriggerSceneAnimationPlayToMenu()
        {
            StartCoroutine(PlaySceneAnimationPlayToMenu());
        }

        private IEnumerator PlaySceneAnimationPlayToMenu()
        {
            VortexAnimator.SetTrigger("VortexStart");
            VortexImage.raycastTarget = true;
            InputManager.Instance.DisableInput();
            yield return new WaitForSecondsRealtime(1f);

            OpenMainMenuPanel();

            VortexAnimator.SetTrigger("VortexEnd");
            yield return new WaitForSecondsRealtime(1f);
            VortexImage.raycastTarget = false;
            InputManager.Instance.EnableInput();
            SessionManager.Instance.PrevGameState = SessionManager.Instance.CurrentGameState;
            SessionManager.Instance.TransitionTo(GameState.NONE);
        }

        private void ValidateAssigned()
        {
            // Basic Setup
            SobiaUtils.IsAssigned(WhatToStartFirst, nameof(WhatToStartFirst), gameObject);

            // Panels
            SobiaUtils.IsAssigned(MainMenuPanel, nameof(MainMenuPanel), gameObject);
            SobiaUtils.IsAssigned(PlayPanel, nameof(PlayPanel), gameObject);
            SobiaUtils.IsAssigned(SettingsPanel, nameof(SettingsPanel), gameObject);
            SobiaUtils.IsAssigned(MenuPanel, nameof(MenuPanel), gameObject);

            // Player & Eggs
            SobiaUtils.IsAssigned(Player, nameof(Player), gameObject);
            SobiaUtils.IsAssigned(ParentOfEggs, nameof(ParentOfEggs), gameObject);
            SobiaUtils.IsAssigned(ParentOfTexts, nameof(ParentOfTexts), gameObject);

            // Transition Scene
            SobiaUtils.IsAssigned(VortexAnimator, nameof(VortexAnimator), gameObject);
            SobiaUtils.IsAssigned(VortexImage, nameof(VortexImage), gameObject);
        }
    }
}