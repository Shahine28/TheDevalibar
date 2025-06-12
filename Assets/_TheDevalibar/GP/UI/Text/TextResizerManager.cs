
using System.Collections.Generic;
using System.Linq;
using MyUtilities;
using UnityEngine;
using TMPro;
using NaughtyAttributes;
using UnityEngine.Events;

public class TextResizerManager : MonoBehaviour
{
    public static TextResizerManager Instance;
    
    [Header("UI Text")]
    [SerializeField] private TextSettingContainer _uiFontSize;
    [SerializeField] private TextSettingContainer _uiCharacterSpacing;
    [SerializeField] private TextSettingContainer _uiWordSpacing;
    [SerializeField] private TextSettingContainer _uiLineSpacing;
    
    [Header("World Text")]
    [SerializeField] private TextSettingContainer _worldFontSize;
    [SerializeField] private TextSettingContainer _worldCharacterSpacing;
    [SerializeField] private TextSettingContainer _worldWordSpacing;
    [SerializeField] private TextSettingContainer _worldLineSpacing;
    
    [Header("Text Fonts")]
    [SerializeField] TMP_FontAsset _defaultFont;
    [SerializeField] TMP_FontAsset _openDysFont;
    [SerializeField] TMP_FontAsset _robotCondensedFont;
    
    
    private List<TMP_Text> _cachedUIText;
    private List<TMP_Text> _cachedWorldText;
    
    private UnityAction<float> _uiFontSizeListener;
    private UnityAction<float> _uiCharacterSpacingListener;
    private UnityAction<float> _uiWordSpacingListener;
    private UnityAction<float> _uiLineSpacingListener;
    
    private UnityAction<float> _worldFontSizeListener;
    private UnityAction<float> _worldCharacterSpacingListener;
    private UnityAction<float> _worldWordSpacingListener;
    private UnityAction<float> _worldLineSpacingListener;
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
        ServiceLocator.Register(this);
    }

    void Start()
    {
        //---- UI ----//
        _uiFontSizeListener = v => UpdateSetting((int)v, _uiFontSize, TextSettingModifier.FontSize, _cachedUIText);
        _uiFontSize.SettingSlider.onValueChanged.AddListener(_uiFontSizeListener);
        
        _uiCharacterSpacingListener = v => UpdateSetting((int)v, _uiCharacterSpacing, TextSettingModifier.CharacterSpacing, _cachedUIText);
        _uiCharacterSpacing.SettingSlider.onValueChanged.AddListener(_uiCharacterSpacingListener);
        
        _uiWordSpacingListener = v => UpdateSetting((int)v, _uiWordSpacing, TextSettingModifier.WordSpacing, _cachedUIText);
        _uiWordSpacing.SettingSlider.onValueChanged.AddListener(_uiWordSpacingListener);
        
        _uiLineSpacingListener = v => UpdateSetting((int)v, _uiLineSpacing, TextSettingModifier.LineSpacing, _cachedUIText);
        _uiLineSpacing.SettingSlider.onValueChanged.AddListener(_uiLineSpacingListener);
        

        //---- WORLD ----//
        _worldFontSizeListener = v => UpdateSetting((int)v, _worldFontSize, TextSettingModifier.FontSize, _cachedWorldText);
        _worldFontSize.SettingSlider.onValueChanged.AddListener(_worldFontSizeListener);
        
        _worldCharacterSpacingListener = v => UpdateSetting((int)v, _worldCharacterSpacing, TextSettingModifier.CharacterSpacing, _cachedWorldText);
        _worldCharacterSpacing.SettingSlider.onValueChanged.AddListener(_worldCharacterSpacingListener);
        
        _worldWordSpacingListener = v => UpdateSetting((int)v, _worldWordSpacing, TextSettingModifier.WordSpacing, _cachedWorldText);
        _worldWordSpacing.SettingSlider.onValueChanged.AddListener(_worldWordSpacingListener);
        
        _worldLineSpacingListener = v => UpdateSetting((int)v, _worldLineSpacing, TextSettingModifier.LineSpacing, _cachedWorldText);
        _worldLineSpacing.SettingSlider.onValueChanged.AddListener(_worldLineSpacingListener);
        
        UpdateTextRegistry();
    }

    private void OnDestroy()
    {
        //---- UI ----//
        _uiFontSize.SettingSlider.onValueChanged.RemoveListener(_uiFontSizeListener);
        _uiCharacterSpacing.SettingSlider.onValueChanged.RemoveListener(_uiCharacterSpacingListener);
        _uiWordSpacing.SettingSlider.onValueChanged.RemoveListener(_uiWordSpacingListener);
        _uiLineSpacing.SettingSlider.onValueChanged.RemoveListener(_uiLineSpacingListener);
        

        //---- WORLD ----//
        _worldFontSize.SettingSlider.onValueChanged.RemoveListener(_worldFontSizeListener);
        _worldCharacterSpacing.SettingSlider.onValueChanged.RemoveListener(_worldCharacterSpacingListener);
        _worldWordSpacing.SettingSlider.onValueChanged.RemoveListener(_worldWordSpacingListener);
        _worldLineSpacing.SettingSlider.onValueChanged.RemoveListener(_worldLineSpacingListener);
    }

    void UpdateSetting(int newValue,TextSettingContainer setting, TextSettingModifier settingModifier, List<TMP_Text> textList)
    {
        setting.SettingValueText.text = newValue.ToString();
        setting.UpdateValue(newValue);
        switch (settingModifier)
        {
            case TextSettingModifier.FontSize:
            {
                textList.ForEach(text => text.fontSize = newValue);
                break;
            }
            case TextSettingModifier.CharacterSpacing:
            {   
                textList.ForEach(text => text.characterSpacing = newValue);
                break;
            }
            case TextSettingModifier.WordSpacing:
            {
                textList.ForEach(text => text.wordSpacing = newValue);
                break;
            }
            case TextSettingModifier.LineSpacing:
            {
                textList.ForEach(text => text.lineSpacing = newValue);
                break;
            }
        }
    }

    public void UpdateTextRegistry()
    {
        _cachedUIText = FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .Cast<TMP_Text>()
            .ToList();
    
        _cachedWorldText = FindObjectsByType<TextMeshPro>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .Cast<TMP_Text>()
            .ToList();
    }

    #region Button
    [Button]
    public void ChangeFontToDefault()
    {
        ChangeFont(_defaultFont);   
    }
    [Button]
    public void ChangeFontToOpenDyslexic()
    {
        ChangeFont(_openDysFont);   
    }

    [Button]
    public void ChangeFontToRobotoCondensed()
    {
        ChangeFont(_robotCondensedFont);
    }

    private void ChangeFont(TMP_FontAsset newFont)
    {
        foreach (var text in _cachedUIText)
        {
            text.font = newFont;
        }

        foreach (var text in _cachedWorldText)
        {
            text.font = newFont;
        }
    }
    #endregion
}

public enum TextSettingModifier
{
    FontSize,
    CharacterSpacing,
    WordSpacing,
    LineSpacing
}


