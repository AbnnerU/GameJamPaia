
using UnityEngine;
public static class SoundsSettings 
{
    public static void SaveMusicVolume(float volume)
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }
        else
        {
            PlayerPrefs.SetFloat("MusicVolume", 100);
        }
    }

    public static void SaveSoundEffectsVolume(float volume)
    {
        if (PlayerPrefs.HasKey("SoundEffectsVolume"))
        {
            PlayerPrefs.SetFloat("SoundEffectsVolume", volume);
        }
        else
        {
            PlayerPrefs.SetFloat("SoundEffectsVolume", 100);
        }
    }


    public static void GetAllSoundVolumeData(out float musicVolume, out float soundEffectsVolume)
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
            musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        else
        {
            musicVolume = 100;
            SaveMusicVolume(100);
        }

        if (PlayerPrefs.HasKey("SoundEffectsVolume"))
            soundEffectsVolume = PlayerPrefs.GetFloat("SoundEffectsVolume");
        else
        {
            soundEffectsVolume = 100;
            SaveSoundEffectsVolume(100);
        }
    }

    public static float GetMusicVolume()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            return PlayerPrefs.GetFloat("MusicVolume");
        }
        else
        {
            SaveMusicVolume(100);
            return 100;
        }
    }

    public static float GetSoundEffectVolume()
    {
        if (PlayerPrefs.HasKey("SoundEffectsVolume"))
            return PlayerPrefs.GetFloat("SoundEffectsVolume");
        else
        {           
            SaveSoundEffectsVolume(100);
            return 100;
        }
    }
}