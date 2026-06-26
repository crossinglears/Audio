using CrossingLears.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace CrossingLears.Audio
{
    public class AudioManagerTab : CL_WindowTab
    {
        public override string TabName => "Audio";

        public override void DrawContent()
        {
            if (GUILayout.Button("Spawn AudioManager", GUILayout.Height(30)))
            {
                if (!Object.FindAnyObjectByType<CrossingLears.Audio.AudioManager>(FindObjectsInactive.Include))
                {
                    string path = "Assets/Crossing Lears/Audio/Runtime/AudioManager.prefab";
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (prefab != null)
                    {
                        PrefabUtility.InstantiatePrefab(prefab);
                    }
                    else
                    {
                        Debug.LogError("AudioManager prefab not found at: " + path);
                    }
                }
            }

            if (GUILayout.Button("Create Volume Mixers and assign", GUILayout.Height(30)))
{
    string targetFolder = EditorUtility.OpenFolderPanel("Select Folder for Audio Mixer", "Assets", "");

    if (string.IsNullOrEmpty(targetFolder))
    {
        return;
    }

    if (!targetFolder.StartsWith(Application.dataPath))
    {
        Debug.LogError("Selected folder must be inside the Assets folder.");
        return;
    }

    string assetTargetFolder = "Assets" + targetFolder.Substring(Application.dataPath.Length);

    string packageTemplatePath = "Packages/com.crossinglears.audio/Editor/Mixer Samples/AudioMixer.mixer";
    string devTemplatePath = "Assets/Crossing Lears/Audio/Editor/Mixer Samples/AudioMixer.mixer";

    string templatePath = string.Empty;

    if (AssetDatabase.LoadAssetAtPath<Object>(packageTemplatePath) != null)
    {
        templatePath = packageTemplatePath;
    }
    else if (AssetDatabase.LoadAssetAtPath<Object>(devTemplatePath) != null)
    {
        templatePath = devTemplatePath;
    }
    else
    {
        Debug.LogError("Could not find AudioMixer template.");
        return;
    }

    string newMixerPath = assetTargetFolder + "/AudioMixer.mixer";

    AssetDatabase.CopyAsset(templatePath, newMixerPath);

    AudioMixer audioMixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(newMixerPath);
    AudioMixerGroup[] groups = audioMixer.FindMatchingGroups(string.Empty);

    AudioMixerGroup musicGroup = null;
    AudioMixerGroup sfxGroup = null;

    for (int i = 0; i < groups.Length; i++)
    {
        if (groups[i].name == "Music")
        {
            musicGroup = groups[i];
        }

        if (groups[i].name == "SFX")
        {
            sfxGroup = groups[i];
        }
    }

    AudioManager audioManager = Object.FindAnyObjectByType<AudioManager>(FindObjectsInactive.Include);

    if (audioManager != null)
    {
        if (audioManager.MusicAudioSource != null && musicGroup != null)
        {
            audioManager.MusicAudioSource.outputAudioMixerGroup = musicGroup;
            EditorUtility.SetDirty(audioManager.MusicAudioSource);
        }

        if (audioManager.sfxAudioSourcePrefab != null && sfxGroup != null)
        {
            audioManager.sfxAudioSourcePrefab.outputAudioMixerGroup = sfxGroup;
            EditorUtility.SetDirty(audioManager.sfxAudioSourcePrefab);
        }

        EditorUtility.SetDirty(audioManager);
    }

    AssetDatabase.SaveAssets();
    AssetDatabase.Refresh();
}
        }
    }
}