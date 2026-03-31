using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sobia.Utils
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private bool ShouldCheckReferences = false;

        [Header("Audio Sources")]
        [SerializeField] private AudioSource SFXSource;

        [SerializeField] private AudioSource CollectSFXSource;
        [SerializeField] private AudioSource BackgroundSource;
        [SerializeField] private AudioSource UISource;

        [Header("Background Music")]
        [SerializeField] private List<AudioClip> BackgroundList;

        private int CurrentTrackIndex;

        [Header("UI Sounds")]
        [SerializeField] private AudioClip OnValueChangedClip;

        [SerializeField] private AudioClip OnClickClip;
        [SerializeField] private AudioClip OnHoverClip;

        private float LastPlayTime;

        public static AudioManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            if (ShouldCheckReferences)
            {
                CheckReferences();
            }
        }

        private void Start()
        {
            if (BackgroundList.Count > 0)
            {
                CurrentTrackIndex = Random.Range(0, BackgroundList.Count);
                PlayTrack(CurrentTrackIndex);
            }
        }

        private void PlayNextTrack()
        {
            CurrentTrackIndex = (CurrentTrackIndex + 1) % BackgroundList.Count;
            PlayTrack(CurrentTrackIndex);
        }

        private void PlayTrack(int index)
        {
            BackgroundSource.clip = BackgroundList[index];
            BackgroundSource.Play();

            // Call the next track automatically when this one ends
            Invoke("PlayNextTrack", BackgroundList[index].length);
        }

        public IEnumerator PlayDelayedSound(AudioClip clip)
        {
            yield return new WaitForSeconds(Random.Range(0f, 0.15f));
            CollectSFXSource.volume = 0.15f;
            CollectSFXSource.pitch = Random.Range(0.9f, 1.1f);
            CollectSFXSource.PlayOneShot(clip);
        }

        public void PlayUIOnClickSound()
        {
            UISource.pitch = Random.Range(0.95f, 1.05f);
            UISource.PlayOneShot(OnClickClip);
        }

        public void PlayUIOnHoverSound()
        {
            UISource.pitch = Random.Range(0.95f, 1.05f);
            UISource.PlayOneShot(OnHoverClip);
        }

        public void PlayUIOnValueChangedSound()
        {
            UISource.pitch = Random.Range(0.95f, 1.05f);
            UISource.PlayOneShot(OnValueChangedClip);
        }

        public IEnumerator FadeOutAndStop(AudioSource source, float duration)
        {
            float startVolume = source.volume;
            float timeElapsed = 0f;

            // Gradually reduce the volume
            while (timeElapsed < duration)
            {
                timeElapsed += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, 0f, timeElapsed / duration);
                yield return null;
            }

            source.volume = 0f;
            source.Stop();

            //start at startvolume next time
            source.volume = startVolume;
        }

        public void PlayCustomOneShot(AudioClip audioClip, float minTimeBetweenSounds)
        {
            if (Time.time - LastPlayTime < minTimeBetweenSounds) return;

            SFXSource.pitch = Random.Range(0.85f, 1.05f);

            SFXSource.PlayOneShot(audioClip);

            LastPlayTime = Time.time;
        }

        private void CheckReferences()
        {
            SobiaUtils.IsAssigned(SFXSource, nameof(SFXSource), gameObject);
            SobiaUtils.IsAssigned(CollectSFXSource, nameof(CollectSFXSource), gameObject);
            SobiaUtils.IsAssigned(BackgroundSource, nameof(BackgroundSource), gameObject);
            SobiaUtils.IsAssigned(UISource, nameof(UISource), gameObject);
            SobiaUtils.IsAssigned(OnValueChangedClip, nameof(OnValueChangedClip), gameObject);
            SobiaUtils.IsAssigned(OnClickClip, nameof(OnClickClip), gameObject);
            SobiaUtils.IsAssigned(OnHoverClip, nameof(OnHoverClip), gameObject);
        }
    }
}