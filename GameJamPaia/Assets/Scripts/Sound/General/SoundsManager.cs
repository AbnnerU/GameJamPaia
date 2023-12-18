using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundsManager : ManagerBase<SoundsManager>
{
    [Header("Audio Source Pool")]
    [SerializeField] private GameObject emitterPrefab;
    [SerializeField] private int initialSize;

    [Header("Channel")]
    [SerializeField] private AudioChannel musicAudioChannel;
    [SerializeField] private AudioChannel soundEffectsChannel;

    [Header("General")]
    [SerializeField] private bool stopMainTrackWhenNewScene = true;

    private List<SoundEmitter> soundEmitters = new List<SoundEmitter>();

    private void Awake()
    {
        Setup(this);

        musicAudioChannel.OnAudioRequest += MusicChannel_AudioRequest;
        musicAudioChannel.OnStopAudioRequest += MusicChannel_StopAudioRequest;

        soundEffectsChannel.OnAudioRequest += SoundEffect_AudioRequest;
        soundEffectsChannel.OnStopAudioRequest += SoundEffect_StopAudioRequest;

        Prewarm();

        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        if (stopMainTrackWhenNewScene)
            StopMainTrack();

        StopSfXAudios();
    }

    private void SoundEffect_StopAudioRequest(AudioRequestInfo request)
    {
        StopAudioRequest(request.audioConfig, request.audioPosition);
    }

    private void SoundEffect_AudioRequest(AudioRequestInfo request)
    {
        float volumePercentage = SoundsSettings.GetSoundEffectVolume(); //Value between 0 and 100

        for (int i = 0; i < soundEmitters.Count; i++)
        {
            if (soundEmitters[i].InUse() == false)
            {
                //print("Sound manager request: " + request.audioConfig.minDistance);

                soundEmitters[i].PlayAudio(request.audioConfig, EmitterType.SOUNDEFFECTS, volumePercentage, request.audioPosition);

              

                return ;
            }
        }

        GameObject obj = Instantiate(emitterPrefab, Vector3.zero, Quaternion.identity, this.transform);

        obj.GetComponent<SoundEmitter>().PlayAudio(request.audioConfig, EmitterType.MAiNTRACK, volumePercentage, request.audioPosition);

        soundEmitters.Add(obj.GetComponent<SoundEmitter>());
    }

    private void MusicChannel_StopAudioRequest(AudioRequestInfo request)
    {
        StopAudioRequest(request.audioConfig, request.audioPosition);
    }

    private void MusicChannel_AudioRequest(AudioRequestInfo request)
    {
        float volumePercentage = SoundsSettings.GetMusicVolume(); //Value between 0 and 100

        for (int i = 0; i < soundEmitters.Count; i++)
        {
            if(soundEmitters[i].InUse() == false)
            {
                soundEmitters[i].PlayAudio(request.audioConfig, EmitterType.MAiNTRACK, volumePercentage, request.audioPosition);
                return;
            }

        }

        GameObject obj = Instantiate(emitterPrefab, Vector3.zero, Quaternion.identity, this.transform);

        obj.GetComponent<SoundEmitter>().PlayAudio(request.audioConfig, EmitterType.MAiNTRACK, volumePercentage, request.audioPosition);

        soundEmitters.Add(obj.GetComponent<SoundEmitter>());
    }

    private void StopSfXAudios()
    {
        foreach (SoundEmitter s in soundEmitters)
        {
            if (s.GetEmitterType() == EmitterType.SOUNDEFFECTS && s.InUse())
            {
                s.Stop();
            }
        }
    }

    private void StopAudioRequest(AudioConfig audioConfig, Vector3 positionRef)
    {
        foreach (SoundEmitter s in soundEmitters)
        {
            if (s.IsUsingAudioConfig(audioConfig, positionRef))
            {
                s.Stop();
                return;
            }
        }
    }

    public void SetNewMusicVolume(float newValue)
    {
        foreach (SoundEmitter s in soundEmitters)
        {
            if (s.GetEmitterType() == EmitterType.MAiNTRACK)
                s.ChangeVolume(newValue);
        }

        SoundsSettings.SaveMusicVolume(newValue);
    }

    public void SetNewSoundEffectVolume(float newValue)
    {
        foreach (SoundEmitter s in soundEmitters)
        {
            if (s.GetEmitterType() == EmitterType.SOUNDEFFECTS)
                s.ChangeVolume(newValue);
        }

        SoundsSettings.SaveSoundEffectsVolume(newValue);
    }

    public void StopMainTrack()
    {
        foreach (SoundEmitter s in soundEmitters)
        {
            if (s.GetEmitterType() == EmitterType.MAiNTRACK)
            {
                if (s.InUse())
                {
                    s.StopWhitFadeOut();
                    break;
                }
            }
        }
    }

    private void Prewarm()
    {
        for(int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(emitterPrefab, Vector3.zero, Quaternion.identity, this.transform);

            soundEmitters.Add(obj.GetComponent<SoundEmitter>());
        }
    }
}
