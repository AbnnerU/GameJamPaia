
using UnityEngine;

[CreateAssetMenu(fileName = "AudioConfig", menuName = "Assets/AudioConfig")]
public class AudioConfig : ScriptableObject
{
    public AudioClip audioClip;

    [Range(0, 256)]
    public int priority = 128;

    [Range(0, 1)]
    public float volume=1;

    [Range(0, 1)]
    public float SpatialBlend=0;

    public float minDistance;

    public float maxDistance;

    public bool loop=false;
}
