using UnityEngine;
using TMPro;
using UnityEngine.UI;
using NaughtyAttributes;
using UnityEngine.TextCore.Text;
using FontStyles = TMPro.FontStyles;

public class TextResizer : MonoBehaviour
{
    [Header("UI Text")]
    [SerializeField] TextMeshProUGUI _UIText;
    [SerializeField] Color _UITextColor;

    [Header("UI Text param")]
    [RangeAttribute(0,100)]
    [SerializeField] int _UITextSize;
    [SerializeField] float _UIspaceInbetweenLetter;
    [SerializeField] float _UISpaceBetweenWord;
    [SerializeField] float _UILineSpace;

    [Header("World Text")]
    [SerializeField] TextMeshPro _worldText;
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
        _worldText.fontSize = _worldTextSize;
        _UIText.fontSize = _UITextSize;
    }

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
}
