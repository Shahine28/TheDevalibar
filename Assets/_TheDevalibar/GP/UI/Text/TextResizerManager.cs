
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
    
   [SerializeField]private float _currentUIFontSize;
   [SerializeField]private float _currentUICharacterSpacing;
   [SerializeField]private float _currentUIWordSpacing;
   [SerializeField]private float _currentUILineSpacing;
   [SerializeField]private TMP_FontAsset _currentUIFont;

   [SerializeField]private float _currentWorldFontSize;
   [SerializeField]private float _currentWorldCharacterSpacing;
   [SerializeField]private float _currentWorldWordSpacing;
   [SerializeField]private float _currentWorldLineSpacing;
   [SerializeField]private TMP_FontAsset _currentWorldFont;
    
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
        _uiFontSizeListener = v => UpdateSetting((int)v, _uiFontSize, TextSettingModifier.FontSize, _cachedUIText,ref _currentUIFontSize);
        _uiFontSize.SettingSlider.onValueChanged.AddListener(_uiFontSizeListener);
        
        _uiCharacterSpacingListener = v => UpdateSetting((int)v, _uiCharacterSpacing, TextSettingModifier.CharacterSpacing, _cachedUIText, ref _currentUICharacterSpacing);
        _uiCharacterSpacing.SettingSlider.onValueChanged.AddListener(_uiCharacterSpacingListener);
        
        _uiWordSpacingListener = v => UpdateSetting((int)v, _uiWordSpacing, TextSettingModifier.WordSpacing, _cachedUIText, ref _currentUIWordSpacing);
        _uiWordSpacing.SettingSlider.onValueChanged.AddListener(_uiWordSpacingListener);
        
        _uiLineSpacingListener = v => UpdateSetting((int)v, _uiLineSpacing, TextSettingModifier.LineSpacing, _cachedUIText, ref _currentUILineSpacing);
        _uiLineSpacing.SettingSlider.onValueChanged.AddListener(_uiLineSpacingListener);
        

        //---- WORLD ----//
        _worldFontSizeListener = v => UpdateSetting((int)v, _worldFontSize, TextSettingModifier.FontSize, _cachedWorldText,ref _currentWorldFontSize);
        _worldFontSize.SettingSlider.onValueChanged.AddListener(_worldFontSizeListener);
        
        _worldCharacterSpacingListener = v => UpdateSetting((int)v, _worldCharacterSpacing, TextSettingModifier.CharacterSpacing, _cachedWorldText, ref _currentWorldCharacterSpacing);
        _worldCharacterSpacing.SettingSlider.onValueChanged.AddListener(_worldCharacterSpacingListener);
        
        _worldWordSpacingListener = v => UpdateSetting((int)v, _worldWordSpacing, TextSettingModifier.WordSpacing, _cachedWorldText, ref _currentWorldWordSpacing);
        _worldWordSpacing.SettingSlider.onValueChanged.AddListener(_worldWordSpacingListener);
        
        _worldLineSpacingListener = v => UpdateSetting((int)v, _worldLineSpacing, TextSettingModifier.LineSpacing, _cachedWorldText, ref _currentWorldLineSpacing);
        _worldLineSpacing.SettingSlider.onValueChanged.AddListener(_worldLineSpacingListener);
        
        SetDefaultValue();
        UpdateTextRegistry();
    }

    private void OnDestroy()
    {
        //---- UI ----//
        if (_uiFontSizeListener != null) _uiFontSize.SettingSlider.onValueChanged.RemoveListener(_uiFontSizeListener);
        if (_uiCharacterSpacingListener != null) _uiCharacterSpacing.SettingSlider.onValueChanged.RemoveListener(_uiCharacterSpacingListener);
        if (_uiWordSpacingListener != null) _uiWordSpacing.SettingSlider.onValueChanged.RemoveListener(_uiWordSpacingListener);
        if (_uiLineSpacingListener != null) _uiLineSpacing.SettingSlider.onValueChanged.RemoveListener(_uiLineSpacingListener);
        

        //---- WORLD ----//
        if (_worldFontSizeListener != null) _worldFontSize.SettingSlider.onValueChanged.RemoveListener(_worldFontSizeListener);
        if (_worldCharacterSpacingListener != null) _worldCharacterSpacing.SettingSlider.onValueChanged.RemoveListener(_worldCharacterSpacingListener);
        if (_worldWordSpacingListener != null) _worldWordSpacing.SettingSlider.onValueChanged.RemoveListener(_worldWordSpacingListener);
        if (_worldLineSpacingListener != null) _worldLineSpacing.SettingSlider.onValueChanged.RemoveListener(_worldLineSpacingListener);
    }

    void SetDefaultValue()
    {
        _currentUIFontSize = _uiFontSize.SettingValue;
        _currentUICharacterSpacing = _uiCharacterSpacing.SettingValue;
        _currentUIWordSpacing = _uiWordSpacing.SettingValue;
        _currentUILineSpacing = _uiLineSpacing.SettingValue;
        _currentUIFont = _defaultFont;
        
        _currentWorldFontSize = _worldFontSize.SettingValue;
        _currentWorldCharacterSpacing = _worldCharacterSpacing.SettingValue;
        _currentWorldWordSpacing = _worldWordSpacing.SettingValue;
        _currentWorldLineSpacing = _worldLineSpacing.SettingValue;
        _currentWorldFont = _defaultFont;
        
    }

    void UpdateSetting(int newValue,TextSettingContainer setting, TextSettingModifier settingModifier, List<TMP_Text> textList,ref float valueToUpdate)
    {
        setting.SettingValueText.text = newValue.ToString();
        setting.UpdateValue(newValue);
        valueToUpdate = newValue;
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

        foreach (var text in _cachedUIText)
        {
            text.fontSize = _currentUIFontSize;
            text.characterSpacing = _currentUICharacterSpacing;
            text.wordSpacing = _currentUIWordSpacing;
            text.lineSpacing = _currentUILineSpacing;
            text.font = _currentUIFont;
        }
        
        _cachedWorldText = FindObjectsByType<TextMeshPro>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .Cast<TMP_Text>()
            .ToList();
        
        foreach (var text in _cachedWorldText)
        {
            text.fontSize = _currentWorldFontSize;
            text.characterSpacing = _currentWorldCharacterSpacing;
            text.wordSpacing = _currentWorldWordSpacing;
            text.lineSpacing = _currentWorldLineSpacing;
            text.font = _currentWorldFont;
        }
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
        
        _currentUIFont = newFont;
        _currentWorldFont = newFont;
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


