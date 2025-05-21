using System.Collections;
using MyUtilities;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private TextMeshProUGUI _dayText;
    private bool _isBackgroundImageSet = true;

    [Header("Transition Animations")]
    [SerializeField] private AnimationCurve _transitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private float _transitionDuration = 1f;

    [Header("Typewriter Effect")]
    [SerializeField] private bool _showTypewriterEffect;
    [SerializeField] private float _showTypewriterEffectDuration = 1f;
    [SerializeField] private float _timeBeforeUpdatingDayIndex = 1;

    [Header("Events")]
    public UnityEvent OnFadeOutComplete;

    private Coroutine _transitionCoroutine;
    private int _dayIndex;
    [SerializeField] private GameManager _gameManager;

    void Start()
    {
        if (!_gameManager)
        {
            _gameManager = ServiceLocator.Get<GameManager>();
        }
        
        StartTransition();
    }
    public void StartTransition()
    {
        if (_transitionCoroutine != null) StopCoroutine(_transitionCoroutine);
        _dayIndex = _gameManager.GameData.DayIndex;
        _dayText.alpha = 0f;
        if (_isBackgroundImageSet)
        {
            if (!_backgroundImage.gameObject.activeInHierarchy) _backgroundImage.gameObject.SetActive(true);
            if (!_dayText.gameObject.activeInHierarchy) _dayText.gameObject.SetActive(true);
            _transitionCoroutine = StartCoroutine(TypewriterEffectCoroutine());
            
        }
        else
        {
            _transitionCoroutine = StartCoroutine(FadeInCoroutine());
        }
        
    }

    private IEnumerator FadeInCoroutine()
    {
        if (!_backgroundImage.gameObject.activeInHierarchy) _backgroundImage.gameObject.SetActive(true);
        if (!_dayText.gameObject.activeInHierarchy) _dayText.gameObject.SetActive(true);
        float elapsed = 0f;
        Color startColor = _backgroundImage.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 1f);
        _backgroundImage.color = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsed < _transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = _transitionCurve.Evaluate(elapsed / _transitionDuration);
            _backgroundImage.color = Color.Lerp(new Color(startColor.r, startColor.g, startColor.b, 0f), endColor, t);
            if (!_showTypewriterEffect) _dayText.alpha = Mathf.Lerp(0f, 1, t);
            yield return null;
        }

        _backgroundImage.color = endColor;

        if (_showTypewriterEffect)
            yield return StartCoroutine(TypewriterEffectCoroutine());
        else
        {
            yield return new WaitForSeconds(_timeBeforeUpdatingDayIndex/2);
            _dayText.text = "Day " + ((_dayIndex == -1 ? _dayIndex+1 : _dayIndex) + 1);
            yield return new WaitForSeconds(_timeBeforeUpdatingDayIndex/2);
            StartFadeOut();
        }

        _isBackgroundImageSet = true;
    }

    private IEnumerator TypewriterEffectCoroutine()
    {
        string fullText = _dayText.text;
        _dayText.text = "";
        _dayText.alpha = 1f;

        float delay = _showTypewriterEffectDuration / Mathf.Max(fullText.Length, 1);

        for (int i = 0; i < fullText.Length; i++)
        {
            _dayText.text += fullText[i];
            yield return new WaitForSeconds(delay);
        }
        
        yield return new WaitForSeconds(_timeBeforeUpdatingDayIndex/2);
        _dayText.text = "Day " + ((_dayIndex == -1 ? _dayIndex+1 : _dayIndex) + 1);
        yield return new WaitForSeconds(_timeBeforeUpdatingDayIndex/2);
        StartFadeOut();
    }

    public void StartFadeOut()
    {
        if (_transitionCoroutine != null) StopCoroutine(_transitionCoroutine);
        _transitionCoroutine = StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        float elapsed = 0f;
        Color startImageColor = _backgroundImage.color;
        Color startTextColor = _dayText.color;

        while (elapsed < _transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = _transitionCurve.Evaluate(elapsed / _transitionDuration);
            float alpha = Mathf.Lerp(1f, 0f, t);

            _backgroundImage.color = new Color(startImageColor.r, startImageColor.g, startImageColor.b, alpha);
            _dayText.color = new Color(startTextColor.r, startTextColor.g, startTextColor.b, alpha);

            yield return null;
        }

        _backgroundImage.color = new Color(startImageColor.r, startImageColor.g, startImageColor.b, 0f);
        _dayText.color = new Color(startTextColor.r, startTextColor.g, startTextColor.b, 0f);
        if (_dayText.gameObject.activeInHierarchy) _dayText.gameObject.SetActive(false);
        if (_backgroundImage.gameObject.activeInHierarchy) _backgroundImage.gameObject.SetActive(false);
        _isBackgroundImageSet = false;
        OnFadeOutComplete?.Invoke();
    }
}
