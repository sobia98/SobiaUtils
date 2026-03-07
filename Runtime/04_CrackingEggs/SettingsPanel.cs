using Sobia.Utils;
using UnityEngine;

namespace Sobia.CrackingEggs
{
    public class SettingsPanel : MonoBehaviour
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

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void PauseClicked()
        {
            if (SessionManager.Instance.CurrentGameState == GameState.NONE) return;
            if (SessionManager.Instance.CurrentGameState == GameState.Finished)
            {
                PanelManager.Instance.MenuPanel.Show();
                Hide();
            }
        }

        public void BackOnSessionClicked()
        {
            if (SessionManager.Instance.CurrentGameState == GameState.NONE)
            {
                PanelManager.Instance.OpenMainMenuPanel();
            }
            else
            {
                SessionManager.Instance.PrevGameState = SessionManager.Instance.CurrentGameState;
                SessionManager.Instance.TransitionTo(GameState.Paused);
            }
        }
    }
}