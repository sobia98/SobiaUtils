using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Sobia.Utils
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private bool ShouldCheckReferences = false;

        [Header("Audio Mixer Routing")]
        [SerializeField] private AudioMixerGroup SFXMixerGroup;

        [SerializeField] private AudioMixerGroup UIMixerGroup;
        [SerializeField] private AudioMixerGroup MusicMixerGroup;

        [Space(10)]
        [Header("Background Music")]
        [SerializeField] private AudioSource BackgroundSource;

        [SerializeField] private List<AudioClip> BackgroundList;

        [Range(0f, 1f)]
        [SerializeField] private float BackgroundVolume = 0.5f;

        private int CurrentTrackIndex;

        [Space(10)]
        [Header("UI Sound Tuning")]
        [Range(0f, 1f)][SerializeField] private float MinOnHover = 0.2f;

        [Range(0f, 1f)][SerializeField] private float MaxOnHover = 0.45f;
        [Range(0f, 1f)][SerializeField] private float MinOnClick = 0.5f;
        [Range(0f, 1f)][SerializeField] private float MaxOnClick = 0.75f;
        [Range(0f, 1f)][SerializeField] private float MinOnChange = 0.4f;
        [Range(0f, 1f)][SerializeField] private float MaxOnChange = 0.65f;
        [Range(0f, 1f)][SerializeField] private float MinOnUpgrade = 0.45f;
        [Range(0f, 1f)][SerializeField] private float MaxOnUpgrade = 0.65f;

        [Space(10)]
        [Header("Audio Source Pooling")]
        [SerializeField] private AudioSource UISource;

        [SerializeField] private int PoolSize = 10;

        private List<AudioSource> sfxPool = new List<AudioSource>();

        [Space(10)]
        [Header("UI Sounds")]
        [SerializeField] private AudioClip OnValueChangedClip;

        [SerializeField] private AudioClip OnClickClip;
        [SerializeField] private AudioClip OnHoverClip;
        [SerializeField] private AudioClip OnSurfaceCompleted;
        [SerializeField] private AudioClip OnUpgradeClip;

        [Header("OnCompletionObject")]
        [SerializeField] private AudioClip OnCompleitionObjectClip;

        [Range(0f, 1f)]
        [SerializeField] private float OnCompletionObjectVolume = 0.5f;

        [Range(0.5f, 2f)][SerializeField] private float MinOnCompletionObjectPitch = 0.9f;
        [Range(0.5f, 2f)][SerializeField] private float MaxOnCompletionObjectPitch = 1.1f;
        [Range(0.05f, 1.0f)][SerializeField] private float OnCompletionObjectCooldown = 0.2f;

        [Header("OnCompletionSurface")]
        [SerializeField] private AudioClip OnCompleitionSurfaceClip;

        [Range(0f, 1f)]
        [SerializeField] private float OnCompletionSurfaceVolume = 0.5f;

        [Range(0.5f, 2f)][SerializeField] private float MinOnCompletionSurfacePitch = 0.9f;
        [Range(0.5f, 2f)][SerializeField] private float MaxOnCompletionSurfacePitch = 1.05f;
        [Range(0.05f, 1.0f)][SerializeField] private float OnCompletionSurfaceCooldown = 0.2f;

        [Header("OnObjectToBarBeginn")]
        [SerializeField] private AudioClip OnObjectToBarBeginnClip;

        [Range(0f, 1f)]
        [SerializeField] private float OnObjectToBarBeginnVolume = 0.3f;

        [Range(0.5f, 2f)][SerializeField] private float MinOnObjectToBarBeginnPitch = 1.4f;
        [Range(0.5f, 2f)][SerializeField] private float MaxOnObjectToBarBeginnPitch = 1.6f;
        [Range(0.05f, 1.0f)][SerializeField] private float OnObjectToBarBeginnCooldown = 0.2f;

        [Header("OnObjectToBarEnd")]
        [SerializeField] private AudioClip OnObjectToBarEndClip;

        [Range(0f, 1f)]
        [SerializeField] private float OnObjectToBarEndVolume = 0.3f;

        [Range(0.5f, 2f)][SerializeField] private float MinOnObjectToBarEndPitch = 1.4f;
        [Range(0.5f, 2f)][SerializeField] private float MaxOnObjectToBarEndPitch = 1.6f;
        [Range(0.05f, 1.0f)][SerializeField] private float OnObjectToBarEndCooldown = 0.2f;

        [Header("OnSurfaceToObject")]
        [SerializeField] private AudioClip OnSurfaceToObjectClip;

        [Range(0f, 1f)]
        [SerializeField] private float OnSurfaceToObjectVolume = 0.15f;

        [Range(0.5f, 2f)][SerializeField] private float MinOnSurfaceToObjectPitch = 0.85f;
        [Range(0.5f, 2f)][SerializeField] private float MaxOnSurfaceToObjectPitch = 1.05f;
        [Range(0.05f, 1.0f)][SerializeField] private float OnSurfaceToObjectCooldown = 0.2f;

        [Header("OnCollectCoin")]
        [SerializeField] private AudioClip OnCollectCoinClip;

        [Range(0f, 1f)]
        [SerializeField] private float OnCollectCoinVolume = 0.172f;

        [Range(0.5f, 2f)][SerializeField] private float MinOnCollectCoinPitch = 0.955f;
        [Range(0.5f, 2f)][SerializeField] private float MaxOnCollectCoinPitch = 1.05f;
        [Range(0.05f, 1.0f)][SerializeField] private float OnCollectCoinCooldown = 0.2f;

        [Header("OnHideObjectUI")]
        [SerializeField] private AudioClip OnHideObjectUIClip;

        [Range(0f, 1f)]
        [SerializeField] private float OnHideObjectUIVolume = 0.172f;

        [Range(0.5f, 2f)][SerializeField] private float MinOnHideObjectUIPitch = 0.955f;
        [Range(0.5f, 2f)][SerializeField] private float MaxOnHideObjectUIPitch = 1.05f;
        [Range(0.05f, 1.0f)][SerializeField] private float OnHideObjectUICooldown = 0.2f;

        [Header("OnShowObjectUI")]
        [SerializeField] private AudioClip OnShowObjectUIClip;

        [Range(0f, 1f)]
        [SerializeField] private float OnShowObjectUIVolume = 0.172f;

        [Range(0.5f, 2f)][SerializeField] private float MinOnShowObjectUIPitch = 0.955f;
        [Range(0.5f, 2f)][SerializeField] private float MaxOnShowObjectUIPitch = 1.05f;
        [Range(0.05f, 1.0f)][SerializeField] private float OnShowObjectUICooldown = 0.2f;

        private Dictionary<AudioClip, float> lastPlayTimes = new Dictionary<AudioClip, float>();
        public static AudioManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializePool();

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

        // ==========================================
        // AUDIO POOL MANAGEMENT
        // ==========================================
        private void InitializePool()
        {
            GameObject poolContainer = new GameObject("SFX_Pool");
            poolContainer.transform.SetParent(transform);

            for (int i = 0; i < PoolSize; i++)
            {
                AudioSource newSource = poolContainer.AddComponent<AudioSource>();
                newSource.playOnAwake = false;

                // ASSIGN MIXER GROUP HERE
                if (SFXMixerGroup != null)
                {
                    newSource.outputAudioMixerGroup = SFXMixerGroup;
                }

                sfxPool.Add(newSource);
            }
        }

        private AudioSource GetAvailableSFXSource()
        {
            foreach (var source in sfxPool)
            {
                if (!source.isPlaying) return source;
            }

            // Fallback: If all sources are busy, grab the first one
            return sfxPool[0];
        }

        public void PlayCompletionObjectSound()
        {
            PlaySFX(OnCompleitionObjectClip, OnCompletionObjectVolume, MinOnCompletionObjectPitch, MaxOnCompletionObjectPitch, OnCompletionObjectCooldown);
        }

        public void PlayCompletionSurfaceSound()
        {
            PlaySFX(OnCompleitionSurfaceClip, OnCompletionSurfaceVolume, MinOnCompletionSurfacePitch, MaxOnCompletionSurfacePitch, OnCompletionSurfaceCooldown);
        }

        public void PlayObjectToBarBeginnSound()
        {
            PlaySFX(OnObjectToBarBeginnClip, OnObjectToBarBeginnVolume, MinOnObjectToBarBeginnPitch, MaxOnObjectToBarBeginnPitch, OnObjectToBarBeginnCooldown);
        }

        public void PlayObjectToBarEndSound()
        {
            PlaySFX(OnObjectToBarEndClip, OnObjectToBarEndVolume, MinOnObjectToBarEndPitch, MaxOnObjectToBarEndPitch, OnObjectToBarEndCooldown);
        }

        public void PlaySurfaceToObjectSound()
        {
            PlaySFX(OnSurfaceToObjectClip, OnSurfaceToObjectVolume, MinOnSurfaceToObjectPitch, MaxOnSurfaceToObjectPitch, OnSurfaceToObjectCooldown);
        }

        public void PlayCollectCoinSound()
        {
            PlaySFX(OnCollectCoinClip, OnCollectCoinVolume, MinOnCollectCoinPitch, MaxOnCollectCoinPitch, OnCollectCoinCooldown);
        }

        // Helper to play any gameplay sound with custom pitch/volume without clashing
        private void PlaySFX(AudioClip clip, float volume = 1.0f, float minPitch = 0.9f, float maxPitch = 1.1f, float cooldown = 0.1f)
        {
            if (clip == null) return;

            // Check if the clip was played too recently
            if (lastPlayTimes.TryGetValue(clip, out float lastTime))
            {
                if (Time.time - lastTime < cooldown) return; // Block playback
            }

            // Update the last play time for this clip
            lastPlayTimes[clip] = Time.time;

            AudioSource source = GetAvailableSFXSource();
            source.pitch = Random.Range(minPitch, maxPitch);
            source.PlayOneShot(clip, volume);
            Debug.Log($"Playing SFX: {clip.name} at volume {volume} and pitch {source.pitch}");
        }

        // ==========================================
        // BACKGROUND MUSIC
        // ==========================================
        [ContextMenu("Play Next Track")]
        private void PlayNextTrack()
        {
            CurrentTrackIndex = (CurrentTrackIndex + 1) % BackgroundList.Count;
            PlayTrack(CurrentTrackIndex);
        }

        private void PlayTrack(int index)
        {
            BackgroundSource.volume = BackgroundVolume;
            BackgroundSource.clip = BackgroundList[index];
            BackgroundSource.Play();
            Invoke(nameof(PlayNextTrack), BackgroundList[index].length);
        }

        private void OnValidate()
        {
            // Instantly update the playing background source volume when slider changes in Inspector
            if (BackgroundSource != null)
            {
            }
            BackgroundSource.volume = BackgroundVolume;
        }

        // ==========================================
        // GAMEPLAY SFX
        // ==========================================
        public IEnumerator PlayDelayedSound(AudioClip clip)
        {
            yield return new WaitForSeconds(Random.Range(0f, 0.15f));
            PlaySFX(clip, 0.15f, 0.9f, 1.1f);
        }

        // ==========================================
        // UI SOUNDS
        // ==========================================
        public void PlayUIOnClickSound()
        {
            UISource.PlayOneShot(OnClickClip, Random.Range(MinOnClick, MaxOnClick));
        }

        public void PlayUIOnHoverSound()
        {
            UISource.PlayOneShot(OnHoverClip, Random.Range(MinOnHover, MaxOnHover));
        }

        public void PlayUIOnUpgradeSound()
        {
            UISource.PlayOneShot(OnUpgradeClip, Random.Range(MinOnHover, MaxOnHover));
        }

        public void PlayUIOnValueChangedSound()
        {
            UISource.PlayOneShot(OnValueChangedClip, Random.Range(MinOnChange, MaxOnChange));
        }

        public void PlayUIOnHideObjectUISound()
        {
            UISource.PlayOneShot(OnHideObjectUIClip, OnHideObjectUIVolume);
        }

        public void PlayUIOnShowObjectUISound()
        {
            UISource.PlayOneShot(OnShowObjectUIClip, OnShowObjectUIVolume);
        }

        // ==========================================
        // UTILITIES
        // ==========================================
        public IEnumerator FadeOutAndStop(AudioSource source, float duration)
        {
            float startVolume = source.volume;
            float timeElapsed = 0f;

            while (timeElapsed < duration)
            {
                timeElapsed += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, 0f, timeElapsed / duration);
                yield return null;
            }

            source.volume = 0f;
            source.Stop();
            source.volume = startVolume;
        }

        private void CheckReferences()
        {
            SobiaUtils.IsAssigned(BackgroundSource, nameof(BackgroundSource), gameObject);
            SobiaUtils.IsAssigned(UISource, nameof(UISource), gameObject);
            SobiaUtils.IsAssigned(OnValueChangedClip, nameof(OnValueChangedClip), gameObject);
            SobiaUtils.IsAssigned(OnClickClip, nameof(OnClickClip), gameObject);
            SobiaUtils.IsAssigned(OnHoverClip, nameof(OnHoverClip), gameObject);
        }
    }
}