using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sobia.Utils
{
    public class ToggleController : MonoBehaviour
    {
        [SerializeField] private Toggle VSyncToggle;
        [SerializeField] private Toggle FullscreenToggle;
        [SerializeField] private Toggle ShowFPSToggle;

        [SerializeField] private TextMeshProUGUI FPSText;
        [SerializeField] private FPSDisplay FPSDisplay;

        private void Awake()
        {
            CheckReferences();
        }

        private void Start()
        {
            VSyncToggle.onValueChanged.AddListener(ToggleVSync);
            FullscreenToggle.onValueChanged.AddListener(ToggleFullscreen);
            ShowFPSToggle.onValueChanged.AddListener(ToggleFPS);
        }

        public void ToggleVSync(bool isOn)
        {
            QualitySettings.vSyncCount = isOn ? 1 : 0;
            SaveSettingsPlayerPrefs.SaveVSync(isOn);
        }

        public void ToggleFullscreen(bool isOn)
        {
            Screen.fullScreen = isOn;
            SaveSettingsPlayerPrefs.SaveFullscreen(isOn);
        }

        public void ToggleFPS(bool isOn)
        {
            FPSDisplay.IsVisible = isOn;
            FPSText.gameObject.SetActive(isOn);
            SaveSettingsPlayerPrefs.SaveShowFps(isOn);
        }

        private void CheckReferences()
        {
            SobiaUtils.IsAssigned(VSyncToggle, nameof(VSyncToggle), gameObject);
            SobiaUtils.IsAssigned(FullscreenToggle, nameof(FullscreenToggle), gameObject);
            SobiaUtils.IsAssigned(ShowFPSToggle, nameof(ShowFPSToggle), gameObject);
            SobiaUtils.IsAssigned(FPSText, nameof(FPSText), gameObject);
            SobiaUtils.IsAssigned(FPSDisplay, nameof(FPSDisplay), gameObject);
        }
    }
}