using UnityEngine;
using System.Collections;
using MyUtilities;
using TMPro.EditorUtilities;

[RequireComponent(typeof(Camera))]
public class CameraZoomToTarget : MonoBehaviour
{
    [Header("Zoom Settings")]
    [SerializeField] private float zoomDistance = 5f;
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] private float zoomFOV = 30f;
    [SerializeField] private float zoomSize = 1f;
    [SerializeField] private AnimationCurve zoomCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Camera _cam;
    private Coroutine _zoomRoutine;

    private Vector3 _originalPosition;
    private float _originalFOV;
    private float _originalSize;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
        _originalPosition = transform.position;
        _originalFOV = _cam.fieldOfView;
        _originalSize = _cam.orthographicSize;
        ServiceLocator.Register(this);
    }

    public void ZoomTo(Transform target)
    {
        if (_zoomRoutine != null) StopCoroutine(_zoomRoutine);
        _zoomRoutine = StartCoroutine(ZoomToTargetRoutine(target));
    }

    public void ResetCamera()
    {
        if (_zoomRoutine != null) StopCoroutine(_zoomRoutine);
        _zoomRoutine = StartCoroutine(ZoomToOriginalRoutine());
    }

    private IEnumerator ZoomToTargetRoutine(Transform target)
    {
        Vector3 startPos = transform.position;
        float startFOV = _cam.fieldOfView;
        float startSize = _cam.orthographicSize;
        // Direction actuelle de la caméra
        Vector3 viewDirection = transform.forward.normalized;

        // Nouvelle position = cible - direction * distance
        Vector3 endPos = target.position - viewDirection * zoomDistance;

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = zoomCurve.Evaluate(elapsed / transitionDuration);
            
            transform.position = Vector3.Lerp(startPos, endPos, t);
            _cam.fieldOfView = Mathf.Lerp(startFOV, zoomFOV, t);
            _cam.orthographicSize = Mathf.Lerp(startSize, zoomSize, t);

            yield return null;
        }

        transform.position = endPos;
        _cam.fieldOfView = zoomFOV;
        _zoomRoutine = null;
    }

    private IEnumerator ZoomToOriginalRoutine()
    {
        Vector3 startPos = transform.position;
        float startFOV = _cam.fieldOfView;
        float startSize = _cam.orthographicSize;

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = zoomCurve.Evaluate(elapsed / transitionDuration);

            transform.position = Vector3.Lerp(startPos, _originalPosition, t);
            _cam.fieldOfView = Mathf.Lerp(startFOV, _originalFOV, t);
            _cam.orthographicSize = Mathf.Lerp(startSize, _originalSize, t);
            yield return null;
        }

        transform.position = _originalPosition;
        _cam.fieldOfView = _originalFOV;
        _zoomRoutine = null;
    }
}
