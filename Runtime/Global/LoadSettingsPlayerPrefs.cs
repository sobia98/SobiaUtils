using UnityEngine;
using UnityEngine.UI;

namespace Sobia.Utils
{
    public class GlobalSettings : MonoBehaviour
    {
        [SerializeField] private Toggle VSyncToggle;
        [SerializeField] private Toggle FullscreenToggle;
        [SerializeField] private Toggle ShowFPSToggle;
        [SerializeField] private Slider MasterVolumeSlider;
        [SerializeField] private Slider SFXVolumeSlider;
        [SerializeField] private Slider MusicVolumeSlider;
        [SerializeField] private Slider BrightnessVolumeSlider;

        private void Start()
        {
            VSyncToggle.isOn = SaveSettingsPlayerPrefs.LoadVSync();
            FullscreenToggle.isOn = SaveSettingsPlayerPrefs.LoadFullscreen();
            ShowFPSToggle.isOn = SaveSettingsPlayerPrefs.LoadShowFps();
            MasterVolumeSlider.value = SaveSettingsPlayerPrefs.LoadMasterVolume();
            MusicVolumeSlider.value = SaveSettingsPlayerPrefs.LoadMusicVolume();
            SFXVolumeSlider.value = SaveSettingsPlayerPrefs.LoadSfxVolume();
            BrightnessVolumeSlider.value = SaveSettingsPlayerPrefs.LoadBrightness();
        }
    }
}