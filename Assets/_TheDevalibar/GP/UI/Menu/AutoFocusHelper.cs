using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class AutoFocusHelper : MonoBehaviour
{
    [SerializeField] private bool autoFocusOnEnable = true;
    [SerializeField] private bool autoFocusOnStart = false;
    void OnEnable()
    {
        if (autoFocusOnEnable)
        {
            SetSelectionFocus();
        }
    }

    void Start()
    {
        if (autoFocusOnStart)
        {
            SetSelectionFocus();
        }
    }
    
    public void SetSelectionFocus()
    {
        if (EventSystem.current.currentSelectedGameObject != gameObject)
            EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
