using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAudioManager : MonoBehaviour {

    [SerializeField]
    private AudioSource m_sfxAudio;
    [SerializeField]
    private AudioSource m_musicAudio;

    [SerializeField]
    private AudioClip m_acceptAudio;
    [SerializeField]
    private AudioClip m_backAudio;

    public void PlayAccept()
    {
        m_sfxAudio.clip = m_acceptAudio;
        m_sfxAudio.Play();
    }

    public void PlayBack()
    {
        m_sfxAudio.clip = m_backAudio;
        m_sfxAudio.Play();
    }
}
