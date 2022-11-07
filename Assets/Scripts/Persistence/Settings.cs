using System;
using Miscellaneous;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class Settings : Singleton<Settings>
{
    /*[Serializable]
    public class Display
    {
        public int Width;
        public int Height;
        public FullScreenMode FullScreenMode;
    }

    [SerializeField] private Display display;*/

    [Serializable]
    public class Sound
    {
        public float MasterVolume;
        public float MusicVolume;
        public float EffectsVolume;
    }

    [SerializeField] private Sound sound;

    [Header("References")] [SerializeField]
    private AudioMixer mixer;

    [SerializeField] private Slider masterVolume;
    [SerializeField] private Slider musicVolume;
    [SerializeField] private Slider effectsVolume;

    protected void Awake()
    {
        //LoadDisplay();
        LoadSound();
    }

    public void Game()
    {
        Prefs.DeleteAll();
        SaveSound();
        SceneManager.GetInstance().Load(SceneManager.Game);
    }

    public void Quit()
    {
        Application.Quit();
    }

    #region Display

    /*private void LoadDisplay()
    {
        if (!Prefs.Exists(display))
        {
            Prefs.Save(display = new Display
            {
                Width = Screen.currentResolution.width,
                Height = Screen.currentResolution.height,
                FullScreenMode = FullScreenMode.FullScreenWindow
            });
        }
        else
            Prefs.Load(ref display);

        SetResolution(display.Width, display.Height);
    }*/

    public void SaveDisplay()
    {
        //Prefs.Save(display);
    }

    public void SetResolution(int width, int height)
    {
        Screen.SetResolution(width, height, Screen.fullScreenMode /*display.FullScreenMode*/);
    }

    public void SetFullScreenMode(FullScreenMode fullScreenMode)
    {
        Screen.fullScreenMode = /*display.FullScreenMode = */fullScreenMode;
    }

    #endregion

    #region Sound

    private void LoadSound()
    {
        Prefs.Load(ref sound);

        SetMasterVolume(sound.MasterVolume);
        SetMusicVolume(sound.MusicVolume);
        SetEffectsVolume(sound.EffectsVolume);

        if (masterVolume != null) masterVolume.value = sound.MasterVolume;
        if (musicVolume != null) musicVolume.value = sound.MusicVolume;
        if (effectsVolume != null) effectsVolume.value = sound.EffectsVolume;
    }

    public void SaveSound()
    {
        Prefs.Save(sound);
    }

    public void SetMasterVolume(float volume)
    {
        SetMixerParameter("masterVolume", sound.MasterVolume = volume);
    }

    public void SetMusicVolume(float volume)
    {
        SetMixerParameter("musicVolume", sound.MusicVolume = volume);
    }

    public void SetEffectsVolume(float volume)
    {
        SetMixerParameter("effectsVolume", sound.EffectsVolume = volume);
    }

    private void SetMixerParameter(string parameter, float value)
    {
        mixer.SetFloat(parameter, value);
    }

    #endregion
}