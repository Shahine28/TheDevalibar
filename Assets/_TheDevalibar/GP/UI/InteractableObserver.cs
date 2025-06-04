using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InteractableObserver : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler, IPointerClickHandler
{
    public UnityEvent OnItemSelected;
    public UnityEvent OnItemDeselected;
    public UnityEvent OnItemSubmitted;
    public UnityEvent OnItemClicked;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public void OnSelect(BaseEventData eventData)
    {
        OnItemSelected?.Invoke();
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
