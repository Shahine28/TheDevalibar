using UnityEngine;
using TMPro;
using UnityEngine.UI;
using NaughtyAttributes;
using UnityEngine.TextCore.Text;
using FontStyles = TMPro.FontStyles;
using UnityEditor.ShaderGraph.Internal;
using Unity.VisualScripting;

public class TextResizer : MonoBehaviour
{
    [Header("UI Slider")]
    [SerializeField] Slider _UISizeSldr;
    [SerializeField] Slider _UISpaceLetterSldr;
    [SerializeField] Slider _UISpaceWordSldr;
    [SerializeField] Slider _UILineSpacingSldr;

    [Header("UI Text")]
    [SerializeField] TextMeshProUGUI _UIText;
    [SerializeField] TextMeshProUGUI _UISizeVal;
    [SerializeField] TextMeshProUGUI _UISpaceLetterVal;
    [SerializeField] TextMeshProUGUI _UISpaceWordVal;
    [SerializeField] TextMeshProUGUI _UISpaceLineVal;

    [Header("UI Color")]
    [SerializeField] Color _UITextColor;

    [Header("UI Text param")]
    [RangeAttribute(0,100)]
    [SerializeField] int _UITextSize;
    [SerializeField] float _UIspaceInbetweenLetter;
    [SerializeField] float _UISpaceBetweenWord;
    [SerializeField] float _UILineSpace;

    [Header("World Slider")]
    [SerializeField] Slider _WorldSizeSldr;
    [SerializeField] Slider _WorldSpaceLetterSldr;
    [SerializeField] Slider _WorldSpaceWordSldr;
    [SerializeField] Slider _WorldSpaceLineSldr;

    [Header("World Text")]
    [SerializeField] TextMeshPro _worldText;
    [SerializeField] TextMeshProUGUI _worldSizeVal;
    [SerializeField] TextMeshProUGUI _worldSpaceLetterVal;
    [SerializeField] TextMeshProUGUI _worldSpaceWordVal;
    [SerializeField] TextMeshProUGUI _worldSpaceLineVal;

    [Header("Color Param")]
    [SerializeField] Color _worldTextColor;

    [Header("World Text param")]
    [RangeAttribute(0, 100)]
    [SerializeField] int _worldTextSize;
    [SerializeField] float _worldSpaceInbetweenLetter;
    [SerializeField] float _worldSpaceBetweenWord;
    [SerializeField] float _worldLineSpace;

    [Header("FontStyle")]
    [SerializeField] TMP_FontAsset _openDys;
    [SerializeField] TMP_FontAsset _robotCondensed;

    void Start()
    {
        //---- UI ----//
        //_UILineSpacingSldr.onValueChanged.AddListener((v) => { _UISpaceLineVal.text = v.ToString("0"); coucou=v; });
        _UISizeSldr.onValueChanged.AddListener(UpdateFontSize);
        _UISpaceLetterSldr.onValueChanged.AddListener(UpdateSpaceBetweenletter);
        _UISpaceWordSldr.onValueChanged.AddListener(UpdateSpaceBetweenWrod);
        _UILineSpacingSldr.onValueChanged.AddListener(UpdateLineSpacing);

        //---- World ----//
        _WorldSizeSldr.onValueChanged.AddListener(UpdateWorldSizeFont);
        _WorldSpaceLetterSldr.onValueChanged.AddListener(UpdateWorldSpaceBetweenLetter);
        _WorldSpaceWordSldr.onValueChanged.AddListener(UpdateWorldSpaceBetweenWord);
        _WorldSpaceLineSldr.onValueChanged.AddListener(UpdateWorldSpaceBetweenLine);
    }
    private void OnDestroy()
    {
        //---- UI ----//
        _UISizeSldr.onValueChanged.RemoveListener(UpdateFontSize);
        _UISpaceLetterSldr.onValueChanged.RemoveListener(UpdateSpaceBetweenletter);
        _UISpaceWordSldr.onValueChanged.RemoveListener(UpdateSpaceBetweenWrod);

        //---- World ----//
        _WorldSizeSldr.onValueChanged.RemoveListener(UpdateWorldSizeFont);
        
    }

