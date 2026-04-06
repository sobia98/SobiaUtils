using UnityEngine;
using TMPro;

namespace Sobia.Utils
{
    public class FPSDisplay : MonoBehaviour
    {
        [Header("Settings")]
        public bool IsVisible = true;

        [SerializeField] private float UpdateInterval = 0.5f;

        private TextMeshProUGUI FPSText;
        private float UpdateDeltaTime = 0.0f;
        private float UpdateTimer = 0f;

        private void Awake()
        {
            FPSText = GetComponent<TextMeshProUGUI>();
            SobiaUtils.IsAssigned(FPSText, nameof(FPSText), gameObject);
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