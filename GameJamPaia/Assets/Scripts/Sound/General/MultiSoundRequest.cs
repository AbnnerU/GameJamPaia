
using UnityEngine;

public class MultiSoundRequest : MonoBehaviour
{
    [SerializeField] private MultiSoundRequestConfig[] requests;

    public void ExecuteRequest(int id)
    {
        if (requests[id].positionReference != null)
        {
            requests[id].channel.AudioRequest(requests[id].audioConfig, requests[id].positionReference.position);
            print("Multi audio request: " + requests[id].audioConfig.minDistance);
        }
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

    public void StopAll()
    {
        for(int i=0; i<requests.Length; i++)
        {
            if (requests[i].positionReference)
                requests[i].channel.StopAudioRequest(requests[i].audioConfig, requests[i].positionReference.position);
            else
                requests[i].channel.StopAudioRequest(requests[i].audioConfig, Vector3.zero);
        }
    }
  

    [System.Serializable]
    public struct MultiSoundRequestConfig
    {
        public AudioChannel channel;
        public AudioConfig audioConfig;
        public Transform positionReference;
    }
}
