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
        private Coroutine musicCoroutine;

        [Header("OnClickUI")]
        [SerializeField] private AudioClip OnClickClip;

        [Range(0f, 1f)]
        [SerializeField] private float OnClickVolume = 0.6f;

        [Range(0.5f, 2f)][SerializeField] private float MinOnClickPitch = 0.95f;
        [Range(0.5f, 2f)][SerializeField] private float MaxOnClickPitch = 1.05f;
        [Range(0.05f, 1.0f)][SerializeField] private float OnClickCooldown = 0.1f;

        [Header("OnStartGame")]
        [SerializeField] private AudioClip OnStartGameClip;

        [Range(0f, 1f)]
        [SerializeField] private float OnStartGameVolume = 0.6f;

        [Range(0.5f, 2f)][SerializeField] private float MinOnStartGamePitch = 0.95f;
        [Range(0.5f, 2f)][SerializeField] private float MaxOnStartGamePitch = 1.05f;
        [Range(0.05f, 1.0f)][SerializeField] private float OnStartGameCooldown = 0.1f;

        [Header("OnBackMainMenu")]
        [SerializeField] private AudioClip OnBackMainMenuClip;

        [Range(0f, 1f)]
        [SerializeField] private float OnBackMainMenuVolume = 0.6f;

        [Range(0.5f, 2f)][SerializeField] private float MinOnBackMainMenuPitch = 0.95f;
        [Range(0.5f, 2f)][SerializeField] private float MaxOnBackMainMenuPitch = 1.05f;
        [Range(0.05f, 1.0f)][SerializeField] private float OnBackMainMenuCooldown = 0.1f;

        [Header("OnPauseGame")]
        [SerializeField] private AudioClip OnPauseGameClip;

        [Range(0f, 1f)]
        [SerializeField] private float OnPauseGameVolume = 0.6f;

        [Range(0.5f, 2f)][SerializeField] private float MinOnPauseGamePitch = 0.95f;
        [Range(0.5f, 2f)][SerializeField] private float MaxOnPauseGamePitch = 1.05f;
        [Range(0.05f, 1.0f)][SerializeField] private float OnPauseGameCooldown = 0.1f;

        [Header("OnOpenGame")]
        [SerializeField] private AudioClip OnOpenGameClip;

        [SerializeField] private AudioClip OnOpenGameClip2;

        [Range(0f, 1f)]
        [SerializeField] private float OnOpenGameVolume = 0.6f;

        [Range(0.5f, 2f)][SerializeField] private float MinOnOpenGamePitch = 0.95f;
        [Range(0.5f, 2f)][SerializeField] private float MaxOnOpenGamePitch = 1.05f;
        [Range(0.05f, 1.0f)][SerializeField] private float OnOpenGameCooldown = 0.1f;

        [Header("OnExitGame")]
        [SerializeField] private AudioClip OnExitGameClip;

        [Range(0f, 1f)]
        [SerializeField] private float OnExitGameVolume = 0.6f;

        [Range(0.5f, 2f)][SerializeField] private float MinOnExitGamePitch = 0.95f;
        [Range(0.5f, 2f)][SerializeField] private float MaxOnExitGamePitch = 1.05f;
        [Range(0.05f, 1.0f)][SerializeField] private float OnExitGameCooldown = 0.1f;

        [Header("OnTabOpen")]
        [SerializeField] private AudioClip OnTabOpenClip;

        [Range(0f, 1f)]
        [SerializeField] private float OnTabOpenVolume = 0.6f;

        [Range(0.5f, 2f)][SerializeField] private float MinOnTabOpenPitch = 0.95f;
        [Range(0.5f, 2f)][SerializeField] private float MaxOnTabOpenPitch = 1.05f;
        [Range(0.05f, 1.0f)][SerializeField] private float OnTabOpenCooldown = 0.1f;

        [Header("OnHoverUI")]
        [SerializeField] private AudioClip OnHoverClip;

        [Range(0f, 1f)]
        [SerializeField] private float OnHoverVolume = 0.3f;

        [Range(0.5f, 2f)][SerializeField] private float MinOnHoverPitch = 0.95f;
        [Range(0.5f, 2f)][SerializeField] private float MaxOnHoverPitch = 1.05f;
        [Range(0.05f, 1.0f)][SerializeField] private float OnHoverCooldown = 0.05f;

        [Header("OnUpgradeUI")]
        [SerializeField] private AudioClip OnUpgradeClip;

        [Range(0f, 1f)]
        [SerializeField] private float OnUpgradeVolume = 0.6f;

        [Range(0.5f, 2f)][SerializeField] private float MinOnUpgradePitch = 0.95f;
        [Range(0.5f, 2f)][SerializeField] private float MaxOnUpgradePitch = 1.05f;
        [Range(0.05f, 1.0f)][SerializeField] private float OnUpgradeCooldown = 0.1f;

        [Header("OnValueChangedUI")]
        [SerializeField] private AudioClip OnValueChangedClip;

        [Range(0f, 1f)]
        [SerializeField] private float OnChangeVolume = 0.5f;

        [Range(0.5f, 2f)][SerializeField] private float MinOnChangePitch = 0.95f;
        [Range(0.5f, 2f)][SerializeField] private float MaxOnChangePitch = 1.05f;
        [Range(0.05f, 1.0f)][SerializeField] private float OnChangeCooldown = 0.05f;

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

        [Space(10)]
        [Header("Audio Source Pooling")]
        [SerializeField] private AudioSource UISource;

        [SerializeField] private int PoolSize = 10;

        private List<AudioSource> sfxPool = new List<AudioSource>();

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

        [Header("OnEmission")]
        [SerializeField] private AudioClip OnEmissionClip;

        [Range(0f, 1f)]
        [SerializeField] private float OnEmissionVolume = 0.172f;

        [Range(0.5f, 2f)][SerializeField] private float MinOnEmissionPitch = 0.955f;
        [Range(0.5f, 2f)][SerializeField] private float MaxOnEmissionPitch = 1.05f;
        [Range(0.05f, 1.0f)][SerializeField] private float OnEmissionCooldown = 0.2f;

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

        public void PlayShowObjectUI()
        {
            PlaySFX(OnShowObjectUIClip, OnShowObjectUIVolume, MinOnShowObjectUIPitch, MaxOnShowObjectUIPitch, OnShowObjectUICooldown);
        }

        public void PlayHideObjectUI()
        {
            PlaySFX(OnHideObjectUIClip, OnHideObjectUIVolume, MinOnHideObjectUIPitch, MaxOnHideObjectUIPitch, OnHideObjectUICooldown);
        }

        public void PlayEmissionSound()
        {
            PlaySFX(OnEmissionClip, OnEmissionVolume, MinOnEmissionPitch, MaxOnEmissionPitch, OnEmissionCooldown);
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
            // Stop any existing music timer coroutine
            if (musicCoroutine != null)
            {
                StopCoroutine(musicCoroutine);
            }

            BackgroundSource.volume = BackgroundVolume;
            BackgroundSource.clip = BackgroundList[index];
            BackgroundSource.Play();

            // Start a coroutine to wait until the track ends
            musicCoroutine = StartCoroutine(WaitAndPlayNextTrack(BackgroundList[index].length));
        }

        private IEnumerator WaitAndPlayNextTrack(float trackLength)
        {
            yield return new WaitForSeconds(trackLength);
            PlayNextTrack();
        }

        private void OnValidate()
        {
            // Safely update volume in Editor without causing serialization resets on Undo
            if (BackgroundSource != null)
            {
                BackgroundSource.volume = BackgroundVolume;

                // Restores playback if Unity's undo system paused the source
                if (Application.isPlaying && !BackgroundSource.isPlaying && BackgroundSource.clip != null)
                {
                    BackgroundSource.Play();
                }
            }
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
            if (OnClickClip == null || UISource == null) return;
            UISource.pitch = Random.Range(MinOnClickPitch, MaxOnClickPitch);
            UISource.PlayOneShot(OnClickClip, OnClickVolume);
        }

        public void PlayUIOnOpenGameSound()
        {
            if (OnOpenGameClip == null || OnOpenGameClip2 == null || UISource == null) return;
            float randomPitch = Random.Range(MinOnOpenGamePitch, MaxOnOpenGamePitch);
            bool playFirstClip = Random.value > 0.5f;
            if (playFirstClip)
            {
                PlayClipWithPitch(OnOpenGameClip, Camera.main.transform.position, OnExitGameVolume, randomPitch);
            }
            else
            {
                PlayClipWithPitch(OnOpenGameClip2, Camera.main.transform.position, OnExitGameVolume, randomPitch);
            }
        }

        public void PlayUIOnStartGameSound()
        {
            if (OnStartGameClip == null || UISource == null) return;
            UISource.pitch = Random.Range(MinOnStartGamePitch, MaxOnStartGamePitch);
            UISource.PlayOneShot(OnStartGameClip, OnStartGameVolume);
        }

        public void PlayUIOnBackMainMenuSound()
        {
            if (OnBackMainMenuClip == null || UISource == null) return;
            UISource.pitch = Random.Range(MinOnBackMainMenuPitch, MaxOnBackMainMenuPitch);
            UISource.PlayOneShot(OnBackMainMenuClip, OnBackMainMenuVolume);
        }

        public void PlayUIOnPauseGame()
        {
            if (OnPauseGameClip == null || UISource == null) return;
            UISource.pitch = Random.Range(MinOnPauseGamePitch, MaxOnPauseGamePitch);
            UISource.PlayOneShot(OnPauseGameClip, OnPauseGameVolume);
        }

        public void PlayUIOnExitGame()
        {
            if (OnExitGameClip == null) return;

            // 1. Calculate random pitch
            float randomPitch = Random.Range(MinOnExitGamePitch, MaxOnExitGamePitch);

            // 2. Play clip via a temporary GameObject at the main camera position
            PlayClipWithPitch(OnExitGameClip, Camera.main.transform.position, OnExitGameVolume, randomPitch);
        }

        private void PlayClipWithPitch(AudioClip clip, Vector3 position, float volume, float pitch)
        {
            GameObject tempGO = new GameObject("TempAudio");
            tempGO.transform.position = position;

            AudioSource aSource = tempGO.AddComponent<AudioSource>();
            aSource.clip = clip;
            aSource.volume = volume;
            aSource.pitch = pitch;
            aSource.spatialBlend = 0f; // 2D sound for UI

            aSource.Play();

            // Automatically destroy after the clip finishes
            Destroy(tempGO, clip.length / Mathf.Max(0.1f, pitch));
        }

        public void PlayUIOnTabOpenSound()
        {
            if (OnTabOpenClip == null || UISource == null) return;
            UISource.pitch = Random.Range(MinOnTabOpenPitch, MaxOnTabOpenPitch);
            UISource.PlayOneShot(OnTabOpenClip, OnTabOpenVolume);
        }

        public void PlayUIOnHoverSound()
        {
            if (OnHoverClip == null || UISource == null) return;
            UISource.pitch = Random.Range(MinOnHoverPitch, MaxOnHoverPitch);
            UISource.PlayOneShot(OnHoverClip, OnHoverVolume);
        }

        public void PlayUIOnUpgradeSound()
        {
            if (OnUpgradeClip == null || UISource == null) return;
            UISource.pitch = Random.Range(MinOnUpgradePitch, MaxOnUpgradePitch);
            UISource.PlayOneShot(OnUpgradeClip, OnUpgradeVolume);
        }

        public void PlayUIOnValueChangedSound()
        {
            if (OnValueChangedClip == null || UISource == null) return;
            UISource.pitch = Random.Range(MinOnChangePitch, MaxOnChangePitch);
            UISource.PlayOneShot(OnValueChangedClip, OnChangeVolume);
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