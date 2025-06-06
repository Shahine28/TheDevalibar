using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FocusElementInScroll : MonoBehaviour
{
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] EventSystem _event;

    RectTransform _currentSelected;

    void Update()
    {
        if (_currentSelected != _event.currentSelectedGameObject.transform as RectTransform &&
            _event.currentSelectedGameObject.GetComponentInParent<ScrollRect>() == scrollRect)
        {
            _currentSelected = _event.currentSelectedGameObject.transform as RectTransform;
            Debug.Log($"newSelected : {_currentSelected.name}");

            var calculatedPosition = (Mathf.Abs(_currentSelected.parent.localPosition.y) - (scrollRect.transform as RectTransform).rect.height / 2);

            Debug.Log($"calculatedPosition : {calculatedPosition}");

            var scrollMin = 0;
            var scrollMax = ((scrollRect.content.transform as RectTransform).rect.height) - ((scrollRect.transform as RectTransform).rect.height);
            calculatedPosition = Mathf.Clamp(calculatedPosition, scrollMin, scrollMax);
            scrollRect.content.localPosition = new Vector2(scrollRect.content.localPosition.x, calculatedPosition);




            //Debug.Log($"{calculatedPosition}");
            //if (calculatedPosition > ((scrollRect.content.transform as RectTransform).rect.height) - ((scrollRect.transform as RectTransform).rect.height ) )
            //{
            //    Debug.Log("TROP BAS");
            //    calculatedPosition = ((scrollRect.content.transform as RectTransform).rect.height) - ((scrollRect.transform as RectTransform).rect.height);
            //}
            //if(calculatedPosition < 0)
            //{
            //    Debug.Log("TROP HAUT");
            //    calculatedPosition = 0f;
            //}

        }


    }
}
