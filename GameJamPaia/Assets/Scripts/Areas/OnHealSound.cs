
using UnityEngine;

public class OnHealSound : MonoBehaviour
{
    [SerializeField] private HealArea healArea;
    [Header("Sound")]
    [SerializeField] private AudioChannel channel;
    [SerializeField] private AudioConfig[] audioConfig;
    [SerializeField] private Transform positionReference;
    // Start is called before the first frame update
    void Start()
    {
        healArea.OnHeal += OnHeal;
    }

    private void OnHeal()
    {
        int index = Random.Range(0, audioConfig.Length);

        if (positionReference)
            channel.AudioRequest(audioConfig[index], positionReference.position);
        else
            channel.AudioRequest(audioConfig[index], Vector3.zero);
    }
}
