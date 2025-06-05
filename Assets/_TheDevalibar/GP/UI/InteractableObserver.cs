using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InteractableObserver : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEvent OnItemSelected;
    public UnityEvent OnItemDeselected;
    public UnityEvent OnItemSubmitted;
    public UnityEvent OnItemClicked;
    public UnityEvent OnItemHovered;
    public UnityEvent OnItemUnhovered;

    public bool IsSelected {get; private set; }

    public void OnSelect(BaseEventData eventData)
    {
        IsSelected = true;
        OnItemSelected?.Invoke();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        IsSelected = false;
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnItemHovered?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (IsSelected) return;
        OnItemUnhovered?.Invoke();
    }
}

