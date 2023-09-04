using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    [SerializeField] private EmitterType emitterType;
    [SerializeField] private AudioSource _audioSource;

    [Header("FadeOut")]
    [SerializeField] private AnimationCurve fadeOutCurve;

    private AudioConfig currentAudioConfig;
    private Vector3 currentPosition;

    private float defaltVolume;

    private Transform _sourceTrasform;

    private void Awake()
    {
        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();

        _sourceTrasform = _audioSource.transform;


    }

    public void PlayAudio(AudioConfig audioConfig, EmitterType emitterType, float volumePercentage, Vector3 position)
    {
        float finalVolume = audioConfig.volume * (volumePercentage / 100);

        this.emitterType = emitterType;

        defaltVolume = audioConfig.volume;

        _audioSource.volume = finalVolume;

        ApplyConfigs(audioConfig);

        _sourceTrasform.position = position;

        _audioSource.Play();

        currentAudioConfig = audioConfig;
        currentPosition = position;

    }

    private void ApplyConfigs(AudioConfig config)
    {
        _audioSource.clip = config.audioClip;
        _audioSource.priority = config.priority;
        _audioSource.spatialBlend = config.SpatialBlend;
        _audioSource.loop = config.loop;
        _audioSource.minDistance = config.minDistance;
        _audioSource.maxDistance = config.maxDistance;
            
    }

    public bool IsUsingAudioConfig(AudioConfig audioConfig, Vector3 position)
    {
        if (audioConfig == currentAudioConfig && position == currentPosition && InUse())
            return true;
        else
            return false;
    }

    public void ChangeVolume(float percentage)
    {
        _audioSource.volume = defaltVolume * (percentage / 100);
    }

    public void Stop()
    {
        _audioSource.Stop();
    }

    public void StopWhitFadeOut()
    {
        StartCoroutine(FadeOut());
    }

    public bool InUse()
    {
        return _audioSource.isPlaying;
    }

    public EmitterType GetEmitterType()
    {
        return emitterType;
    }

    IEnumerator FadeOut()
    {
        float startVolume = _audioSource.volume;

        float fadeOutTime = fadeOutCurve.keys[fadeOutCurve.length - 1].time;

        float currentTime = 0;

        do
        {
            currentTime += Time.deltaTime;

            _audioSource.volume = startVolume * fadeOutCurve.Evaluate(currentTime);

            yield return null;

        } while (currentTime < fadeOutTime);

        _audioSource.Stop();

        yield break;
    }

}

public enum EmitterType
{
    MAiNTRACK,
    SOUNDEFFECTS
}
