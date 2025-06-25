using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class DayNightLightTransition : MonoBehaviour
{
    [SerializeField] private Light _light;
    [Header("Day Light Values")]
    [SerializeField] private Color _dayLightColor = Color.black;
    [SerializeField] private float _dayLightIntensity = -1;
    [SerializeField] private float _dayLightRange = -1;
    
    [Header("Night Light Values")]
    [SerializeField] private Color _nightLightColor = Color.black;
    [SerializeField] private float _nightLightIntensity = -1;
    [SerializeField] private float _nightLightRange = -1;
    
    private Coroutine _transitionCoroutine;

    public void StartTransition(bool transitionToDay, float duration, AnimationCurve curve)
    {
        if (_transitionCoroutine != null)
        {
            StopCoroutine(_transitionCoroutine);
        }

        _transitionCoroutine = StartCoroutine(TransitionLight(transitionToDay, duration, curve));
    }

    private IEnumerator TransitionLight(bool transitionToDay, float duration, AnimationCurve curve)
    {
        Color startColor = _light.color;
        float startIntensity = _light.intensity;
        float startRange = _light.range;
        
        Color targetColor = transitionToDay ? _dayLightColor : _nightLightColor;
        float targetIntensity = transitionToDay ? _dayLightIntensity : _nightLightIntensity;
        float targetRange = transitionToDay ? _dayLightRange : _nightLightRange;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float curveValue = curve.Evaluate(t);

            if (_dayLightColor != _nightLightColor)
                _light.color = Color.Lerp(startColor, targetColor, curveValue);
            
            if (!Mathf.Approximately(_dayLightIntensity, _nightLightIntensity)) 
                _light.intensity = Mathf.Lerp(startIntensity, targetIntensity, curveValue);
            
            if (!Mathf.Approximately(_dayLightRange, _nightLightRange))
                _light.range = Mathf.Lerp(startRange, targetRange, curveValue);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        if (_dayLightColor != _nightLightColor) _light.color = targetColor;
        if (!Mathf.Approximately(_dayLightIntensity, _nightLightIntensity))  _light.intensity = targetIntensity;
        if (!Mathf.Approximately(_dayLightRange, _nightLightRange)) _light.range = targetRange;

        _transitionCoroutine = null;
    }
}
