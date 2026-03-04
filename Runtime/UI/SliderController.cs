using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

namespace Sobia.Utils
{
    public class SliderController : MonoBehaviour
    {
        [SerializeField] private AudioMixer MyMixer;
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;

        private void Start()
        {
            masterSlider.onValueChanged.AddListener(SetMasterVolume);
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        public void SetMasterVolume(float value)
        {
            MyMixer.SetFloat("MasterVol", Mathf.Log10(value) * 20);
            SaveSettingsPlayerPrefs.SaveMasterVolume(value);
        }

        public void SetMusicVolume(float value)
        {
            MyMixer.SetFloat("MusicVol", Mathf.Log10(value) * 20);
            SaveSettingsPlayerPrefs.SaveMusicVolume(value);
        }

        public void SetSFXVolume(float value)
        {
            MyMixer.SetFloat("SFXVol", Mathf.Log10(value) * 20);
            SaveSettingsPlayerPrefs.SaveSfxVolume(value);
        }
    }
}