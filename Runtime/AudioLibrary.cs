using UnityEngine;

namespace CrossingLears.Audio
{
    public class AudioLibrary : ObjectLibrary<AudioLibrary, AudioClip>
    {
        public static AudioLibrary Instance;
        public AudioManager audioManager;

        protected override string NameGetter(AudioClip inp)
        {
            return inp.name;
        }

        protected new void Awake()
        {
            base.Awake();

            if (Instance == null)
            {
                Instance = this;
            }

            if (audioManager == null)
            {
                audioManager = GetComponent<AudioManager>();
            }
        }

        private void Reset()
        {
            if (audioManager == null)
            {
                audioManager = GetComponent<AudioManager>();
            }
        }

        [CrossingLears.Button]
        public void ChangeBackgroundMusic(string clipName)
        {
            AudioClip clip = Get(clipName);

            if (clip == null)
            {
                Debug.LogWarning($"[AudioLibrary] Music clip not found: {clipName}");
                return;
            }

            AudioManager.ChangeBackgroundMusic(clip);
        }

        public static void Play(string clipName, float volume = 1f)
        {
            AudioClip clip = Get(clipName);

            if (clip == null)
            {
                Debug.LogWarning($"[AudioLibrary] UI clip not found: {clipName}");
                return;
            }

            AudioManager.PlayUI(clip, volume);
        }

        public static void Play(string clipName, Vector3 position, float volume = 1f)
        {
            AudioClip clip = Get(clipName);

            if (clip == null)
            {
                Debug.LogWarning($"[AudioLibrary] SFX clip not found: {clipName}");
                return;
            }

            AudioManager.PlaySFX(clip, position, volume);
        }

        public static void PlayMusic(string clipName, float volume = 1f, bool looping = true)
        {
            AudioClip clip = Get(clipName);

            if (clip == null)
            {
                Debug.LogWarning($"[AudioLibrary] Music clip not found: {clipName}");
                return;
            }

            AudioManager.ChangeBackgroundMusic(clip, volume, looping);
        }
    }
}