using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
                EditorUtility.SetDirty(AudioConfig[i]);
            }
        }
    }
}
