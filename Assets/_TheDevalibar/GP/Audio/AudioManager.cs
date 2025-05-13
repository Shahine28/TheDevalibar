using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio parameters")]
    [SerializeField] AUDIO_PROPERTIES _noCategory = new AUDIO_PROPERTIES();
    [SerializeField] AUDIO_PROPERTIES _ambiance = new AUDIO_PROPERTIES();
    [SerializeField] AUDIO_PROPERTIES _music = new AUDIO_PROPERTIES();
    [SerializeField] AUDIO_PROPERTIES _SFX = new AUDIO_PROPERTIES();
    [SerializeField] AUDIO_PROPERTIES _foley = new AUDIO_PROPERTIES();
    [SerializeField] AUDIO_PROPERTIES _voices = new AUDIO_PROPERTIES();

    [Serializable]
    public enum AUDIO_CATEGORY
    {
        NONE = 0,
        Ambiance = 10,
        Music = 11,
        SFX = 12,
        Foley = 13,
        Voices = 14
    }

    [Serializable]
    public struct AUDIO_PROPERTIES
    {
        [Range(0, 1)] public float _volume;
        [Range(-3, 3)] public float _pitch;
        [Range(-1, 1)] public float _stereoPan;
        [Range(0, 1)] public float _spacialBlend;
        [Range(0, 1.1f)] public float _reverbZoneMix;

        public AUDIO_PROPERTIES(bool val = true)
        {
            _volume = 1f;
            _pitch = 0f;
            _stereoPan = 0f;
            _spacialBlend = 0f;
            _reverbZoneMix = 0f;
        }
    }

    [Header("Audio List")]
    [SerializeField] private List<AudioClip> _audioList;
    [Header("Audio Sources")]
    [SerializeField] GameObject _audioChild;

    [SerializeField] private AudioSource _ambianceAudioSource;
    [SerializeField] private AudioSource _musicAudioSource;
    [SerializeField] private AudioSource _voiceAudioSource;
    [SerializeField] private List<AudioSource> _audioSource = new List<AudioSource>();

    public AudioClip GetAudioClip(string audioName)
    {
        foreach (AudioClip audio in _audioList)
        {
            if(audioName == audio.name) return audio;
        }
        return null;
    }

    public AudioClip GetAudioClip(List<AudioClip> localAudioList, string audioName)
    {
        foreach(AudioClip audio in localAudioList)  
        {
            if (audioName == audio.name) return audio;
        }
        return null;
    }
}
