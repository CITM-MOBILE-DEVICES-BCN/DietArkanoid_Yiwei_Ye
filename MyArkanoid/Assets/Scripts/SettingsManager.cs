using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    public float MusicVolume { get; private set; }
    public float SFXVolume { get; private set; }
    public bool Fullscreen { get; private set; }
    public bool VSync { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadSettings()
    {
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        Fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        VSync = PlayerPrefs.GetInt("VSync", 1) == 1;

        ApplySettings();
    }

    public void SetMusicVolume(float volume)
    {
        MusicVolume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
        // TODO: Apply music volume change
    }

    public void SetSFXVolume(float volume)
    {
        SFXVolume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
        // TODO: Apply SFX volume change
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Fullscreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
        Screen.fullScreen = isFullscreen;
    }

    public void SetVSync(bool enableVSync)
    {
        VSync = enableVSync;
        PlayerPrefs.SetInt("VSync", enableVSync ? 1 : 0);
        PlayerPrefs.Save();
        QualitySettings.vSyncCount = enableVSync ? 1 : 0;
    }

    private void ApplySettings()
    {
        // TODO: Apply music and SFX volume changes
        Screen.fullScreen = Fullscreen;
        QualitySettings.vSyncCount = VSync ? 1 : 0;
    }
}