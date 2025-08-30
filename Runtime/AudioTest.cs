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
        AudioManager.PlayMusic(MusicClip[MusicIndex], MusicVolume);
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
}
