using CrossingLears.Audio;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
    [Header("Music")]
    public float MusicVolume;
    public float SoundVolume;

    [Header("Music")]
    public int MusicIndex;
    public AudioClip[] MusicClip;

    [Header("SFX")]
    public GameObject SoundSource;
    public AudioClip SoundEffectClip;

    [Header("UI")]
    public AudioClip UIClip;

    [CrossingLears.Button]
    public void ToggleMusic()
    {
        MusicIndex++;
        if (MusicIndex >= MusicClip.Length)
        {
            MusicIndex = 0;
        }
        AudioManager.ChangeBackgroundMusic(MusicClip[MusicIndex], MusicVolume);
    }

    [CrossingLears.Button]
    public void PlaySFX()
    {
        AudioManager.PlaySFX(SoundEffectClip, SoundSource.transform.position, SoundVolume);
    }
    [CrossingLears.Button]
    public void PlayUI()
    {
        AudioManager.PlayUI(SoundEffectClip, SoundVolume);
    }

    [Header("Library Tests (by Name)")]
    public string TestClipName;

    [CrossingLears.Button]
    public void LibraryPlayUI()
    {
        if (AudioLibrary.Instance != null)
        {
            AudioLibrary.Instance.PlayUI(TestClipName, SoundVolume);
        }
        else
        {
            Debug.LogError("AudioLibrary.Instance is null!");
        }
    }

    [CrossingLears.Button]
    public void LibraryPlaySFX()
    {
        if (AudioLibrary.Instance != null)
        {
            AudioLibrary.Instance.PlaySFX(TestClipName, SoundSource.transform.position, SoundVolume);
        }
        else
        {
            Debug.LogError("AudioLibrary.Instance is null!");
        }
    }
}
