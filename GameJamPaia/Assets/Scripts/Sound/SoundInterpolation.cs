using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundInterpolation : MonoBehaviour
{
    [SerializeField] private MusicInterpolationConfig[] config;
    [SerializeField] private AudioSource audioSource01;
    //[SerializeField] private AudioSource audioSource02;

    [SerializeField] private int playingAt = 0;

    private int lastAudioId=-1;

    public void PlaySound(AudioConfig audioConfig)
    {
        int id = GetIdOf(audioConfig);


        if (id >= 0)
        {
            float volumePercentage;
            float finalVolume;
            if (lastAudioId <0)
            {
                lastAudioId = id;

                volumePercentage = SoundsSettings.GetSoundEffectVolume();
                finalVolume = audioConfig.volume * (volumePercentage / 100);

                audioSource01.clip = audioConfig.audioClip;
                audioSource01.time = config[id].audioClipTime;
                audioSource01.volume = finalVolume;

                audioSource01.clip = audioConfig.audioClip;
                audioSource01.Play();
            }
            else if (audioSource01.isPlaying)
            {
                audioSource01.Pause();
                config[lastAudioId].audioClipTime = audioSource01.time;

                volumePercentage = SoundsSettings.GetSoundEffectVolume();
                finalVolume = audioConfig.volume * (volumePercentage / 100);

                audioSource01.clip = audioConfig.audioClip;
                audioSource01.time = config[id].audioClipTime;
                audioSource01.volume = finalVolume;
                audioSource01.Play();

                lastAudioId = id;
            }
        }

    }


    private int GetIdOf(AudioConfig audioConfig)
    {
        for(int i = 0; i < config.Length; i++)
        {
            if (config[i].audioConfig == audioConfig)
            {
                return i;
            }
        }

        return -1;
    }

    [System.Serializable]
    private class MusicInterpolationConfig
    {
        public AudioConfig audioConfig;
        //public float fadeOutTime;
        //public AnimationCurve fadeOutCurve;
        //public float fadeInTime;
        //public AnimationCurve fadeInCurve;
        public float audioClipTime;
    }
}
