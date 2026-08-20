using UnityEngine;

namespace CrossingLears.Audio
{
    public class AudioLibrary : ObjectLibrary<AudioLibrary, AudioClip>
    {
        protected override string NameGetter(AudioClip inp)
        {
            return inp.name;
        }
        public static AudioLibrary Instance;
        public AudioManager audioManager;

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

        /// <summary>
        /// Plays a sound clip as a UI/2D sound.
        /// </summary>
        public void Play(string clipName)
        {
            PlayUI(clipName);
        }

        /// <summary>
        /// Plays a sound clip as a UI/2D sound with a specific volume.
        /// </summary>
        public void Play(string clipName, float volume)
        {
            PlayUI(clipName, volume);
        }

        /// <summary>
        /// Plays a sound clip at a 3D position with a specific volume.
        /// </summary>
        public void Play(string clipName, Vector3 position, float volume = 1f)
        {
            PlaySFX(clipName, position, volume);
        }

        /// <summary>
        /// Plays a sound clip as a 3D sound at a given position.
        /// </summary>
        public void PlaySFX(string clipName, Vector3 position, float volume = 1f)
        {
            AudioClip clip = Get(clipName);
            if (clip != null)
            {
                AudioManager.PlaySFX(clip, position, volume);
            }
            else
            {
                Debug.LogWarning($"[AudioLibrary] SFX clip not found: {clipName}");
            }
        }

        /// <summary>
        /// Plays a sound clip as a UI/2D sound.
        /// </summary>
        public void PlayUI(string clipName, float volume = 1f)
        {
            AudioClip clip = Get(clipName);
            if (clip != null)
            {
                AudioManager.PlayUI(clip, volume);
            }
            else
            {
                Debug.LogWarning($"[AudioLibrary] UI clip not found: {clipName}");
            }
        }

        /// <summary>
        /// Plays a music track.
        /// </summary>
        public void PlayMusic(string clipName, float volume = 1f, bool looping = true)
        {
            AudioClip clip = Get(clipName);
            if (clip != null)
            {
                AudioManager.ChangeBackgroundMusic(clip, volume, looping);
            }
            else
            {
                Debug.LogWarning($"[AudioLibrary] Music clip not found: {clipName}");
            }
        }
    }
}
