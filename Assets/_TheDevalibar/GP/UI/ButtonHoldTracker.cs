using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoldTracker : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool IsHolding { get; private set; }
    public event Action OnPointerStateUpdated;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        IsHolding = true;
        OnPointerStateUpdated?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsHolding = false;
        OnPointerStateUpdated?.Invoke();
    }
}