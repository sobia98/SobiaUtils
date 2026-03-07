using Sobia.Utils;
using UnityEngine;

namespace Sobia.CrackingEggs
{
    public class MenuPanel : MonoBehaviour
    {
        private void OnEnable()
        {
            var playerMap = InputManager.Instance.InputActions.FindActionMap("Player");
            var pauseAction = playerMap?.FindAction("Pause");

            if (pauseAction != null)
            {
                pauseAction.performed += ctx => PauseClicked();
            }
        }

        private void OnDisable()
        {
            var playerMap = InputManager.Instance.InputActions.FindActionMap("Player");
            var pauseAction = playerMap?.FindAction("Pause");

            if (pauseAction != null)
            {
                pauseAction.performed -= ctx => PauseClicked();
            }
        }

        public void PauseClicked()
        {
            if (SessionManager.Instance.CurrentGameState == GameState.Paused)
            {
                SessionManager.Instance.PrevGameState = SessionManager.Instance.CurrentGameState;
                SessionManager.Instance.TransitionTo(GameState.Start);
                PanelManager.Instance.SettingsPanel.Hide();
            }
            else if (SessionManager.Instance.CurrentGameState == GameState.Start)
            {
                SessionManager.Instance.PrevGameState = SessionManager.Instance.CurrentGameState;
                SessionManager.Instance.TransitionTo(GameState.Paused);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        // Linked to the "Resume" Button in the Inspector
        public void ContinueGame()
        {
            SessionManager.Instance.PrevGameState = SessionManager.Instance.CurrentGameState;
            SessionManager.Instance.TransitionTo(GameState.Start);
        }

        public void SettingsClicked()
        {
            PanelManager.Instance.SettingsPanel.Show();
            this.Hide();
        }

        public void MainMenuClicked()
        {
            PanelManager.Instance.TriggerSceneAnimationPlayToMenu();
        }
    }
}