using Assets.Scripts.GameAction;

using UnityEngine;

public class RequestNewSoundArrayAction : GameAction
{
    [Header("Sound")]
    [SerializeField] private AudioChannel channel;
    [SerializeField] private AudioConfig[] audioConfig;
    [SerializeField] private Transform positionReference;

    private int length = 0;
    private void Awake()
    {
        length = audioConfig.Length;
    }

    public override void DoAction()
    {
        if (length == 0) return;

        if (positionReference)
            channel.AudioRequest(audioConfig[Random.Range(0,length)], positionReference.position);
        else
            channel.AudioRequest(audioConfig[Random.Range(0, length)], Vector3.zero);
    }
}
