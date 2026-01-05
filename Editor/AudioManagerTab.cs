using CrossingLearsEditor;
using UnityEditor;
using UnityEngine;

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
        }
    }
}