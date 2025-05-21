using NUnit.Framework;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] GameObject _pauseMenus;
    [SerializeField] GameObject _settingsMenus;
    [SerializeField] GameObject _rebindMenus;
    [SerializeField] GameObject _audioMenu;
    [SerializeField] GameObject _textSettingsMenu;

    [Header("Parameters")]
    //[SerializeField] bool _isPaused = false;
    [SerializeField] bool _isSettingsDisplay = false;
    [SerializeField] bool _isAudioDisplay = false;
    [SerializeField] bool _isRebindDisplay = false;
    [SerializeField] bool _isTextSettingsDisplay = false;

    void Start()
    {
        _settingsMenus.SetActive(false);
    }

    void Update()
    {
        
    }

    public void OnPause()
    {
        _pauseMenus?.SetActive(true);
        Time.timeScale = 0f;
    }

    public void resume()
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
            _rebindMenus?.SetActive(false);
            _textSettingsMenu?.SetActive(false);
        }
        else
        {
            _settingsMenus?.SetActive(false);
        }
    }
    public void GoBack()
    {
        _settingsMenus?.SetActive(false);
        /*_audioMenu?.SetActive(false);
        _rebindMenus?.SetActive(false);*/
        _pauseMenus.SetActive(true);
    }

    public void ShowAudioParam()
    {
        if (!_isAudioDisplay)
        {
            _audioMenu?.SetActive(true);
            _rebindMenus?.SetActive(false);
            _textSettingsMenu?.SetActive(false);
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
            _audioMenu?.SetActive(false);
            _textSettingsMenu?.SetActive(false);
        }
        else
        {
            _rebindMenus?.SetActive(false);
        }
    }

    public void ShowTextSettings()
    {
        if (!_isTextSettingsDisplay)
        {
            _textSettingsMenu?.SetActive(true);
            _audioMenu?.SetActive(false);
            _rebindMenus?.SetActive(false);
        }
        else
        {
            _textSettingsMenu?.SetActive(false);
        }
    }
}
