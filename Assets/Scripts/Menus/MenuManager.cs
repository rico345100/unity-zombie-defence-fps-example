using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

    [SerializeField]
    private GameObject[] m_menus;
    [SerializeField]
    private int m_defaultMenu = 0;
    private int m_currentMenu;
    [SerializeField]
    private bool m_showMenus = true;

    [SerializeField]
    private int m_currentlySelectedOption = 0;
    [SerializeField]
    private float m_navigationDelay = 0.5f;
    private float m_navigationWaitTime = 0;

    [SerializeField]
    private int[] m_parentMenus;

    private MenuAudioManager m_menuAudio;

    private bool m_accessingDropDown = false;

    void Start()
    {
        m_currentMenu = m_defaultMenu;
        if(m_showMenus)
            ShowCurrentMenu();
        m_navigationWaitTime = m_navigationDelay;

        m_menuAudio = GetComponentInChildren<MenuAudioManager>();
    }

    void Update()
    {
    }

    public void Button_GoToMenu(int menuIndex)
    {
        m_currentMenu = menuIndex;
        ShowCurrentMenu();
    }

    public void Button_GoToScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void Button_GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void Button_QuitGame()
    {
        Application.Quit();
    }

    public void ShowMenus(bool b)
    {
        m_showMenus = b;
        if(b == false)
        {
            m_menus[m_currentMenu].SetActive(false);
            m_currentMenu = m_defaultMenu;
        }
        ShowCurrentMenu();
    }

    private void ShowCurrentMenu()
    {
        for (int i = 0; i < m_menus.Length; i++)
        {
            if (i == m_currentMenu)
            {
                m_menus[i].SetActive(true);
            }
            else
            {
                m_menus[i].SetActive(false);
            }
        }
    }

}
