using UnityEngine;
using UnityEngine.UI;

public class ScrollViewAutoScroll : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private RectTransform _viewportRectTransform;
    [SerializeField] private RectTransform _content;
    [SerializeField] private float _transitionDuration = 0.2f;

    private TransitionHelper _transitionHelper = new TransitionHelper();

    private void Update()
    {
        if (_transitionHelper.InProgress)
        {
            _transitionHelper.Update();
            _content.localPosition = _transitionHelper.PosCurrent;
        }
    }

    public void HandleOnSelectChange(GameObject selected)
    {
        RectTransform selectedRect = selected.GetComponent<RectTransform>();
        if (selectedRect == null) return;

        Vector3[] itemCorners = new Vector3[4];
        Vector3[] viewportCorners = new Vector3[4];

        selectedRect.GetWorldCorners(itemCorners);
        _viewportRectTransform.GetWorldCorners(viewportCorners);

        Vector2 offset = Vector2.zero;

        if (_scrollRect.vertical)
        {
            float topDelta = itemCorners[1].y - viewportCorners[1].y;
            float bottomDelta = itemCorners[0].y - viewportCorners[0].y;

            if (topDelta > 0f)
                offset.y = topDelta + GetVerticalPadding().top;
            else if (bottomDelta < 0f)
                offset.y = bottomDelta - GetVerticalPadding().bottom;
        }

        if (_scrollRect.horizontal)
        {
            float leftDelta = itemCorners[0].x - viewportCorners[0].x;
            float rightDelta = itemCorners[3].x - viewportCorners[3].x;

            if (leftDelta < 0f)
                offset.x = leftDelta - GetHorizontalPadding().left;
            else if (rightDelta > 0f)
                offset.x = rightDelta + GetHorizontalPadding().right;
        }

        if (offset != Vector2.zero)
        {
            Vector2 start = _content.localPosition;
            Vector2 target = start - offset * 2;
            _transitionHelper.TransitionPositionFromTo(start, target, _transitionDuration);
        }
    }

    private RectOffset GetVerticalPadding()
    {
        var layout = _content.GetComponent<VerticalLayoutGroup>();
        return layout != null ? layout.padding : new RectOffset();
    }

    private RectOffset GetHorizontalPadding()
    {
        var layout = _content.GetComponent<HorizontalLayoutGroup>();
        return layout != null ? layout.padding : new RectOffset();
    }

    private class TransitionHelper
    {
        private float _duration, _timeElapsed, _progress;
        private bool _inProgress;
        public bool InProgress => _inProgress;

        private Vector2 _posCurrent, _posFrom, _posTo;
        public Vector2 PosCurrent => _posCurrent;

        public void Update()
        {
            if (!_inProgress) return;
            _timeElapsed += Time.unscaledDeltaTime;
            _progress = Mathf.Clamp01(_timeElapsed / _duration);
            _posCurrent = Vector2.Lerp(_posFrom, _posTo, _progress);
            if (_progress >= 1f) _inProgress = false;
        }

        public void TransitionPositionFromTo(Vector2 from, Vector2 to, float duration)
        {
            _posFrom = from;
            _posTo = to;
            _duration = duration;
            _timeElapsed = 0f;
            _progress = 0f;
            _inProgress = true;
        }
    }
}
