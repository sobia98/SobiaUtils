using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Sobia.Utils
{
    public class SliderController : MonoBehaviour
    {
        [SerializeField] private AudioMixer MyMixer;
        [SerializeField] private Slider MasterSlider;
        [SerializeField] private Slider MusicSlider;
        [SerializeField] private Slider SfxSlider;
        [SerializeField] private Slider UISlider;

        private void Awake()
        {
            CheckReferences();
        }

        private void Start()
        {
            MasterSlider.onValueChanged.AddListener(SetMasterVolume);
            MusicSlider.onValueChanged.AddListener(SetMusicVolume);
            SfxSlider.onValueChanged.AddListener(SetSFXVolume);
            UISlider.onValueChanged.AddListener(SetUIVolume);
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
            SaveSettingsPlayerPrefs.SaveSFXVolume(value);
        }

        public void SetUIVolume(float value)
        {
            MyMixer.SetFloat("UIVol", Mathf.Log10(value) * 20);
            SaveSettingsPlayerPrefs.SaveUIVolume(value);
        }

        private void CheckReferences()
        {
            SobiaUtils.IsAssigned(MyMixer, nameof(MyMixer), gameObject);
            SobiaUtils.IsAssigned(MasterSlider, nameof(MasterSlider), gameObject);
            SobiaUtils.IsAssigned(MusicSlider, nameof(MusicSlider), gameObject);
            SobiaUtils.IsAssigned(SfxSlider, nameof(SfxSlider), gameObject);
            SobiaUtils.IsAssigned(UISlider, nameof(UISlider), gameObject);
        }
    }
}