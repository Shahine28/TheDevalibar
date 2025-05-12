using UnityEngine;
using TMPro;
using UnityEngine.UI;
using NaughtyAttributes;

public class TextResizer : MonoBehaviour
{
    [Header("UI Text")]
    [SerializeField] TextMeshProUGUI _UIText;
    [RangeAttribute(0,100)]
    [SerializeField] int _UITextSize;

    [Header("World Text")]
    [SerializeField] TextMeshPro _WorldText;
    [RangeAttribute(0, 100)]
    [SerializeField] int _WorldTextSize;
   


    void Start()
    {
        _WorldText.fontSize = _WorldTextSize;
        _UIText.fontSize = _UITextSize;
    }

    void Update()
    {
        if (_WorldText.fontSize != _WorldTextSize || _UIText.fontSize != _UITextSize)
        {
            _WorldText.fontSize = _WorldTextSize;
            _UIText.fontSize = _UITextSize;
        }
        else if (_WorldTextSize > 100 || _WorldTextSize < 0 || _UITextSize > 100 || _UITextSize < 0)
        {
            _WorldTextSize = Mathf.Clamp(_WorldTextSize, 0, 100);
            _UITextSize = Mathf.Clamp(_UITextSize, 0, 100);
        }
    }

    [Button]
    public void OnBoldText()
    {
        if (_WorldText.fontStyle != FontStyles.Bold || _UIText.fontStyle != FontStyles.Bold) 
        {
            _WorldText.fontStyle = FontStyles.Bold;
            _UIText.fontStyle = FontStyles.Bold;
        }
        else
        {
            _WorldText.fontStyle = FontStyles.Normal;
            _UIText.fontStyle = FontStyles.Normal;
        }
    }
}
