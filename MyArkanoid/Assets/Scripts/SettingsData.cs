using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingsData
{
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    public bool fullscreen = true;
    public bool vSync = false;

    public SettingsData()
    {
        musicVolume = 1f;
        sfxVolume = 1f;
        fullscreen = true;
        vSync = false;
    }
}
