using System;
using MyUtilities;
using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class CameraMovementAndZoomControl : MonoBehaviour
{
    [Header("Camera Movement")]
    public float moveSpeed = 50f;
    
    [ReadOnly] public Vector3 CameraOriginalPosition;
    [SerializeField] private bool _useEdgeScrolling = false;
    [ShowIf("_useEdgeScrolling")]public int EdgeScrollSize = 20;
    [SerializeField] private bool _useDragPan = false;
    [ShowIf("_useDragPan")]public float DragPanSpeed = 1;
    
    
    [Header("Camera Zoom")]
    [SerializeField] private Transform _movingCameraTransform;
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private float _zoomSpeed = 25;
    [SerializeField] private float _zoomMinSize = 1f;
    private float _zoomMaxSize = 3;
    [SerializeField] private AnimationCurve _zoomCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private Transform _cameraContainerTransform;
    [SerializeField] private float _maxContainerScale;
    public bool CanZoom = true;

    
    private InputValuesManager _inputValuesManager;
    private Vector2 _inputDir;

    void Awake()
    {
        if (!_camera) _camera = GetComponent<CinemachineCamera>();
        CameraOriginalPosition = _camera.transform.position;
        _zoomMaxSize = _camera.Lens.OrthographicSize;
        CanZoom = true;
        ServiceLocator.Register(this);
    }

    void Start()
    {
        _inputValuesManager = ServiceLocator.Get<InputValuesManager>();
        UpdateContainerScale();
    }
    public void Update()
    {
        if (!_inputValuesManager) return;

        if (GetZoomPercentage() > 0.2f)
        {
            if (!_inputValuesManager._isMouseUsed && CanZoom)
            {
                HandleCameraMovement();
            }
            else
            {
                if (_useEdgeScrolling && CanZoom) HandleCameraMovementEdgeScrolling();
                if (_useDragPan && _inputValuesManager.IsDragClickPressed && CanZoom) HandleCameraMovementDragPan();
            }
        }
        else
        {
            if (!Mathf.Approximately(Vector3.Distance(transform.position, CameraOriginalPosition), 0f))
            {
                transform.position = Vector3.Lerp(transform.position, CameraOriginalPosition, Time.deltaTime * _zoomSpeed);
            }
        }

        if (CanZoom)
        {
            HandleCameraZoom();
        }
    }
    
    private void LateUpdate()
    {
        if (_inputDir == Vector2.zero)
        {
            if (!Mathf.Approximately(Vector3.Distance(transform.position, _movingCameraTransform.position), 0f))
            {
                transform.position = _movingCameraTransform.position;
            }
        }
    }
    
    

#region Camera Movement
    private void HandleCameraMovement()
    {
        _inputDir = _inputValuesManager.MovementValue;
       _inputDir.x = Mathf.Clamp(_inputDir.x, -1, 1);
       _inputDir.y = Mathf.Clamp(_inputDir.y, -1, 1);
        
        Vector3 moveDir = transform.up *_inputDir.y + transform.right *_inputDir.x;
        transform.position += moveDir  * moveSpeed * Time.deltaTime;
    }
    
    private void HandleCameraMovementEdgeScrolling()
    {
        _inputDir = Vector2.zero;
        Vector2 mousePos = _inputValuesManager.MousePosition;
        if (mousePos.x < EdgeScrollSize)_inputDir.x = -1f;
        if (mousePos.y < EdgeScrollSize)_inputDir.y = -1f;
        if (mousePos.x > Screen.width - EdgeScrollSize)_inputDir.x = 1f;
        if (mousePos.y > Screen.height - EdgeScrollSize)_inputDir.y = 1f;
        
        
        Vector3 moveDir =  transform.up *_inputDir.y + transform.right *_inputDir.x;
        transform.position += moveDir  * moveSpeed * Time.deltaTime;
    }

    private void HandleCameraMovementDragPan()
    {
        _inputDir = Vector2.zero;
        
        Vector2 movementDelta = _inputValuesManager.CurrentControlScheme == ControlScheme.Mouse 
            ? _inputValuesManager.DeltaMousePosition 
            : _inputValuesManager.DeltaMovementValue;
        
       _inputDir.x = movementDelta.x * DragPanSpeed;
       _inputDir.y = movementDelta.y * DragPanSpeed;
       
        Vector3 moveDir =   transform.up *_inputDir.y + transform.right *_inputDir.x;
        transform.position += moveDir  * moveSpeed * Time.deltaTime;
    }
#endregion
#region CameraZoom

private void HandleCameraZoom()
{
    float targetSize = _camera.Lens.OrthographicSize;
    if (_inputValuesManager.ScrollValue < 0)
    {
        targetSize  += _zoomSpeed;
    }
    else if (_inputValuesManager.ScrollValue > 0)
    {
        targetSize  -= _zoomSpeed;
    }
    targetSize  = Mathf.Clamp(targetSize , _zoomMinSize, _zoomMaxSize);
    _camera.Lens.OrthographicSize = Mathf.Lerp(_camera.Lens.OrthographicSize, targetSize , Time.deltaTime * _zoomSpeed);
    UpdateContainerScale();
}

public void UpdateContainerScale()
{
    _cameraContainerTransform.localScale = 
        Vector3.Lerp(Vector3.zero,
              new Vector3(_maxContainerScale, _maxContainerScale, _maxContainerScale)
            ,GetZoomPercentage());
}

private float GetZoomPercentage()
{
    return Mathf.InverseLerp(_zoomMaxSize, _zoomMinSize, _camera.Lens.OrthographicSize);
}
#endregion


}
