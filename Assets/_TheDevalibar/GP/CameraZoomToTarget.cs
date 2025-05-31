using UnityEngine;
using System.Collections;
using MyUtilities;
using Unity.Cinemachine;

public class CameraZoomToTarget : MonoBehaviour
{
    [Header("Zoom Settings")]
    [SerializeField] private float zoomDistance = 5f;
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] private float zoomSize = 1f;
    [SerializeField] private AnimationCurve zoomCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float verticalOffset = 0.5f;
    
    [SerializeField] private CinemachineCamera _cam;
    [SerializeField] private Collider _camContainer;
    private CameraMovementAndZoomControl _cameraMovementAndZoomControl;
    private Coroutine _zoomRoutine;

    private Vector3 _originalPosition;
    private float _originalSize;

    private void Awake()
    {
        _originalPosition = _camContainer.transform.position;
        _originalSize = _cam.Lens.OrthographicSize;
        _cameraMovementAndZoomControl = GetComponent<CameraMovementAndZoomControl>();
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
        Vector3 startPos = _camContainer.transform.position;
        float startSize = _cam.Lens.OrthographicSize;;
        Vector3 viewDirection = transform.forward.normalized;

        Renderer renderer = target.GetComponentInChildren<Renderer>(false);
        Vector3 targetCenter = renderer != null ? renderer.bounds.center : target.position;
        targetCenter += Vector3.up * verticalOffset;
        Vector3 endPos = targetCenter - viewDirection * zoomDistance;

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = zoomCurve.Evaluate(elapsed / transitionDuration);
            _cam.Lens.OrthographicSize = Mathf.Lerp(startSize, zoomSize, t);
            _camContainer.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        _camContainer.transform.position = endPos;
        _zoomRoutine = null;
    }

    private IEnumerator ZoomToOriginalRoutine()
    {
        Vector3 startPos = _camContainer.transform.position;
        float startSize = _cam.Lens.OrthographicSize;;

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = zoomCurve.Evaluate(elapsed / transitionDuration);
            _cam.Lens.OrthographicSize = Mathf.Lerp(startSize, _originalSize, t);
            _camContainer.transform.position = Vector3.Lerp(startPos, _originalPosition, t);
            
            yield return null;
        }
        _cameraMovementAndZoomControl.UpdateContainerScale();

        _camContainer.transform.position = _originalPosition;
        _zoomRoutine = null;
    }
}
