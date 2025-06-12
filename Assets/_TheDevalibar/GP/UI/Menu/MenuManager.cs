using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class MenuManager : MonoBehaviour
{
    [Header("Menu Elements")]
    [SerializeField] GameObject _mainMenu;
    
    [SerializeField] SettingsMenu _settingsMenus;
    
    void Start()
    {
        _settingsMenus?.gameObject.SetActive(false);
    }
    
#region PauseResume
    public void PauseResume(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (_mainMenu.gameObject.activeInHierarchy)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        _mainMenu?.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        _mainMenu?.SetActive(false);
        Time.timeScale = 1f;
    }

#endregion

    public void ShowSettings()
    {
        _settingsMenus.gameObject.SetActive(true);
        _mainMenu?.SetActive(false);
    }
    
    public void GoBackToMainMenu()
    {
        _settingsMenus.gameObject.SetActive(false);
        _mainMenu?.SetActive(true);
    }

    
}


