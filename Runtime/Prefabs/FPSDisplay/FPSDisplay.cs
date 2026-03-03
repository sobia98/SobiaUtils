using UnityEngine;
using TMPro;

namespace Sobia.Utils
{
    public class FPSDisplay : MonoBehaviour
    {
        [Header("Setup")]
        public TextMeshProUGUI FPSText;

        [Header("Settings")]
        [SerializeField] private float UpdateInterval = 0.5f;

        public bool IsVisible = true;

        private float UpdateDeltaTime = 0.0f;
        private float UpdateTimer = 0f;

        private void Start()
        {
            if (FPSText == null)
            {
                Debug.LogError($"[FPSDisplay] Missing TextMeshPro reference on {gameObject.name}!");
                enabled = false;
                return;
            }
            FPSText.gameObject.SetActive(IsVisible);

            Debug.Log(TMP_Settings.defaultFontAsset);
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