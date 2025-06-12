using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderBehavior : MonoBehaviour
{
    [Header("Slider Background")]
    [SerializeField] private Sprite _normalSpriteBackground;
    [SerializeField] private Sprite _hoveredSpriteBackground;
    
    [Header("Slider Fill")]
    [SerializeField] private Sprite _normalSpriteFill;
    [SerializeField] private Sprite _hoveredSpriteFill;
    
    [Header("Slider Handler")]
    [SerializeField] private Sprite _normalSpriteHandler;
    [SerializeField] private Sprite _hoveredSpriteHandler;
    
    [Header("Slider Text")]
    [SerializeField] private TextMeshProUGUI _sliderText;
    private bool _isSliderTextAssigned => _sliderText != null;
    [SerializeField, ShowIf("_isSliderTextAssigned")] private Color _regularTextColor = Color.white;
    [SerializeField, ShowIf("_isSliderTextAssigned")] private Color _highlightTextColor = Color.yellow;
    
    [Header("Slider Images")]
    [SerializeField] private GameObject _sliderSelectedObject;
    [SerializeField] private Image _sliderImageBackground;
    [SerializeField] private Image _sliderImageFill;
    [SerializeField] private Image _sliderImageHandler;


    void Awake()
    {
        if (_sliderText != null) _sliderText.color = _regularTextColor;
        if (_sliderImageBackground != null) _sliderImageBackground.sprite = _normalSpriteBackground;
        if (_sliderImageFill != null) _sliderImageFill.sprite = _normalSpriteFill;
        if (_sliderImageHandler != null) _sliderImageHandler.sprite = _normalSpriteHandler;
        if (_sliderSelectedObject!= null) _sliderSelectedObject.SetActive(false);
    }
    
    
    public void HoverButton()
    {
        if (_sliderText != null ) _sliderText.color = _highlightTextColor;
        if (_sliderSelectedObject!= null) _sliderSelectedObject.SetActive(true);
        if (_sliderImageBackground != null) _sliderImageBackground.sprite = _hoveredSpriteBackground;
        if (_sliderImageFill != null) _sliderImageFill.sprite = _hoveredSpriteFill;
        if (_sliderImageHandler != null) _sliderImageHandler.sprite = _hoveredSpriteHandler;
    }

    public void UnHoverButton()
    {
        if (_sliderText != null ) _sliderText.color = _regularTextColor;
        if (_sliderSelectedObject!= null) _sliderSelectedObject.SetActive(false);
        if (_sliderImageBackground != null) _sliderImageBackground.sprite = _normalSpriteBackground;
        if (_sliderImageFill != null) _sliderImageFill.sprite = _normalSpriteFill;
        if (_sliderImageHandler != null) _sliderImageHandler.sprite = _normalSpriteHandler;
    }
    

    void OnDisable()
    {
        if (_sliderText != null) _sliderText.color = _regularTextColor;
        if (_sliderSelectedObject!= null) _sliderSelectedObject.SetActive(false);
        if (_sliderImageBackground != null) _sliderImageBackground.sprite = _normalSpriteBackground;
        if (_sliderImageFill != null) _sliderImageFill.sprite = _normalSpriteFill;
        if (_sliderImageHandler != null) _sliderImageHandler.sprite = _normalSpriteHandler;
    }
}