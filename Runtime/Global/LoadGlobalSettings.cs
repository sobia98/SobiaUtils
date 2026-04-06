using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sobia.Utils
{
    public class LoadGlobalSettings : MonoBehaviour
    {
        [SerializeField] private bool ShouldCheckReferences = true;

        //Audio
        [SerializeField] private Slider MasterSlider;

        [SerializeField] private Slider SFXSlider;
        [SerializeField] private Slider MusicSlider;
        [SerializeField] private Slider UISlider;

        //Graphics
        [SerializeField] private Slider BrightnessSlider;

        [SerializeField] private Slider FOVSlider;
        [SerializeField] private Toggle ShowFPSToggle;
        [SerializeField] private TMP_Dropdown TextureQualityDropdown;
        [SerializeField] private Toggle VSyncToggle;
        [SerializeField] private TMP_Dropdown ResolutionDropdown;
        [SerializeField] private Slider MaxFramerateSlider;
        [SerializeField] private Toggle FullscreenToggle;

        //General
        [SerializeField] private Toggle InverMouseXToggle;

        [SerializeField] private Toggle InverMouseYToggle;
        [SerializeField] private Slider SensitivitySlider;

        private void Awake()
        {
            if (ShouldCheckReferences)
            {
                //Audio
                SobiaUtils.IsAssigned(MasterSlider, nameof(MasterSlider), gameObject);
                SobiaUtils.IsAssigned(SFXSlider, nameof(SFXSlider), gameObject);
                SobiaUtils.IsAssigned(MusicSlider, nameof(MusicSlider), gameObject);
                SobiaUtils.IsAssigned(UISlider, nameof(UISlider), gameObject);
                //Graphics
                SobiaUtils.IsAssigned(BrightnessSlider, nameof(BrightnessSlider), gameObject);
                SobiaUtils.IsAssigned(FOVSlider, nameof(FOVSlider), gameObject);
                SobiaUtils.IsAssigned(ShowFPSToggle, nameof(ShowFPSToggle), gameObject);
                SobiaUtils.IsAssigned(TextureQualityDropdown, nameof(TextureQualityDropdown), gameObject);
                SobiaUtils.IsAssigned(VSyncToggle, nameof(VSyncToggle), gameObject);
                SobiaUtils.IsAssigned(ResolutionDropdown, nameof(ResolutionDropdown), gameObject);
                SobiaUtils.IsAssigned(MaxFramerateSlider, nameof(MaxFramerateSlider), gameObject);
                SobiaUtils.IsAssigned(FullscreenToggle, nameof(FullscreenToggle), gameObject);
                //General
                SobiaUtils.IsAssigned(InverMouseXToggle, nameof(InverMouseXToggle), gameObject);
                SobiaUtils.IsAssigned(InverMouseYToggle, nameof(InverMouseYToggle), gameObject);
                SobiaUtils.IsAssigned(SensitivitySlider, nameof(SensitivitySlider), gameObject);
            }
        }

        private void Start()
        {
            if (ShouldCheckReferences)
            {
                //Audio
                MasterSlider.value = SaveSettingsPlayerPrefs.LoadMasterVolume();
                SFXSlider.value = SaveSettingsPlayerPrefs.LoadSFXVolume();
                MusicSlider.value = SaveSettingsPlayerPrefs.LoadMusicVolume();
                UISlider.value = SaveSettingsPlayerPrefs.LoadUIVolume();
                //Graphics
                BrightnessSlider.value = SaveSettingsPlayerPrefs.LoadBrightness();
                FOVSlider.value = SaveSettingsPlayerPrefs.LoadFOV();
                ShowFPSToggle.isOn = SaveSettingsPlayerPrefs.LoadShowFps();
                TextureQualityDropdown.value = SaveSettingsPlayerPrefs.LoadTextureQuality();
                VSyncToggle.isOn = SaveSettingsPlayerPrefs.LoadVSync();
                ResolutionDropdown.value = SaveSettingsPlayerPrefs.LoadResolution();
                MaxFramerateSlider.value = SaveSettingsPlayerPrefs.LoadMaxFramerate();
                FullscreenToggle.isOn = SaveSettingsPlayerPrefs.LoadFullscreen();
                //General
                InverMouseXToggle.isOn = SaveSettingsPlayerPrefs.LoadInvertMouseX();
                InverMouseYToggle.isOn = SaveSettingsPlayerPrefs.LoadInvertMouseY();
                SensitivitySlider.value = SaveSettingsPlayerPrefs.LoadSensitivity();
            }
        }
    }
}