using NaughtyAttributes;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [Header("Slider param")]
    [SerializeField] Slider _AmbienceSld;
    [SerializeField] Slider _MusicSld;
    [SerializeField] Slider _EffectsSld;

    [Header("Text Param")]
    [SerializeField] TextMeshProUGUI _AmbianceText;
    [SerializeField] TextMeshProUGUI _MusicText;
    [SerializeField] TextMeshProUGUI _EffectsText;

    [Header("Sounds Value")]
    [SerializeField] int _ambienceVal;
    [SerializeField] int _musicVal;
    [SerializeField] int _effectsVal;

    [Header("Audio List")]
    [SerializeField]public Audio[] _audio;
    
    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = null;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Audio audio in _audio)
        {
            audio._source = gameObject.AddComponent<AudioSource>();
            audio._source.clip = audio._clip;

            audio._source.volume = audio._volume;
            audio._source.pitch = audio._pitch;
            audio._source.loop = audio._loop;
            audio._source.playOnAwake = false;
        }

        _AmbienceSld.value = _ambienceVal;
        _AmbianceText.text = _ambienceVal.ToString();
        _MusicSld.value = _musicVal;
        _MusicText.text = _musicVal.ToString();
        _EffectsSld.value = _effectsVal;
        _EffectsText.text = _effectsVal.ToString();
    }
    private void Start()
    {
        Play("Music");

        _AmbienceSld.onValueChanged.AddListener(UpdateAmbienceSounds);
        _MusicSld.onValueChanged.AddListener(UpdateMusicSounds);
        _EffectsSld.onValueChanged.AddListener(UpdateEffectSounds);
    }

    private void OnDestroy()
    {
        _AmbienceSld.onValueChanged.RemoveListener(UpdateAmbienceSounds);
        _MusicSld.onValueChanged.RemoveListener(UpdateMusicSounds);
        _EffectsSld.onValueChanged.RemoveListener(UpdateEffectSounds);
    }

    void UpdateAmbienceSounds(float v)
    {
        _AmbianceText.text = v.ToString();
        _ambienceVal = (int)v;
    }

    void UpdateMusicSounds(float v)
    {
        _MusicText.text = v.ToString();
        _musicVal = (int)v;
    }

    void UpdateEffectSounds(float v)
    {
        _EffectsText.text = v.ToString();
        _effectsVal = (int)v;
    }

    public void Play(string name)
    {
        Audio a = Array.Find(_audio, audio => audio._name == name);
        if (a == null)
        {
            Debug.LogWarning("Sound:" + name + "not found");
            return;
        }
        a._source.Play();
    }
}