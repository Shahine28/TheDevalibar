using System;
using MyUtilities;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AudioSettingContainer : MonoBehaviour
{
    public TextMeshProUGUI SettingNameText;
    public Slider SettingSlider;
    public TextMeshProUGUI SettingValueText;

    public float SettingValue;
    [MinMaxSlider(0, 1)] public Vector2 SettingRangeValue = new Vector2(0, 1);
    
    public UnityEvent<float> OnSettingValueChanged;
    
    public AudioType AudioType;
    
    private void OnValidate()
    {
        SettingValue = Mathf.Clamp(SettingValue, SettingRangeValue.x, SettingRangeValue.y);
    }

    public void Awake()
    {
        SettingSlider.minValue = SettingRangeValue.x;
        SettingSlider.maxValue = SettingRangeValue.y;
        SettingSlider.value = SettingValue;
        SettingValueText.text = (SettingValue*100).ToString();
    }

    void Start()
    {
        RebindToAudioManager();
    }
    
    public void UpdateValue(float newValue)
    {
        if (newValue == SettingValue) return;
        SettingValue = Mathf.Clamp(newValue, SettingRangeValue.x, SettingRangeValue.y);
        SettingSlider.value = SettingValue;
        int NewValue = Mathf.RoundToInt(Mathf.InverseLerp(SettingRangeValue.x, SettingRangeValue.y, SettingValue) * 100); ;
        SettingValueText.text = NewValue.ToString();
        OnSettingValueChanged?.Invoke(SettingValue);
    }


    public void RebindToAudioManager()
    {
        AudioManager audioManager = ServiceLocator.Get<AudioManager>();
        if (audioManager == null) return;

        if (AudioType == AudioType.Music)
        {
            OnSettingValueChanged?.AddListener(audioManager.UpdateMusicVolume);  
        }
        else if (AudioType == AudioType.Sfx)
        {
            OnSettingValueChanged?.AddListener(audioManager.UpdateSfxVolume);  
        }
        
    }
}

[Serializable]
public enum AudioType
{
    Music,
    Sfx,
}