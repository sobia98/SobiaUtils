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

        [Range(0f, 1f)]
        [SerializeField] private float DuckMultiplier = 0.2f; // Lowers volume to 20% while a

        private bool isDucked = false;

        private readonly Dictionary<AudioSource, float> originalSourceVolumes = new Dictionary<AudioSource, float>();

        [Space(10)]
        [Header("Background Music")]
        [SerializeField] private AudioSource BackgroundSource;

        [SerializeField] private List<AudioClip> BackgroundList;

        [Range(0f, 1f)]
        [SerializeField] private float BackgroundVolume = 0.5f;

        [SerializeField] private float fadeDuration = 1.5f;

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

        [Header("OnUpgradeDenialUI")]
        [SerializeField] private AudioClip OnUpgradeDenialClip;

        [Range(0f, 1f)]
        [SerializeField] private float OnUpgradeDenialVolume = 0.6f;

        [Range(0.5f, 2f)][SerializeField] private float MinOnUpgradeDenialPitch = 0.95f;
        [Range(0.5f, 2f)][SerializeField] private float MaxOnUpgradeDenialPitch = 1.05f;
        [Range(0.05f, 1.0f)][SerializeField] private float OnUpgradeDenialCooldown = 0.1f;

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

        [Header("OnUnlockStat")]
        [SerializeField] private AudioClip[] OnUnlockStatClips;

        [Range(0f, 1f)]
        [SerializeField] private float OnUnlockStatVolume = 0.172f;

        [Range(0.5f, 2f)][SerializeField] private float MinOnUnlockStatPitch = 0.955f;
        [Range(0.5f, 2f)][SerializeField] private float MaxOnUnlockStatPitch = 1.05f;
        [Range(0.05f, 1.0f)][SerializeField] private float OnUnlockStatCooldown = 0.2f;

        [Header("OnUpgradeAvailable")]
        [SerializeField] private AudioClip[] OnUpgradeAvailableClips;

        [Range(0f, 1f)]
        [SerializeField] private float OnUpgradeAvailableVolume = 0.172f;

        [Range(0.5f, 2f)][SerializeField] private float MinOnUpgradeAvailablePitch = 0.955f;
        [Range(0.5f, 2f)][SerializeField] private float MaxOnUpgradeAvailablePitch = 1.05f;
        [Range(0.05f, 1.0f)][SerializeField] private float OnOnUpgradeAvailableCooldown = 0.2f;

        [Header("OnHoldDelete")]
        [SerializeField] private AudioClip OnHoldDeleteClip;

        [Range(0f, 1f)]
        [SerializeField] private float OnHoldDeleteVolume = 0.172f;

        [Range(0.5f, 2f)][SerializeField] private float MinOnHoldDeletePitch = 0.955f;
        [Range(0.5f, 2f)][SerializeField] private float MaxOnHoldDeletePitch = 1.05f;
        [Range(0.05f, 1.0f)][SerializeField] private float OnOnHoldDeleteCooldown = 0.2f;
        private AudioSource HoldDeleteAudioSource;

        [Header("OnAfterDelete")]
        [SerializeField] private AudioClip OnAfterDeleteClip;

        [Range(0f, 1f)]
        [SerializeField] private float OnAfterDeleteVolume = 0.172f;

        [Range(0.5f, 2f)][SerializeField] private float MinOnAfterDeletePitch = 0.955f;
        [Range(0.5f, 2f)][SerializeField] private float MaxOnAfterDeletePitch = 1.05f;
        [Range(0.05f, 1.0f)][SerializeField] private float OnOnAfterDeleteCooldown = 0.2f;

        private Coroutine demoFinishedCoroutine;

        private AudioSource demoAudioSource;

        [Header("Demo Finished")]
        [SerializeField] private AudioClip OnDemoFinishedClip;

        [Range(0f, 1f)]
        [SerializeField] private float OnDemoFinishedVolume = 0.172f;

        [Range(0.5f, 2f)][SerializeField] private float MinOnDemoFinishedPitch = 0.955f;
        [Range(0.5f, 2f)][SerializeField] private float MaxOnDemoFinishedPitch = 1.05f;
        [Range(0.05f, 1.0f)][SerializeField] private float OnDemoFinishedCooldown = 0.2f;

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

        public void PlayOnAfterDeleteSound()
        {
            PlaySFX(OnAfterDeleteClip, OnAfterDeleteVolume, MinOnAfterDeletePitch, MaxOnAfterDeletePitch, OnOnAfterDeleteCooldown);
        }

        public void StartDeleteHoldSound()
        {
            if (HoldDeleteAudioSource == null)
            {
                GameObject holdObj = new GameObject("Hold_AudioSource");
                holdObj.transform.SetParent(transform);
                HoldDeleteAudioSource = holdObj.AddComponent<AudioSource>();
                HoldDeleteAudioSource.playOnAwake = false;
                if (UIMixerGroup != null) HoldDeleteAudioSource.outputAudioMixerGroup = UIMixerGroup;
            }

            HoldDeleteAudioSource.clip = OnHoldDeleteClip;
            HoldDeleteAudioSource.volume = OnHoldDeleteVolume;
            HoldDeleteAudioSource.pitch = Random.Range(MinOnHoldDeletePitch, MaxOnHoldDeletePitch);
            HoldDeleteAudioSource.time = 0f;
            HoldDeleteAudioSource.Play();
        }

        public void StopDeleteHoldSound()
        {
            if (HoldDeleteAudioSource != null && HoldDeleteAudioSource.isPlaying)
            {
                HoldDeleteAudioSource.Stop(); // Resets playhead back to 0
            }
        }

        private void TriggerSpecialSound(AudioClip clip, float minPitch, float maxPitch, float volume)
        {
            if (clip == null) return;

            if (demoFinishedCoroutine != null)
            {
                StopCoroutine(demoFinishedCoroutine);
            }

            demoFinishedCoroutine = StartCoroutine(PlayDemoFinishedRoutine(clip, minPitch, maxPitch, volume));
        }

        public void PlayUpgradeAvailableSound()
        {
            if (OnUpgradeAvailableClips == null || OnUpgradeAvailableClips.Length == 0) return;
            AudioClip selectedClip = OnUpgradeAvailableClips[Random.Range(0, OnUpgradeAvailableClips.Length)];
            TriggerSpecialSound(selectedClip, MinOnUpgradeAvailablePitch, MaxOnUpgradeAvailablePitch, OnUpgradeAvailableVolume);
        }

        public void PlayUnlockStatSound()
        {
            if (OnUnlockStatClips == null || OnUnlockStatClips.Length == 0) return;
            AudioClip selectedClip = OnUnlockStatClips[Random.Range(0, OnUnlockStatClips.Length)];
            TriggerSpecialSound(selectedClip, MinOnUnlockStatPitch, MaxOnUnlockStatPitch, OnUnlockStatVolume);
        }

        public void PlayOnDemoFinishedSound()
        {
            TriggerSpecialSound(OnDemoFinishedClip, MinOnDemoFinishedPitch, MaxOnDemoFinishedPitch, OnDemoFinishedVolume);
        }

        private IEnumerator PlayDemoFinishedRoutine(AudioClip audioClip, float minpitch, float maxpitch, float clipvolume)
        {
            if (audioClip == null) yield break;

            if (demoAudioSource == null)
            {
                GameObject demoSourceObj = new GameObject("DemoFinished_AudioSource");
                demoSourceObj.transform.SetParent(transform);
                demoAudioSource = demoSourceObj.AddComponent<AudioSource>();
                demoAudioSource.playOnAwake = false;
                if (UIMixerGroup != null) demoAudioSource.outputAudioMixerGroup = UIMixerGroup;
            }

            SetAllOtherSourcesDucked(true);

            float pitch = Random.Range(minpitch, maxpitch);
            demoAudioSource.pitch = pitch;
            demoAudioSource.volume = clipvolume;
            demoAudioSource.clip = audioClip;
            demoAudioSource.Play();

            float playbackDuration = audioClip.length / Mathf.Max(0.01f, Mathf.Abs(pitch));
            yield return new WaitForSecondsRealtime(playbackDuration);

            SetAllOtherSourcesDucked(false);
            demoFinishedCoroutine = null;
        }

        private void SetAllOtherSourcesDucked(bool duck)
        {
            if (duck)
            {
                // Only capture base volumes if we aren't ALREADY ducked
                if (!isDucked)
                {
                    originalSourceVolumes.Clear();

                    DuckSingleSource(BackgroundSource);
                    DuckSingleSource(UISource);

                    for (int i = 0; i < sfxPool.Count; i++)
                    {
                        DuckSingleSource(sfxPool[i]);
                    }

                    isDucked = true;
                }
            }
            else
            {
                // Restore each registered source back to its pre-duck volume
                foreach (var kvp in originalSourceVolumes)
                {
                    if (kvp.Key != null)
                    {
                        kvp.Key.volume = kvp.Value;
                    }
                }

                originalSourceVolumes.Clear();
                isDucked = false;
            }
        }

        private void DuckSingleSource(AudioSource source)
        {
            if (source == null) return;

            // Store true baseline volume before ducking
            originalSourceVolumes[source] = source.volume;
            source.volume *= DuckMultiplier;
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
        public void PlayNextTrack()
        {
            if (BackgroundList == null || BackgroundList.Count == 0) return;

            CurrentTrackIndex = (CurrentTrackIndex + 1) % BackgroundList.Count;
            PlayTrack(CurrentTrackIndex);
        }

        [ContextMenu("Play Previous Track")]
        public void PlayPreviousTrack()
        {
            if (BackgroundList == null || BackgroundList.Count == 0) return;

            CurrentTrackIndex = (CurrentTrackIndex - 1 + BackgroundList.Count) % BackgroundList.Count;
            PlayTrack(CurrentTrackIndex);
        }

        private void PlayTrack(int index)
        {
            if (musicCoroutine != null)
            {
                StopCoroutine(musicCoroutine);
            }

            musicCoroutine = StartCoroutine(CrossfadeTrackRoutine(index));
        }

        private IEnumerator CrossfadeTrackRoutine(int index)
        {
            AudioClip clip = BackgroundList[index];

            // If already playing something, fade out the current track first
            if (BackgroundSource.isPlaying && BackgroundSource.volume > 0f)
            {
                yield return StartCoroutine(FadeSource(BackgroundSource, BackgroundSource.volume, 0f, fadeDuration));
            }

            // Assign and start playing at zero volume
            BackgroundSource.clip = clip;
            BackgroundSource.volume = 0f;
            BackgroundSource.Play();

            // Fade In
            yield return StartCoroutine(FadeSource(BackgroundSource, 0f, BackgroundVolume, fadeDuration));

            // Calculate how long to play at full volume before needing to fade out
            // Mathf.Max protects against tracks shorter than total fade duration
            float sustainTime = Mathf.Max(0f, clip.length - (fadeDuration * 2f));
            yield return new WaitForSeconds(sustainTime);

            // Fade Out
            yield return StartCoroutine(FadeSource(BackgroundSource, BackgroundVolume, 0f, fadeDuration));

            // Move to the next track
            PlayNextTrack(); // Or call your next-track selection logic here
        }

        private IEnumerator FadeSource(AudioSource source, float startVol, float targetVol, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                source.volume = Mathf.Lerp(startVol, targetVol, elapsed / duration);
                yield return null;
            }

            source.volume = targetVol;
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

        public void PlayUIOnUpgradeDenialSound()
        {
            if (OnUpgradeDenialClip == null || UISource == null) return;
            UISource.pitch = Random.Range(MinOnUpgradeDenialPitch, MaxOnUpgradeDenialPitch);
            UISource.PlayOneShot(OnUpgradeDenialClip, OnUpgradeDenialVolume);
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
                timeElapsed += Time.unscaledDeltaTime;
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