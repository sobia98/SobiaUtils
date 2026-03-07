using UnityEngine;

namespace Sobia.CrackingEggs
{
    public class PlayPanel : MonoBehaviour
    {
        [Header("References")]
        public GameObject BloodPoolPrefab;

        public Transform BloodPoolContainer;

        private void OnEnable()
        {
            PlayerController.OnEggCollected += CustomMethod;
        }

        private void OnDisable()
        {
            PlayerController.OnEggCollected -= CustomMethod;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void CustomMethod(int onEggCollect)
        {
            float precentage = (float)onEggCollect / (float)PlayerController.Instance.BloodPoolMax;

            if (precentage >= 1.0f)
            {
                GameObject newEgg = Instantiate(BloodPoolPrefab, BloodPoolContainer);
            }
        }
    }
}