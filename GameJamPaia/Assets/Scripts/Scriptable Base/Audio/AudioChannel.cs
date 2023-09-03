using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioChannel", menuName = "Assets/AudioChannel")]
public class AudioChannel : ScriptableObject
{
    public Action<AudioRequestInfo> OnAudioRequest;

    public Action<AudioRequestInfo> OnStopAudioRequest;

    public void StopAudioRequest(AudioConfig audioConfig, Vector3 audioPosition)
    {
        if (OnStopAudioRequest != null)
        {
            AudioRequestInfo audioRequestInfo = new AudioRequestInfo()
            {
                audioConfig = audioConfig,
                audioPosition = audioPosition
            };

            OnStopAudioRequest.Invoke(audioRequestInfo);
        }
    }

    public void AudioRequest(AudioConfig audioConfig, AudioClip clip, Vector3 audioPosition)
    {
        if (OnAudioRequest != null)
        {
            audioConfig.audioClip = clip;

            AudioRequestInfo audioRequestInfo = new AudioRequestInfo()
            {
                audioConfig = audioConfig,
                audioPosition = audioPosition
            };

            OnAudioRequest.Invoke(audioRequestInfo);
        }
    }

    public void AudioRequest(AudioConfig audioConfig, Vector3 audioPosition)
    {
        if (OnAudioRequest != null)
        {
            AudioRequestInfo audioRequestInfo = new AudioRequestInfo()
            {
                audioConfig = audioConfig,
                audioPosition = audioPosition
            };

            OnAudioRequest.Invoke(audioRequestInfo);
        }
    }

    public void AudioRequest(AudioRequestInfo audioRequestInfo)
    {
        if (OnAudioRequest != null)
        {
            OnAudioRequest.Invoke(audioRequestInfo);
        }
    }
}

public struct AudioRequestInfo
{
    public AudioConfig audioConfig;
    public Vector3 audioPosition;
}




