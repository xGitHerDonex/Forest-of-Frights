using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] public Text volumeUIText = null;

    private void Start()
    {
        LoadTheValuesBruh();
    }
    public void VolumeSlider(float volume )
    {
        volumeUIText.text = volume.ToString("0.0");
    }
   public void SaveTheVolumeBruh()
    {
        float volumeValue = volumeSlider.value;
        PlayerPrefs.SetFloat("GameVolume", volumeValue);
        LoadTheValuesBruh();
    }

    private void LoadTheValuesBruh()
    {
        float volumeValue = PlayerPrefs.GetFloat("GameVolume");
        volumeSlider.value = volumeValue;
        AudioListener.volume = volumeValue;
    }
}
