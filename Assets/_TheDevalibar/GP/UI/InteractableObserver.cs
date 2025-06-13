using System.Collections;
using MyUtilities;
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
    private InputValuesManager inputValuesManager;

    private void Start()
    {
        inputValuesManager = ServiceLocator.Get<InputValuesManager>();
    }

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
        if (OnItemHovered == null) return;
        if (inputValuesManager!= null && !inputValuesManager._isMouseUsed) return;
        if (EventSystem.current != null &&
            EventSystem.current.currentSelectedGameObject != gameObject)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
        OnItemHovered?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (OnItemUnhovered == null) return;
        if (inputValuesManager!= null && !inputValuesManager._isMouseUsed) return;
        if (IsSelected) return;
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
            return;
        OnItemUnhovered?.Invoke();
        // StopAllCoroutines();
        // StartCoroutine(OnPointerExitDelayed());
    }

    public IEnumerator OnPointerExitDelayed()
    {
        yield return new WaitForEndOfFrame();
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
            yield return null;
        OnItemUnhovered?.Invoke();
    }
}

