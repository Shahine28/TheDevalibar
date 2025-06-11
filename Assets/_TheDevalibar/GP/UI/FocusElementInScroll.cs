using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FocusElementInScroll : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private EventSystem _event;

    private RectTransform _currentSelected;

    void Update()
    {
        if (_scrollRect == null || _event == null || _currentSelected == null) return;
        if (_currentSelected != _event.currentSelectedGameObject.transform as RectTransform &&
            _event.currentSelectedGameObject.GetComponentInParent<ScrollRect>() == _scrollRect)
        {
            _currentSelected = _event.currentSelectedGameObject.transform as RectTransform;
            Debug.Log($"newSelected : {_currentSelected.name}");

            var calculatedPosition = (Mathf.Abs(_currentSelected.parent.localPosition.y) - (_scrollRect.transform as RectTransform).rect.height / 2);

            Debug.Log($"calculatedPosition : {calculatedPosition}");

            var scrollMin = 0;
            var scrollMax = ((_scrollRect.content.transform as RectTransform).rect.height) - ((_scrollRect.transform as RectTransform).rect.height);
            calculatedPosition = Mathf.Clamp(calculatedPosition, scrollMin, scrollMax);
            _scrollRect.content.localPosition = new Vector2(_scrollRect.content.localPosition.x, calculatedPosition);
        }

    }
}
