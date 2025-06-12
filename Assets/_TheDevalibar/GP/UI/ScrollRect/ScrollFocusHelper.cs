using MyUtilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollFocusHelper : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler, IPointerClickHandler
{
    [SerializeField] private ScrollViewAutoScroll _scrollViewAutoScroll;
    private Selectable _selectable;
    private Navigation _originalNavigation;
    
    [SerializeField] private MoveUI _moveUI;
    [SerializeField] private bool _disableNavigationAtStart = true;
    [SerializeField] private bool _useShowHideUI = true;
    
    
    public UnityEvent OnItemSelected;
    public UnityEvent OnItemDeselected;
    public UnityEvent OnItemSubmitted;
    public UnityEvent OnItemClicked;
    
    

    private void Awake()
    {
        _selectable = GetComponent<Selectable>();
        _originalNavigation = _selectable.navigation;
        
    }

    void Start()
    {
        if (_disableNavigationAtStart) DisableNavigation();
        if (_moveUI)
        {
            _moveUI.OnUIMoveToTarget += EnableDisableNavigation;
        }
        else if (_useShowHideUI)
        {
            ShowHideUI showHideUI = ServiceLocator.Get<ShowHideUI>();
            if (showHideUI == null) return;
            showHideUI.OnUIShowed += EnableNavigation;
            showHideUI.OnUIHided += DisableNavigation;
        }
    }

    void EnableDisableNavigation(bool enable)
    {
        if (enable)
        {
            EnableNavigation();
        }
        else
        {
            DisableNavigation();
        }
    }

    void EnableNavigation()
    {
        _selectable.navigation = _originalNavigation;
    }

    void DisableNavigation()
    {
        Navigation nav = _selectable.navigation;
        nav.mode = Navigation.Mode.None;
        _selectable.navigation = nav;
    }
    
    public void OnSelect(BaseEventData eventData)
    {
        OnItemSelected?.Invoke();
        _scrollViewAutoScroll?.HandleOnSelectChange(gameObject);
    }
    
    public void OnDeselect(BaseEventData eventData)
    {
        OnItemDeselected?.Invoke();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        
        OnItemSubmitted?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnItemClicked?.Invoke();
    }

    
}

