using UnityEngine;

[ExecuteAlways]
public class TryFit : MonoBehaviour
{
    public RectTransform _rect;
    public RectTransform _childRect;
    // Update is called once per frame
    void Update()
    {
        //Rect r = _childRect.rect;

        _rect.sizeDelta = _childRect.sizeDelta;





    }
}
