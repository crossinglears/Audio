using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrossingLears.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        [SerializeField] AudioSource sfxAudioSourcePrefab;

        Dictionary<AudioClip, List<AudioSource>> sfxPool = new Dictionary<AudioClip, List<AudioSource>>();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                ActiveListener = FindAnyObjectByType<AudioListener>(FindObjectsInactive.Include);
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void OnDisable()
        {
            sfxPool.Clear();
        }

        public static void PlayMusic(AudioClip clip, float volume = 1f, bool looping = true)
        {
            Instance.StartCoroutine(Instance.FadeRoutine(clip, volume, looping));
        }
    [Range(0.02f, 2)] public float FadeOutSpeed = 1f;
    [Range(0.02f, 2)]public float FadeInSpeed = 1f;
    public AudioSource MusicAudioSource;
    [Range(0, 1)] public float MusicFadeOffset = 0.7f;

        IEnumerator FadeRoutine(AudioClip clip, float volume, bool looping)
    {
        AudioSource oldSource = MusicAudioSource;

        // New AudioSource
        AudioSource newSource = Instantiate(MusicAudioSource, transform).GetComponent<AudioSource>();
            newSource.gameObject.name = clip.name;
        newSource.clip = clip;
        newSource.loop = looping;
        newSource.volume = 0f;
        newSource.outputAudioMixerGroup = oldSource.outputAudioMixerGroup;
        newSource.Play();

        float fadeOutDuration = 1f / FadeOutSpeed;
        float fadeInDuration = 1f / FadeInSpeed;
        float oldStartVol = oldSource.volume;

        // Start fade out immediately
        StartCoroutine(FadeOutRoutine(oldSource, fadeOutDuration, oldStartVol));

        // Start fade in after offset delay
        float delay = fadeOutDuration * MusicFadeOffset;
        yield return new WaitForSecondsRealtime(delay);
        yield return StartCoroutine(FadeInRoutine(newSource, volume, fadeInDuration));

        MusicAudioSource = newSource;
    }

        IEnumerator FadeOutRoutine(AudioSource source, float duration, float startVol)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                source.volume = Mathf.Lerp(startVol, 0f, t);
                yield return null;
            }
            source.volume = 0f;
            Destroy(source.gameObject);
        }

        IEnumerator FadeInRoutine(AudioSource source, float targetVolume, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                source.volume = Mathf.Lerp(0f, targetVolume, t);
                yield return null;
            }
            source.volume = targetVolume;
        }

        public static void PlaySFX(AudioClip clip, Vector3 position, float volume = 1f)
        {
            List<AudioSource> sources;
            if (!Instance.sfxPool.TryGetValue(clip, out sources))
            {
                sources = new List<AudioSource>();
                Instance.sfxPool[clip] = sources;
            }

            AudioSource source = null;
            for (int i = 0; i < sources.Count; i++)
            {
                if (!sources[i].isPlaying)
                {
                    source = sources[i];
                    break;
                }
            }

            if (source == null)
            {
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

        public AudioListener ActiveListener;
        public static void PlayUI(AudioClip clip, float volume = 1f)
        {
            PlaySFX(clip, Instance.ActiveListener.transform.position, volume);
        }

        static IEnumerator DelayedPlay(AudioSource source, float delay)
        {
            yield return new WaitForSeconds(delay);
            source.Play();
        }
    }
}