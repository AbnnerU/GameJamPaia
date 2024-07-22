using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundInterpolation : MonoBehaviour
{
    [SerializeField] private List<MusicInterpolationConfig> config;
    [SerializeField] private AudioSource audioSource01;
    //[SerializeField] private AudioSource audioSource02;

    [SerializeField] private int playingAt = 0;

    private int lastAudioId = -1;

    private void Awake()
    {
        config = new List<MusicInterpolationConfig>();
    }

    public void PlaySound(AudioConfig audioConfig)
    {
        int id = GetIdOf(audioConfig);
        //float volumePercentage = SoundsSettings.GetSoundEffectVolume();
        float volumePercentage = 100;
        float finalVolume = audioConfig.volume * (volumePercentage / 100);

        if (id >= 0)
        {

            if (lastAudioId >= 0)
            {
                audioSource01.Pause();
                config[lastAudioId].audioClipTime = audioSource01.time;

                SetSound(audioConfig, finalVolume, id);

                lastAudioId = id;
            }
        }
        else
        {
            if (lastAudioId >= 0)
            {
                audioSource01.Pause();
                config[lastAudioId].audioClipTime = audioSource01.time;
            }

            MusicInterpolationConfig music = new MusicInterpolationConfig();
            music.audioConfig = audioConfig;
            music.audioClipTime = 0;
            config.Add(music);

            int index = GetIdOf(audioConfig);

            SetSound(audioConfig, finalVolume, index);

            lastAudioId = index;
        }

    }

    public void SetSound(AudioConfig audioConfig, float finalVolume, int id)
    {
        audioSource01.clip = audioConfig.audioClip;
        audioSource01.time = config[id].audioClipTime;
        audioSource01.volume = finalVolume;

        audioSource01.Play();
    }


    public void StopSound()
    {
        audioSource01.Stop();
    }

    private int GetIdOf(AudioConfig audioConfig)
    {
        for (int i = 0; i < config.Count; i++)
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
