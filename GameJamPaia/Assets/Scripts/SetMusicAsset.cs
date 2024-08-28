#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class SetMusicAsset : MonoBehaviour
{
    [SerializeField]private AudioConfig[] AudioConfig;
    [SerializeField]private AudioClip[] Room;
    private void OnValidate()
    {
        if(Room.Length == AudioConfig.Length)
        {
            for(int i = 0; i < AudioConfig.Length; i++)
            {
                AudioConfig[i].audioClip = Room[i];
#if UNITY_EDITOR
                EditorUtility.SetDirty(AudioConfig[i]);
#endif
            }
        }
    }
}
