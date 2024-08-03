
using UnityEngine;

public class OnTriggerSoundArray : MonoBehaviour
{
    [SerializeField] private string targetTag;

    [SerializeField] private bool uniqueInteraction = true;

    [Header("Sound")]
    [SerializeField] private AudioChannel channel;
    [SerializeField] private AudioConfig[] audioConfig;
    [SerializeField] private Transform positionReference;

    private bool alreadyInteract = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            if (uniqueInteraction && alreadyInteract == false)
            {
                alreadyInteract = true;

                int index = Random.Range(0, audioConfig.Length);

                if (positionReference)
                    channel.AudioRequest(audioConfig[index], positionReference.position);
                else
                    channel.AudioRequest(audioConfig[index], Vector3.zero);
            }
            else if (uniqueInteraction == false)
            {
                int index = Random.Range(0, audioConfig.Length);

                if (positionReference)
                    channel.AudioRequest(audioConfig[index], positionReference.position);
                else
                    channel.AudioRequest(audioConfig[index], Vector3.zero);
            }
        }
    }
}
