namespace Sobia.CrackingEggs
{
    using Sobia.Utils;
    using UnityEngine;

    public class MainMenuPanel : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log("MainMenuPanel Start");
            AudioManager.Instance.PlayUIOnOpenGameSound();
            SessionManager.Instance.PrevGameState = SessionManager.Instance.CurrentGameState;
            SessionManager.Instance.TransitionTo(GameState.NONE);
        }

        private void OnEnable()
        {
            SpawnerManager.Instance.StopSpawning();
            PlayerController.OnEggCollected -= SessionManager.Instance.UpdateEggUI;
            PanelManager.Instance.Player.SetActive(false);

            PanelManager.Instance.ParentOfTexts.SetActive(false);
            foreach (Transform child in PanelManager.Instance.ParentOfTexts.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in PanelManager.Instance.ParentOfEggs.transform)
            {
                if (child.TryGetComponent<Egg>(out Egg egg))
                {
                    egg.SetEggVisibility(false);
                }
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void PlayPressed()
        {
            PanelManager.Instance.TriggerSceneAnimationMenuToPlay(); ;
        }

        public void SettingsPressed()
        {
            PanelManager.Instance.OpenSettingsPanel();
        }

        public void ExitPressed()
        {
            Debug.Log("Game Exited");
            Application.Quit();
        }
    }
}