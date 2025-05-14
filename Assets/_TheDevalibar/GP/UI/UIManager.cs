using NUnit.Framework;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] GameObject _pauseMenus;
    [SerializeField] GameObject _settingsMenus;
    [SerializeField] GameObject _rebindMenus;
    [SerializeField] GameObject _audioMenu;

    [Header("Parameters")]
    //[SerializeField] bool _isPaused = false;
    [SerializeField] bool _isSettingsDisplay = false;
    [SerializeField] bool _isAudioDisplay = false;
    [SerializeField] bool _isRebindDisplay = false;

    void Start()
    {
        _pauseMenus.SetActive(false);
        _audioMenu.SetActive(false);
        _settingsMenus.SetActive(false);
        _rebindMenus.SetActive(false);
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

    public void ShowAudioParam()
    {
        if (!_isAudioDisplay)
        {
            _audioMenu?.SetActive(true);
        }
        else if(!_isAudioDisplay)
        {
            _audioMenu?.SetActive(true);
            _rebindMenus?.SetActive(false);
        }
    }

    public void ShowSettings()
    {
        if (!_isSettingsDisplay)
        {
            _settingsMenus?.SetActive(true);
        }
        else
        {
            _settingsMenus?.SetActive(false);
        }
    }

    public void ShowRebinds()
    {
        if (!_isRebindDisplay)
        {
            _rebindMenus?.SetActive(true);
        }
        else
        {
            _rebindMenus?.SetActive(false);
        }
    }
}
