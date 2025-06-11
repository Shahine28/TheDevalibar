using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextSettingContainer : MonoBehaviour
{
    public TextMeshProUGUI SettingNameText;
    public Slider SettingSlider;
    public TextMeshProUGUI SettingValueText;
    
    public int SettingValue;
    [MinMaxSlider(0,100)]
    public Vector2Int SettingRangeValue;
    
    private void OnValidate()
    {
        SettingValue = Mathf.Clamp(SettingValue, SettingRangeValue.x, SettingRangeValue.y);
    }
    public void Awake()
    {
        SettingSlider.minValue = SettingRangeValue.x;
        SettingSlider.maxValue = SettingRangeValue.y;
        SettingSlider.value = SettingValue;
        
        
        SettingValueText.text = SettingValue.ToString();
    }

    public void UpdateValue(int newValue)
    {
        SettingValue = Mathf.Clamp(newValue, SettingRangeValue.x, SettingRangeValue.y);
    }
}
