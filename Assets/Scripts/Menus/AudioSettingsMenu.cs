using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioSettingsMenu : MonoBehaviour {

    [SerializeField]
    private Slider m_masterSlider;
    [SerializeField]
    private Slider m_sfxSlider;
    [SerializeField]
    private Slider m_musicSlider;

    [SerializeField]
    private AudioMixer m_mainMixer;

    [SerializeField]
    private float masterVolume = 0.5f;

    void Start ()
    {
        float returnValue = 0.0f;

        //m_mainMixer.GetFloat("MasterVolume", out returnValue);
        m_masterSlider.value = masterVolume;
        AudioListener.volume = masterVolume;

        m_mainMixer.GetFloat("SFXVolume", out returnValue);
        m_sfxSlider.value = returnValue;

        m_mainMixer.GetFloat("MusicVolume", out returnValue);
        m_musicSlider.value = returnValue;
	}

    void Update()
    {
        masterVolume = m_masterSlider.value;        
        AudioListener.volume = m_masterSlider.value;
        //m_mainMixer.SetFloat("MasterVolume", m_masterSlider.value);
        m_mainMixer.SetFloat("SFXVolume", m_sfxSlider.value);
        m_mainMixer.SetFloat("MusicVolume", m_musicSlider.value);
    }

}
