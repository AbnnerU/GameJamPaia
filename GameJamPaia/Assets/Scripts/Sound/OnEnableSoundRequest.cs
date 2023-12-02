
using UnityEngine;

public class OnEnableSoundRequest : MonoBehaviour
{
    [Header("Sound")]
    [SerializeField] private AudioChannel channel;
    [SerializeField] private AudioConfig audioConfig;
    [SerializeField] private Transform positionReference;

    private void OnEnable()
    {
        ExecuteRequest();
    }

    public void ExecuteRequest()
    {
        if (positionReference)
            channel.AudioRequest(audioConfig, positionReference.position);
        else
            channel.AudioRequest(audioConfig, Vector3.zero);
    }

    public void StopSoundRequest()
    {
        if (positionReference)
            channel.StopAudioRequest(audioConfig, positionReference.position);
        else
            channel.StopAudioRequest(audioConfig, Vector3.zero);
    }
}
