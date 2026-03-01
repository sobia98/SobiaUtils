using UnityEngine;
using TMPro;

namespace Sobia.Utils
{
    public static class SobiaDiagnositcs
    {
        /// <summary>
        /// TextMeshProUGUI is required.
        /// Shows FPS
        /// </summary>
        public class FPSDisplay : MonoBehaviour
        {
            [RequiredMember]
            public TextMeshProUGUI FPSText;

            [HideInInspector]
            public bool IsVisible = true;

            private float UpdateDeltaTime = 0.0f;
            private float UpdateTimer = 0f;
            private float UpdateInterval = 0.5f;

            private void Start()
            {
                FPSText.gameObject.SetActive(IsVisible);
            }

            private void Update()
            {
                if (!IsVisible) return;

                UpdateDeltaTime += (Time.unscaledDeltaTime - UpdateDeltaTime) * 0.1f;

                UpdateTimer += Time.unscaledDeltaTime;
                if (UpdateTimer >= UpdateInterval)
                {
                    float fps = 1.0f / UpdateDeltaTime;
                    FPSText.text = $"FPS: {Mathf.Ceil(fps)}";

                    UpdateTimer = 0f;
                }
            }
        }
    }
}