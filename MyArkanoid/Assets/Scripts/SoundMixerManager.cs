using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("materVolume", level);
    }

    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("SFXVolume", level);

    }

    public void SetSFXVolume(float level)
    {
        audioMixer.SetFloat("musicVolume", level);
    }
}
