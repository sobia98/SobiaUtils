using UnityEngine;
using UnityEngine.UI;

namespace Sobia.LaserProject
{
    public class ButtonManagerUI : MonoBehaviour
    {
        public static ButtonManagerUI Instance { get; private set; }

        [SerializeField] private Button femaleSkin;
        [SerializeField] private Button maleSkin;
        [SerializeField] private Button goblinSkin;
        [SerializeField] private Button continueGame;
        [SerializeField] private Button resetScore;
        [SerializeField] private Button restartGame;

        //read only properties to access the buttons, like a getter
        public Button FemaleSkin => femaleSkin;

        public Button MaleSkin => maleSkin;
        public Button GoblinSkin => goblinSkin;
        public Button ContinueGame => continueGame;
        public Button ResetScore => resetScore;
        public Button RestartGame => restartGame;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}