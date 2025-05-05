using UnityEngine;
using UnityEngine.EventSystems;

public class ClickMoi : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("coucou"); 
    }
}
