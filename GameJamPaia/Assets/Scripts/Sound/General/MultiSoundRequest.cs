
using UnityEngine;

public class MultiSoundRequest : MonoBehaviour
{
    [SerializeField] private MultiSoundRequestConfig[] requests;

    public void ExecuteRequest(int id)
    {      
        if (requests[id].positionReference)
            requests[id].channel.AudioRequest(requests[id].audioConfig, requests[id].positionReference.position);
        else
            requests[id].channel.AudioRequest(requests[id].audioConfig, Vector3.zero);
    }

    public void StopRequest(int id)
    {
        if (requests[id].positionReference)
            requests[id].channel.StopAudioRequest(requests[id].audioConfig, requests[id].positionReference.position);
        else
            requests[id].channel.StopAudioRequest(requests[id].audioConfig, Vector3.zero);
    }

    [System.Serializable]
    public struct MultiSoundRequestConfig
    {
        public AudioChannel channel;
        public AudioConfig audioConfig;
        public Transform positionReference;
    }
}