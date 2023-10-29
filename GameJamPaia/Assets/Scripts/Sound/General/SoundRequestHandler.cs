
using UnityEngine;

public class SoundRequestHandler : MonoBehaviour
{
    [SerializeField] private SoundRequest[] soundRequests;

    public void RequestSoundId(int id)
    {
        if(id < 0 && id >= soundRequests.Length)
        {
            soundRequests[id].ExecuteRequest();
        }
    }

    public void StopSoundId(int id)
    {
        if (id < 0 && id >= soundRequests.Length)
        {
            soundRequests[id].StopSoundRequest();
        }
    }
}
