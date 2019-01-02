using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySettingsMenu : MonoBehaviour {

    private enum EResolution
    {
        r1024x576,
        r1280x720,
        r1600x900,
        r1920x1080
    }

    [SerializeField]
    private Dropdown m_resolutionDropdown;
    [SerializeField]
    private Dropdown m_qualityDropdown;
    [SerializeField]
    private Toggle m_fullScreenToggle;

    private int m_width;
    private int m_height;
    private int m_currentQuality;
    private bool m_fullScreen;

	void Start ()
    {
        m_width = Screen.currentResolution.width;
        m_height = Screen.currentResolution.height;
        m_currentQuality = QualitySettings.GetQualityLevel();
        m_fullScreen = Screen.fullScreen;

        if(m_width == 1024 && m_height == 576)
        {
            m_resolutionDropdown.value = (int)EResolution.r1024x576;
        }
        else if (m_width == 1280 && m_height == 720)
        {
            m_resolutionDropdown.value = (int)EResolution.r1280x720;
        }
        else if(m_width == 1600 && m_height == 900)
        {
            m_resolutionDropdown.value = (int)EResolution.r1600x900;
        }
        else if (m_width == 1920 && m_height == 1080)
        {
            m_resolutionDropdown.value = (int)EResolution.r1920x1080;
        }
        else
        {
            m_resolutionDropdown.value = (int)EResolution.r1024x576;
        }

        m_qualityDropdown.value = m_currentQuality;

        m_fullScreenToggle.isOn = m_fullScreen;
    }

    public void UI_ApplyDisplayChanges()
    {
        switch((EResolution)m_resolutionDropdown.value)
        {
            case EResolution.r1024x576:
                m_width = 1024;
                m_height = 576;
                break;
            case EResolution.r1280x720:
                m_width = 1280;
                m_height = 720;
                break;
            case EResolution.r1600x900:
                m_width = 1600;
                m_height = 900;
                break;
            case EResolution.r1920x1080:
                m_width = 1920;
                m_height = 1080;
                break;
            default:
                m_width = 1024;
                m_height = 576;
                break;
        }

        m_currentQuality = m_qualityDropdown.value;

        m_fullScreen = m_fullScreenToggle.isOn;

        QualitySettings.SetQualityLevel(m_currentQuality);
        Screen.SetResolution(m_width, m_height, m_fullScreen);
    }
}
