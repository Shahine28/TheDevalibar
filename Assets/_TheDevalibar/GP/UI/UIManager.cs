using NUnit.Framework;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] GameObject _mainMenu;
    [SerializeField] GameObject _pauseMenus;
    [SerializeField] GameObject _settingsMenus;
    [SerializeField] GameObject _rebindMenus;
    [SerializeField] GameObject _audioMenu;
    [SerializeField] GameObject _textSettingsMenu;
    [SerializeField] GameObject _keyboardDisplay;
    [SerializeField] GameObject _mouseDisplay;
    [SerializeField] GameObject _controllerDisplay;

    [Header("device container")]
    [SerializeField] GameObject _DeviceDispaly;

    [Header("Parameters")]
    //[SerializeField] bool _isPaused = false;
    [SerializeField] bool _isSettingsDisplay = false;
    [SerializeField] bool _isAudioDisplay = false;
    [SerializeField] bool _isRebindDisplay = false;
    [SerializeField] bool _isTextSettingsDisplay = false;
    [SerializeField] bool _isKeyboardControlDisplay = false;
    [SerializeField] bool _isMouseControlDisplay = false;
    [SerializeField] bool _isControlerControlDisplay = false;
    [SerializeField] bool _isDeviceDisplay = false;

    [Header("Controller Selection")]
    [SerializeField] EventSystem _selected;
    [SerializeField] GameObject _newSelected;
    [SerializeField] GameObject _previousSelected;

    void Start()
    {
        _settingsMenus.SetActive(false);
        _previousSelected = _selected.firstSelectedGameObject;
    }
    
    public void PauseResume(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (_pauseMenus.gameObject.activeInHierarchy)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Pause()
    {
        _pauseMenus?.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        _pauseMenus?.SetActive(false);
        Time.timeScale = 1f;
    }
    public void ShowSettings()
    {
        if (!_isSettingsDisplay)
        {
            _settingsMenus?.SetActive(true);
            _audioMenu?.SetActive(true);
            //_mainMenu?.SetActive(false);
            _pauseMenus.SetActive(false);
            _rebindMenus?.SetActive(false);
            _textSettingsMenu?.SetActive(false);
            StartCoroutine(coucou());
            //set the first selected obj when using controller
           // _selected.SetSelectedGameObject(_newSelected);
            //_selected.firstSelectedGameObject = _newSelected;
        }
        else
        {
            _settingsMenus?.SetActive(false);
        }
    }

    IEnumerator coucou()
    {
        yield return new WaitForSecondsRealtime(0.01f);
        _selected.SetSelectedGameObject(_newSelected);
    }

    public void GoBackToPause()
    {
        _settingsMenus?.SetActive(false);
        /*_audioMenu?.SetActive(false);
        _rebindMenus?.SetActive(false);*/
        _pauseMenus.SetActive(true);
        _selected.SetSelectedGameObject(_previousSelected);
        _selected.firstSelectedGameObject = _previousSelected;
    }

    public void GoBackToMain()
    {
        _settingsMenus?.SetActive(false);
        _mainMenu?.SetActive(true);
        _selected.SetSelectedGameObject(_previousSelected);
        _selected.firstSelectedGameObject= _previousSelected;
    }

    public void ShowAudioParam()
    {
        if (!_isAudioDisplay)
        {
            _audioMenu?.SetActive(true);
            _rebindMenus?.SetActive(false);
            _textSettingsMenu?.SetActive(false);
            _mainMenu.SetActive(false);
            _keyboardDisplay?.SetActive(false);
            _mouseDisplay?.SetActive(false);
            _controllerDisplay?.SetActive(false);
        }
        else
        {
            _audioMenu?.SetActive(false);
        }
    }

    public void ShowRebinds()
    {
        if (!_isRebindDisplay)
        {
            _rebindMenus?.SetActive(true);
            _DeviceDispaly?.SetActive(true);

            _audioMenu?.SetActive(false);
            _textSettingsMenu?.SetActive(false);
            _keyboardDisplay?.SetActive(false);
            _mouseDisplay?.SetActive(false);
            _controllerDisplay?.SetActive(false);
        }
        else
        {
            _rebindMenus?.SetActive(false);
            _DeviceDispaly?.SetActive(false);
        }
    }

    //---- Keyboard ----//
    public void DisplayKeyboardRebind()
    {
        if (!_isKeyboardControlDisplay)
        {
            _keyboardDisplay?.SetActive(true);
            _DeviceDispaly?.SetActive(false);

            _controllerDisplay?.SetActive(false);
            _mouseDisplay?.SetActive(false);
        }
        else
        {
            _keyboardDisplay?.SetActive(false);
            _DeviceDispaly?.SetActive(true);
        }
    }

    //---- Mouse ----//
    public void DisplayMouseRebind()
    {
        if (!_isMouseControlDisplay)
        {
            _mouseDisplay?.SetActive(true);
            _DeviceDispaly?.SetActive(false);
            _keyboardDisplay?.SetActive(false);
            _controllerDisplay?.SetActive(false);
        }
        else
        {
            _mouseDisplay?.SetActive(false);
            _DeviceDispaly?.SetActive(true);
        }
    }

    //---- Controller ----//
    public void DisplayControllerRebind()
    {
        if (!_isControlerControlDisplay)
        {
            _controllerDisplay?.SetActive(true);
            _DeviceDispaly?.SetActive(false);
            _mouseDisplay?.SetActive(false);
            _keyboardDisplay?.SetActive(false);
        }
        else
        {
            _controllerDisplay?.SetActive(false);
            _DeviceDispaly?.SetActive(true);
        }
    }

    public void ShowTextSettings()
    {
        if (!_isTextSettingsDisplay)
        {
            _textSettingsMenu?.SetActive(true);
            _audioMenu?.SetActive(false);
            _rebindMenus?.SetActive(false);
            _keyboardDisplay?.SetActive(false);
            _mouseDisplay?.SetActive(false);
            _controllerDisplay?.SetActive(false);
        }
        else
        {
            _textSettingsMenu?.SetActive(false);
        }
    }
}
