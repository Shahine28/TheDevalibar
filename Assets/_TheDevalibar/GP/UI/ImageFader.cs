using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ImageFader : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image _image;

    [Header("Fade Settings")]
    [SerializeField] private float _fadeSpeed = 1f;
    [SerializeField] private AnimationCurve _fadeCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Events")]
    public UnityEvent OnFadeInStarted;
    public UnityEvent OnFadeOutFinished;

    private Coroutine _fadeCoroutine;

    public void FadeIn()
    {
        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeRoutine(0f, 1f, OnFadeInStarted, null));
    }

    public void FadeOut()
    {
        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeRoutine(1f, 0f, null, OnFadeOutFinished));
    }

    private IEnumerator FadeRoutine(float from, float to, UnityEvent onStart, UnityEvent onComplete)
    {
        if (_image == null)
        {
            Debug.LogError("Image is not assigned.");
            yield break;
        }

        onStart?.Invoke();

        float elapsed = 0f;
        float duration = 1f / _fadeSpeed;

        Color color = _image.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float alpha = Mathf.Lerp(from, to, _fadeCurve.Evaluate(t));
            _image.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        _image.color = new Color(color.r, color.g, color.b, to);
        onComplete?.Invoke();
        _fadeCoroutine = null;
    }
}

