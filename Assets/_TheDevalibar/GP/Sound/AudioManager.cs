using System;
using System.Collections.Generic;
using MyUtilities;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [SerializeField] private AudioSettingContainer _musicSetting;
    [SerializeField] private AudioSettingContainer _sfxSetting;
    
    [SerializeField] private List<Sound> _musicSounds, _sfxSounds;
    [SerializeField] private AudioSource _musicSource, _sfxSource;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        _musicSetting?.UpdateValue(_musicSource.volume);
        _sfxSetting?.UpdateValue(_sfxSource.volume);
        PlayMusic("DefaultTheme");
    }
    
#region Music
    public void PlayMusic(string name)
    {
        Sound s = _musicSounds.Find(x => x.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound not found!");
        }
        else
        {
            _musicSource.clip = s.clip;
            _musicSource.Play();
        }
    }
    
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("Clip is null!");
        }
        else
        {
            _musicSource.clip = clip;
            _musicSource.Play();
        }
    }

    public void ToggleMusic()
    {
        _musicSource.mute = !_musicSource.mute;
    }

    public void UpdateMusicVolume(float volume)
    {
        _musicSource.volume = Mathf.Clamp01(volume);
        if (_musicSource.volume == 0)
        {
            _musicSource.mute = true;
        }
        else
        {
            _musicSource.mute = false;
        }
    }

    public AudioClip GetMusicAudioClip(string name)
    {
        return _musicSounds.Find(x => x.name == name).clip;
    }
#endregion
#region SFX
    public void PlaySFX(string name)
    {
        Sound s = _sfxSounds.Find(x => x.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound not found!");
        }
        else
        {
            _sfxSource.clip = s.clip;
            _sfxSource.Play();
        }
    }
    
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("Clip is null!");
        }
        else
        {
            _sfxSource.clip = clip;
            _sfxSource.Play();
        }
    }

    public void ToggleSFX()
    {
        _sfxSource.mute = !_sfxSource.mute;
    }
    
    public void UpdateSfxVolume(float volume)
    {
        _sfxSource.volume = Mathf.Clamp01(volume);
        if (_sfxSource.volume == 0)
        {
            _sfxSource.mute = true;
        }
        else
        {
            _sfxSource.mute = false;
        }
    }
    
    public AudioClip GetSfxAudioClip(string name)
    {
        return _musicSounds.Find(x => x.name == name).clip;
    }
#endregion
}


[Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}