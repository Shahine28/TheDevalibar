using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonsBehavior : MonoBehaviour

{
    [SerializeField] private Sprite _normalSprite;
    [SerializeField] private Sprite _hoveredSprite;
    [SerializeField] private TextMeshProUGUI _buttonText;
    [SerializeField] private GameObject _buttonSelectedObject;
    
    private bool _isButtonTextAssigned => _buttonText != null;
    
    [SerializeField, ShowIf("_isButtonTextAssigned")] private Color _regularTextColor = Color.white;
    [SerializeField, ShowIf("_isButtonTextAssigned")] private Color _highlightTextColor = Color.yellow;
    private Button _button;
    private Image _buttonImage;


    void Awake()
    {
        _button = GetComponent<Button>();
        _buttonImage = _button.GetComponent<Image>();
        
        if (_buttonText != null) _buttonText.color = _regularTextColor;
        if (_buttonSelectedObject != null) _buttonSelectedObject.SetActive(false);
        if (_buttonImage != null) _buttonImage.sprite = _normalSprite;
    }
    
    
    public void HoverButton()
    {
        if (!_button.interactable)
            return;
        
        if (_buttonText != null ) _buttonText.color = _highlightTextColor;
        if (_buttonSelectedObject != null ) _buttonSelectedObject.SetActive(true);
        if (_buttonImage != null) _buttonImage.sprite = _hoveredSprite;
    }

    public void UnHoverButton()
    {
        if (!_button.interactable)
            return;
        
        if (_buttonText != null ) _buttonText.color = _regularTextColor;
        if (_buttonSelectedObject != null ) _buttonSelectedObject.SetActive(false);
        if (_buttonImage != null) _buttonImage.sprite = _normalSprite;
    }
    

    void OnDisable()
    {
        if (_buttonText != null) _buttonText.color = _regularTextColor;
        if (_buttonSelectedObject != null ) _buttonSelectedObject.SetActive(false);
        if (_buttonImage != null) _buttonImage.sprite = _normalSprite;
    }
}