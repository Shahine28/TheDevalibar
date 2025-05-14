using System.Collections;
using UnityEngine;

public class MoveUI : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Vector2 _startPosition;
    [SerializeField] private Vector2 _targetPosition;
    [SerializeField] private float _speed;
    [SerializeField] private AnimationCurve _curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private Coroutine _coroutine;
    
    private IEnumerator MoveUICoroutine(bool reverse = false)
    {
        Vector2 currentPosition = _rectTransform.anchoredPosition;
        Vector2 startPosition = reverse ? _targetPosition : _startPosition;
        Vector2 targetPosition = reverse ? _startPosition : _targetPosition;
         
        // Redéfinit "start" = position actuelle
        float totalDistance = Vector2.Distance(startPosition, targetPosition);
        float remainingDistance = Vector2.Distance(currentPosition, targetPosition);

        float duration = (1f / _speed) * (remainingDistance / totalDistance);;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float curvedT = _curve.Evaluate(t);

            // Interpolation entre la position actuelle et la cible
            _rectTransform.anchoredPosition = Vector2.Lerp(currentPosition, targetPosition, curvedT);
            yield return null;
        }

        _rectTransform.anchoredPosition = targetPosition;
    }


    public void LaunchMoveUI(bool reverse = false)
    {
        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(MoveUICoroutine(reverse));
    }

    public bool IsUIAtTargetPoint()
    {
        return _rectTransform.anchoredPosition == _targetPosition;
    }

    public bool IsUIAtStartPoint()
    {
        return _rectTransform.anchoredPosition == _startPosition;
    }
}
