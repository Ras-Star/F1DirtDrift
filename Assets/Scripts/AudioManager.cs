using UnityEngine;
using System.Collections.Generic;

namespace F1DirtDrift
{
    /// <summary>
    /// Singleton AudioManager that handles all music and sound effects.
    /// Manages audio playback, volume control, and spatial sound.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        #region Singleton
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
        }
        #endregion

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource engineSource;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip menuMusic;
        [SerializeField] private AudioClip raceMusic;
        [SerializeField] private AudioClip engineIdleSound;
        [SerializeField] private AudioClip engineRevSound;
        [SerializeField] private AudioClip skidSound;
        [SerializeField] private AudioClip crashSound;

        [Header("Volume Settings")]
        [Range(0f, 1f)]
        [SerializeField] private float masterVolume = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float musicVolume = 0.7f;
        [Range(0f, 1f)]
        [SerializeField] private float sfxVolume = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float engineVolume = 0.8f;

        // Engine sound pitch based on speed
        private float minEnginePitch = 0.8f;
        private float maxEnginePitch = 1.5f;

        // SFX cooldown tracking
        private Dictionary<string, float> sfxCooldowns = new Dictionary<string, float>();
        private float skidCooldown = 0.5f;

        #region Initialization

        private void Start()
        {
            InitializeAudioSources();
            PlayMenuMusic();
        }

        private void InitializeAudioSources()
        {
            // Create audio sources if not assigned
            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.SetParent(transform);
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }

            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.SetParent(transform);
                sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }

            if (engineSource == null)
            {
                GameObject engineObj = new GameObject("EngineSource");
                engineObj.transform.SetParent(transform);
                engineSource = engineObj.AddComponent<AudioSource>();
                engineSource.loop = true;
                engineSource.playOnAwake = false;
            }

            ApplyVolumeSettings();
        }

        #endregion

        #region Music Control

        /// <summary>
        /// Plays menu background music
        /// </summary>
        public void PlayMenuMusic()
        {
            if (menuMusic != null && musicSource != null)
            {
                musicSource.clip = menuMusic;
                musicSource.Play();
            }
        }

        /// <summary>
        /// Plays race background music
        /// </summary>
        public void PlayRaceMusic()
        {
            if (raceMusic != null && musicSource != null)
            {
                musicSource.clip = raceMusic;
                musicSource.Play();
            }
        }

        /// <summary>
        /// Stops all music
        /// </summary>
        public void StopMusic()
        {
            if (musicSource != null)
            {
                musicSource.Stop();
            }
        }

        #endregion

        #region Sound Effects

        /// <summary>
        /// Plays a one-shot sound effect
        /// </summary>
        public void PlaySFX(AudioClip clip, float volumeScale = 1f)
        {
            if (clip != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(clip, volumeScale * sfxVolume);
            }
        }

        /// <summary>
        /// Plays tire skid sound with cooldown
        /// </summary>
        public void PlaySkidSound()
        {
            if (skidSound == null) return;

            // Check cooldown
            if (sfxCooldowns.ContainsKey("skid") && Time.time < sfxCooldowns["skid"])
            {
                return;
            }

            PlaySFX(skidSound, 0.7f);
            sfxCooldowns["skid"] = Time.time + skidCooldown;
        }

        /// <summary>
        /// Plays crash/collision sound
        /// </summary>
        public void PlayCrashSound()
        {
            if (crashSound != null)
            {
                PlaySFX(crashSound, 1f);
            }
        }

        #endregion

        #region Engine Sound

        /// <summary>
        /// Starts engine idle sound loop
        /// </summary>
        public void StartEngineSound()
        {
            if (engineIdleSound != null && engineSource != null)
            {
                engineSource.clip = engineIdleSound;
                engineSource.loop = true;
                engineSource.Play();
            }
        }

        /// <summary>
        /// Stops engine sound
        /// </summary>
        public void StopEngineSound()
        {
            if (engineSource != null)
            {
                engineSource.Stop();
            }
        }

        /// <summary>
        /// Updates engine sound pitch based on car speed
        /// </summary>
        public void UpdateEngineSound(float speedPercentage)
        {
            if (engineSource != null && engineSource.isPlaying)
            {
                // Adjust pitch based on speed (0 = idle, 1 = max speed)
                engineSource.pitch = Mathf.Lerp(minEnginePitch, maxEnginePitch, speedPercentage);
                engineSource.volume = engineVolume * masterVolume * Mathf.Lerp(0.5f, 1f, speedPercentage);
            }
        }

        #endregion

        #region Volume Control

        /// <summary>
        /// Sets master volume
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            ApplyVolumeSettings();
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        }

        /// <summary>
        /// Sets music volume
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            ApplyVolumeSettings();
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        }

        /// <summary>
        /// Sets SFX volume
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            ApplyVolumeSettings();
            PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        }

        /// <summary>
        /// Applies current volume settings to audio sources
        /// </summary>
        private void ApplyVolumeSettings()
        {
            if (musicSource != null)
            {
                musicSource.volume = musicVolume * masterVolume;
            }

            if (sfxSource != null)
            {
                sfxSource.volume = sfxVolume * masterVolume;
            }

            if (engineSource != null)
            {
                engineSource.volume = engineVolume * masterVolume;
            }
        }

        /// <summary>
        /// Loads volume settings from PlayerPrefs
        /// </summary>
        private void LoadVolumeSettings()
        {
            masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            ApplyVolumeSettings();
        }

        #endregion

        #region Public Accessors

        public float GetMasterVolume() => masterVolume;
        public float GetMusicVolume() => musicVolume;
        public float GetSFXVolume() => sfxVolume;

        #endregion
    }
}
