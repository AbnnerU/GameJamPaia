
using UnityEngine;

public class OnTriggerSound : MonoBehaviour
{
    [SerializeField] private string targetTag;

    [SerializeField] private bool uniqueInteraction = true;

    [Header("Sound")]
    [SerializeField] private AudioChannel channel;
    [SerializeField] private AudioConfig audioConfig;
    [SerializeField] private Transform positionReference;

    private bool alreadyInteract = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            if(uniqueInteraction && alreadyInteract == false)
            {
                alreadyInteract = true;

                if (positionReference)
                    channel.AudioRequest(audioConfig, positionReference.position);
                else
                    channel.AudioRequest(audioConfig, Vector3.zero);
            }
            else if (uniqueInteraction == false)
            {
                if (positionReference)
                    channel.AudioRequest(audioConfig, positionReference.position);
                else
                    channel.AudioRequest(audioConfig, Vector3.zero);
            }
        }
    }
}
