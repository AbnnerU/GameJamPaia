using UnityEngine;

public class SoundRequest : MonoBehaviour
{
    [SerializeField] private bool playOnStart;

    [Header("Sound")]
    [SerializeField] private AudioChannel channel;
    [SerializeField] private AudioConfig audioConfig;
    [SerializeField] private Transform positionReference;

    private void Start()
    {
        if (playOnStart)
        {
            ExecuteRequest();
        }
    }

    public void ExecuteRequest()
    {
        if (positionReference)
            channel.AudioRequest(audioConfig, positionReference.position);
        else
            channel.AudioRequest(audioConfig, Vector3.zero);
    }

}