    #region UI Listener
    void UpdateFontSize(float v)
    {
        _UISizeVal.text = v.ToString();
        _UITextSize = (int)v;
        //Debug.Log(_UITextSize);
    }

    void UpdateSpaceBetweenletter(float v)
    {
        _UISpaceLetterVal.text = v.ToString();
        _UIspaceInbetweenLetter = (int)v;
    }

    void UpdateSpaceBetweenWrod(float v)
    {
        _UISpaceWordVal.text = v.ToString();
        _UISpaceBetweenWord = (int)v;
    }

    void UpdateLineSpacing(float v)
    {
        _UISpaceLineVal.text = v.ToString();
        _UILineSpace = (int)v;
    }
    #endregion

    #region world Listener
    void UpdateWorldSizeFont(float v)
    {
        _worldSizeVal.text = v.ToString();
        _worldTextSize = (int)v;
    }

    void UpdateWorldSpaceBetweenLetter(float v)
    {
        _worldSpaceLetterVal.text = v.ToString();
        _worldSpaceInbetweenLetter = (int)v;
    }

    void UpdateWorldSpaceBetweenWord(float v)
    {
        _worldSpaceWordVal.text = v.ToString();
        _worldSpaceBetweenWord = (int)v;
    }

    void UpdateWorldSpaceBetweenLine(float v)
    {
        _worldSpaceLineVal.text = v.ToString();
        _worldLineSpace = (int)v;
    }
    #endregion

    void Update()
    {
        var allUIText = FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None); //get all TextMeshProUGUI
        foreach (var text in allUIText)
        {
            text.fontSize = _UITextSize; //change UI text size
        }

        var allWorldText = FindObjectsByType<TextMeshPro>(FindObjectsInactive.Include, FindObjectsSortMode.None); // get all TextMeshPro
        foreach (var text in allWorldText)
        {
            text.fontSize = _worldTextSize; // change non UI Text size
        }

        //----- clamp the max and min size of a text -----//
        if (_worldTextSize > 100 || _worldTextSize < 0 || _UITextSize > 100 || _UITextSize < 0)
        {
            _worldTextSize = Mathf.Clamp(_worldTextSize, 0, 100);
            _UITextSize = Mathf.Clamp(_UITextSize, 0, 100);
        }

        //----- Text Color ----//
        _UIText.color = _UITextColor;
        _worldText.color = _worldTextColor;

        //---- Text Spacing settings ----//
        _UIText.characterSpacing = _UIspaceInbetweenLetter;
        _UIText.wordSpacing = _UISpaceBetweenWord;
        _UIText.lineSpacing = _UILineSpace;
        _worldText.characterSpacing = _worldSpaceInbetweenLetter;
        _worldText.wordSpacing = _worldSpaceBetweenWord;
        _worldText.lineSpacing = _worldLineSpace;
    }

    #region Button
    [Button]
    public void OnBoldText()
    {
        var allUIText = FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var text in allUIText)
        {
            text.fontStyle = FontStyles.Bold;
            if(text.fontStyle == FontStyles.Bold) { text.fontStyle = FontStyles.Normal; }
        }

        var allWorldText = FindObjectsByType<TextMeshPro>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var text in allWorldText)
        {
            text.fontStyle = FontStyles.Bold;
            if (text.fontStyle == FontStyles.Bold) { text.fontStyle = FontStyles.Normal; }
        }
    }

    [Button]
    public void ChangeFontToOpenDyslexique()
    {
        var allUIText = FindObjectsByType<TextMeshPro>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var text in allUIText)
        {
            text.font = _openDys; // change non UI Text font style
        }
        var allWorldText = FindObjectsByType<TextMeshPro>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach(var text in allWorldText)
        {
            text.font = _openDys; // change non UI Text font style
        }        
    }

    [Button]
    public void ChangeFontToRoboCondensed()
    {
        var allUIText = FindObjectsByType<TextMeshPro>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var text in allUIText)
        {
            text.font = _robotCondensed;
        }
        var allWorldText = FindObjectsByType<TextMeshPro>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var text in allWorldText)
        {
            text.font = _robotCondensed;
        }
    }
    #endregion
}
