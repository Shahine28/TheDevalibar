using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonsBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler

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
    private bool _isPointerDown = false;    

    void Awake()
    {
        _button = GetComponent<Button>();
        _buttonImage = _button.GetComponent<Image>();
    }
    
    void Start()
    {
        if (_buttonText != null) _buttonText.color = _regularTextColor;
        _button.onClick.AddListener(UnHoverButton);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverButton();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UnHoverButton();
    }

    private void HoverButton()
    {
        if (_buttonText != null)
        {
            _buttonText.color = _highlightTextColor;
            _buttonSelectedObject.SetActive(true);
            _buttonImage.sprite = _hoveredSprite;
            foreach (var variableButtonsBehavior in _otherButtonsBehaviors)
            {
                variableButtonsBehavior.UnHoverButton();
            }
        }
    }

    public void UnHoverButton()
    {
        if (_buttonText == null || !_button.interactable)
            return;

        // Si le bouton est toujours sélectionné (clavier/tab ou clic maintenu), on ne change pas la couleur
        if (EventSystem.current != null &&
            EventSystem.current.currentSelectedGameObject == _button.gameObject)
            return;

        _buttonText.color = _regularTextColor;
        _buttonSelectedObject.SetActive(false);
        _buttonImage.sprite = _normalSprite;
    }

    void OnDisable()
    {
        if (_buttonText != null) _buttonText.color = _regularTextColor;
        _buttonSelectedObject?.SetActive(false);
        if (_buttonImage != null) _buttonImage.sprite = _normalSprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isPointerDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_isPointerDown)
        {
            _isPointerDown = false;
            UnHoverButton();
        }
    }
}