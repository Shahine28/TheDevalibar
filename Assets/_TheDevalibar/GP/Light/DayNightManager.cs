using System.Collections;
using System.Collections.Generic;
using MyUtilities;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

public class DayNightManager : MonoBehaviour
{
    [SerializeField] private List<DayNightLightTransition> _dayNightLightTransitions;
    [SerializeField] private SpriteRenderer _bgSpriteRenderer;
    [SerializeField] private Color _daySpriteColor = Color.white;
    [SerializeField] private Color _nightSpriteColor = Color.white;
    
    [SerializeField] private float _dayNightTransitionDuration;
    [SerializeField] private AnimationCurve _dayNightTransitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Coroutine _dayNightTransitionCoroutine;

    void Awake()
    {
        ServiceLocator.Register(this);
    }

    [Button]
    public void StartToDayTransition()
    {
        StartTransition(true);
    }
    
    [Button]
    public void StartToNightTransition()
    {
        StartTransition(false);
    }
    
    private void StartTransition(bool transitionToDay)
    {
        if (_dayNightTransitionCoroutine != null)
        {
            StopCoroutine(_dayNightTransitionCoroutine);
        }

        StartCoroutine(DayNightTransitionCoroutine(transitionToDay));
    }

    private IEnumerator DayNightTransitionCoroutine(bool transitionToDay)
    {
        _dayNightLightTransitions.ForEach(x => 
            x.StartTransition(
                transitionToDay, 
                _dayNightTransitionDuration, 
                _dayNightTransitionCurve
                ));
        
        
        Color startColor = _bgSpriteRenderer.color;
        Color targetColor = transitionToDay ? _daySpriteColor : _nightSpriteColor ;
        
        
        float elapsedTime = 0f;
        while (elapsedTime < _dayNightTransitionDuration)
        {
            float t = elapsedTime / _dayNightTransitionDuration;
            float curveValue = _dayNightTransitionCurve.Evaluate(t);

            if (_nightSpriteColor != _daySpriteColor)
            {
                _bgSpriteRenderer.color = Color.Lerp(startColor, targetColor, curveValue);
            }
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        if (_nightSpriteColor != _daySpriteColor) _bgSpriteRenderer.color = targetColor;
        
        _dayNightTransitionCoroutine = null;
    }
}
