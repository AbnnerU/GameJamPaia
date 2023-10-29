
using UnityEngine;

public class RestartButtonSounds : MonoBehaviour
{
    [SerializeField] private AudioChannel channel;
    [SerializeField] private AudioConfig normalAudioConfig;
    [SerializeField] private AudioConfig pressedAudioConfig;
    [SerializeField] private bool playing = false;

    private void OnEnable()
    {
        PlayNormalAudio();
    }

    public void PlayPressedAudio()
    {
        channel.AudioRequest(pressedAudioConfig, Vector3.zero);
    }

   public void PlayNormalAudio()
   {
        if (playing) return;

        playing = true;
        channel.AudioRequest(normalAudioConfig, Vector3.zero);
   }

    public void StopNormalAudio()
    {
        channel.StopAudioRequest(normalAudioConfig, Vector3.zero);
    }
}
