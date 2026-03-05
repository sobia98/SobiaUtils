using UnityEngine;
using TMPro;

namespace Sobia.Utils
{
    public class TimerScript : MonoBehaviour
    {
        [SerializeField] private TMP_Text TimeText;
        private float TimeElapsed = 0;

        private void Awake()
        {
            SobiaUtils.IsAssigned(TimeText, nameof(TimeText), gameObject);
        }

        private void Update()
        {
            TimeElapsed += Time.deltaTime;
            DisplayTime(TimeElapsed);
        }

        private void DisplayTime(float timeToDisplay)
        {
            float minutes = Mathf.FloorToInt(timeToDisplay / 60);
            float seconds = Mathf.FloorToInt(timeToDisplay % 60);
            float milliSeconds = (timeToDisplay % 1) * 100;

            TimeText.text = string.Format("Time: {0:00}:{1:00}:{2:00}", minutes, seconds, milliSeconds);
        }
    }
}