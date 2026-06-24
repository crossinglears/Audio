using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrossingLears.Audio
{
    /// <summary>
    /// Manages playback of music, sound effects (SFX), and UI audio in the game.
    /// Supports pooling of sound effect sources and fading transitions for music.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        /// <summary>
        /// Singleton instance of the AudioManager.
        /// </summary>
        public static AudioManager Instance;

        [Header("SFX Settings")]
        [Tooltip("Prefab for the AudioSource used to play 3D SFX clips.")]
        [SerializeField] private AudioSource sfxAudioSourcePrefab;

        [Header("Music Settings")]
        [Tooltip("AudioSource used to play the active music track.")]
        public AudioSource MusicAudioSource;

        [Tooltip("Fading speed when transitioning music out (multiplier).")]
        [Range(0.02f, 2f)] public float FadeOutSpeed = 1f;

        [Tooltip("Fading speed when transitioning music in (multiplier).")]
        [Range(0.02f, 2f)] public float FadeInSpeed = 1f;

        [Tooltip("The delay offset ratio (0 to 1) of the fade out duration before the new track starts fading in.")]
        [Range(0f, 1f)] public float MusicFadeOffset = 0.7f;

        /// <summary>
        /// The active AudioListener in the scene, used to position UI/2D sounds.
        /// </summary>
        [HideInInspector] public AudioListener ActiveListener;

        private readonly Dictionary<AudioClip, List<AudioSource>> sfxPool = new Dictionary<AudioClip, List<AudioSource>>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                ActiveListener = FindAnyObjectByType<AudioListener>(FindObjectsInactive.Include);
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Instance.ActiveListener = FindAnyObjectByType<AudioListener>(FindObjectsInactive.Include);
                Destroy(gameObject);
            }
        }

        private void OnDisable()
        {
            sfxPool.Clear();
        }

        /// <summary>
        /// Plays a music track, fading out the current track and fading in the new one.
        /// </summary>
        /// <param name="clip">The music clip to play.</param>
        /// <param name="volume">The target volume for the music track.</param>
        /// <param name="looping">Whether the music track should loop.</param>
        public static void ChangeBackgroundMusic(AudioClip clip, float volume = 1f, bool looping = true)
        {
            if (Instance == null)
            {
                Debug.LogWarning("[AudioManager] Cannot play music because Instance is null.");
                return;
            }
            Instance.StartCoroutine(Instance.FadeRoutine(clip, volume, looping));
        }

        /// <summary>
        /// Plays a sound effect at a specific 3D position, using pooling to optimize performance.
        /// Applies slight randomized pitch and volume variations.
        /// </summary>
        /// <param name="clip">The sound effect clip to play.</param>
        /// <param name="position">The 3D position in the world to play the sound at.</param>
        /// <param name="volume">The target volume for the sound effect.</param>
        public static void PlaySFX(AudioClip clip, Vector3 position, float volume = 1f)
        {
            if (Instance == null)
            {
                Debug.LogWarning("[AudioManager] Cannot play SFX because Instance is null.");
                return;
            }

            if (clip == null)
            {
                Debug.LogWarning("[AudioManager] SFX clip is null.");
                return;
            }

            List<AudioSource> sources;
            if (!Instance.sfxPool.TryGetValue(clip, out sources))
            {
                sources = new List<AudioSource>();
                Instance.sfxPool[clip] = sources;
            }

            AudioSource source = null;
            for (int i = 0; i < sources.Count; i++)
            {
                if (sources[i] != null && !sources[i].isPlaying)
                {
                    source = sources[i];
                    break;
                }
            }

            if (source == null)
            {
                if (Instance.sfxAudioSourcePrefab == null)
                {
                    Debug.LogError("[AudioManager] sfxAudioSourcePrefab is not assigned in the inspector!");
                    return;
                }
                source = Instantiate(Instance.sfxAudioSourcePrefab, Instance.transform);
                sources.Add(source);
            }

            source.clip = clip;
            source.volume = volume * Random.Range(0.9f, 1.1f);
            source.pitch = Random.Range(0.95f, 1.05f);
            source.transform.position = position;

            float delay = Random.Range(0f, 0.05f);
            if (delay > 0f)
            {
                Instance.StartCoroutine(DelayedPlay(source, delay));
            }
            else
            {
                source.Play();
            }
        }

        /// <summary>
        /// Plays a UI/2D sound effect at the position of the active AudioListener.
        /// </summary>
        /// <param name="clip">The sound effect clip to play.</param>
        /// <param name="volume">The target volume for the sound effect.</param>
        public static void PlayUI(AudioClip clip, float volume = 1f)
        {
            if (Instance == null)
            {
                Debug.LogWarning("[AudioManager] Cannot play UI sound because Instance is null.");
                return;
            }

            if (Instance.ActiveListener == null)
            {
                Instance.ActiveListener = FindAnyObjectByType<AudioListener>(FindObjectsInactive.Include);
            }

            Vector3 position = Instance.ActiveListener != null
                ? Instance.ActiveListener.transform.position
                : Vector3.zero;

            PlaySFX(clip, position, volume);
        }

        private IEnumerator FadeRoutine(AudioClip clip, float volume, bool looping)
        {
            AudioSource oldSource = MusicAudioSource;

            // New AudioSource
            AudioSource newSource = Instantiate(MusicAudioSource, transform).GetComponent<AudioSource>();
            newSource.gameObject.name = clip.name;
            newSource.clip = clip;
            newSource.loop = looping;
            newSource.volume = 0f;
            newSource.outputAudioMixerGroup = oldSource != null ? oldSource.outputAudioMixerGroup : null;
            newSource.Play();

            float fadeOutDuration = 1f / FadeOutSpeed;
            float fadeInDuration = 1f / FadeInSpeed;
            float oldStartVol = oldSource != null ? oldSource.volume : 0f;

            if (oldSource != null)
            {
                // Start fade out immediately
                StartCoroutine(FadeOutRoutine(oldSource, fadeOutDuration, oldStartVol));
            }

            // Start fade in after offset delay
            float delay = fadeOutDuration * MusicFadeOffset;
            yield return new WaitForSecondsRealtime(delay);
            yield return StartCoroutine(FadeInRoutine(newSource, volume, fadeInDuration));

            MusicAudioSource = newSource;
        }

        private IEnumerator FadeOutRoutine(AudioSource source, float duration, float startVol)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                if (source == null) yield break;
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                source.volume = Mathf.Lerp(startVol, 0f, t);
                yield return null;
            }
            if (source != null)
            {
                source.volume = 0f;
                Destroy(source.gameObject);
            }
        }

        private IEnumerator FadeInRoutine(AudioSource source, float targetVolume, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                if (source == null) yield break;
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                source.volume = Mathf.Lerp(0f, targetVolume, t);
                yield return null;
            }
            if (source != null)
            {
                source.volume = targetVolume;
            }
        }

        private static IEnumerator DelayedPlay(AudioSource source, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (source != null)
            {
                source.Play();
            }
        }
    }
}