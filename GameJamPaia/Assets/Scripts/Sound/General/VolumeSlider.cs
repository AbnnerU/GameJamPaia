using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;

    [Header("Type")]
    [SerializeField] private bool musicSlider;
    [SerializeField] private bool soundEffectsSlider;

    [Header("Config")]
    [SerializeField] private bool updateOnEnable=true;
    [SerializeField] private bool updateOnDisable;

    private void Awake()
    {
        if(musicSlider)
            slider.onValueChanged.AddListener(SetNewMusicVolume);
        else if(soundEffectsSlider)
            slider.onValueChanged.AddListener(SetNewSoundEffectVolume);
    }

    private void OnEnable()
    {
        if (updateOnEnable)
        {
            if (musicSlider)
                slider.value = SoundsSettings.GetMusicVolume();
            else if (soundEffectsSlider)
                slider.value = SoundsSettings.GetSoundEffectVolume();
        }

    }

    private void OnDisable()
    {
        if (updateOnDisable)
        {
            if (musicSlider)
                slider.value = SoundsSettings.GetMusicVolume();
            else if (soundEffectsSlider)
                slider.value = SoundsSettings.GetSoundEffectVolume();
        }
    }

    public void SetNewMusicVolume(float newValue)
    {
        SoundsManager.Instance?.SetNewMusicVolume(newValue);
    }

    public void SetNewSoundEffectVolume(float newValue)
    {
        SoundsManager.Instance?.SetNewSoundEffectVolume(newValue);
    }

    private void OnValidate()
    {
        if (musicSlider)
            soundEffectsSlider = false;
        else if (soundEffectsSlider)
            musicSlider = false;
    }
}
