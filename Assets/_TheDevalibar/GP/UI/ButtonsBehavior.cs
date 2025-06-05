using System.Collections;
using System.Collections.Generic;
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
    
    [SerializeField] private Color _regularTextColor = Color.white;
    [SerializeField] private Color _highlightTextColor = Color.yellow;
    [SerializeField] private List<ButtonsBehavior> _otherButtonsBehaviors;
    private Button _button;
    private Image _buttonImage;


    void Awake()
    {
        _button = GetComponent<Button>();
        _buttonImage = _button.GetComponent<Image>();
        if (_buttonText != null) _buttonText.color = _regularTextColor;
    }
    
    
    public void HoverButton()
    {
        if (!_button.interactable)
            return;

        if (EventSystem.current != null &&
            EventSystem.current.currentSelectedGameObject != gameObject)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
        
        
         if (_buttonText != null ) _buttonText.color = _highlightTextColor;
        _buttonSelectedObject?.SetActive(true);
        if (_buttonImage != null) _buttonImage.sprite = _hoveredSprite;
    }

    public void UnHoverButton()
    {
        if (!_button.interactable)
            return;

        StopAllCoroutines();
        StartCoroutine(UnhoverDelayed());
    }

    IEnumerator UnhoverDelayed()
    {
        yield return new WaitForEndOfFrame();
        // Si le bouton est toujours sélectionné (clavier/tab ou clic maintenu), on ne change pas la couleur
        if (EventSystem.current != null &&
            EventSystem.current.currentSelectedGameObject == _button.gameObject)
            yield return null;

        if (_buttonText != null ) _buttonText.color = _regularTextColor;
        _buttonSelectedObject?.SetActive(false);
        if (_buttonImage != null) _buttonImage.sprite = _normalSprite;
        
    }

    void OnDisable()
    {
        if (_buttonText != null) _buttonText.color = _regularTextColor;
        _buttonSelectedObject?.SetActive(false);
        if (_buttonImage != null) _buttonImage.sprite = _normalSprite;
    }
}