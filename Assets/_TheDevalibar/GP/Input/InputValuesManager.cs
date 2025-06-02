using System.Collections;
using MyUtilities;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class InputValuesManager : MonoBehaviour
{
    [Header("Input Scheme")]
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField, ReadOnly] private ControlScheme _curentControlScheme = ControlScheme.Mouse;
    public ControlScheme CurrentControlScheme => _curentControlScheme;
    public bool _isMouseUsed => _curentControlScheme == ControlScheme.Mouse;
    
    
    [Header("Scroll Value")]
    [SerializeField, ReadOnly] private int _scrollValue;
    public int ScrollValue => _scrollValue;
    
    
    
    [Header("Movement Value")]
    [SerializeField, ReadOnly, HideIf("_isMouseUsed")] private Vector2 _movementValue;
    public Vector2 MovementValue => _movementValue;
    
    [SerializeField, ReadOnly, HideIf("_isMouseUsed")] private Vector2 _deltaMovementValue; 
    public Vector2 DeltaMovementValue => _deltaMovementValue;
    private Coroutine _moveCoroutine;
    
    
    
    [Header("Mouse Value")]
    [SerializeField, ReadOnly, ShowIf("_isMouseUsed")] private Vector2 _mousePosition;
    public Vector2 MousePosition => _mousePosition;
    [SerializeField, ReadOnly, ShowIf("_isMouseUsed")] private Vector2 _deltaMousePosition;
    public Vector2 DeltaMousePosition => _deltaMousePosition;
    private Coroutine _mouseCoroutine;
    
    
    
    [FormerlySerializedAs("_isMiddleClickPressed")]
    [Header("Drag Click")] 
    [SerializeField, ReadOnly] private bool _isDragClickPressed;
    public bool IsDragClickPressed => _isDragClickPressed;
    
    
    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        ServiceLocator.Register(this);
    }

    public void OnControlSchemeChanged(UnityEngine.InputSystem.PlayerInput input)
    {
        string scheme = input.currentControlScheme;
        Debug.Log($"Contrôle changé. Nouveau schéma : {scheme}");
        
        switch (scheme)
        {
            case "Keyboard":
                _curentControlScheme = ControlScheme.Keyboard;
                break;

            case "Mouse":
                _curentControlScheme = ControlScheme.Mouse;
                break;
            case "Gamepad":
                _curentControlScheme = ControlScheme.Gamepad;
                break;
        }
    }
    public void OnInputZoom(InputAction.CallbackContext context)
    {
        _scrollValue = Mathf.Clamp((int)context.ReadValue<float>(), -1, 1);
    }

    public void OnInputMovement(InputAction.CallbackContext context)
    {
        if (!gameObject.activeInHierarchy) return;
        Vector2 movementValue = context.ReadValue<Vector2>();
        _deltaMovementValue = movementValue - _movementValue;
        _movementValue = movementValue;
        if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
        _moveCoroutine = StartCoroutine(InputMovementCoroutine());
    }

    public IEnumerator InputMovementCoroutine()
    {
        yield return new WaitForEndOfFrame();
        _deltaMovementValue = Vector2.zero;
    }

    public void OnInputPoint(InputAction.CallbackContext context)
    {
        if (!context.performed || !gameObject.activeInHierarchy) return;
        Vector2 pointPosition = context.ReadValue<Vector2>();
        _deltaMousePosition = pointPosition - _mousePosition;
        _mousePosition = pointPosition;
        if (_mouseCoroutine != null) StopCoroutine(_mouseCoroutine);
        _mouseCoroutine = StartCoroutine(InputPointCoroutine());
    }

    public IEnumerator InputPointCoroutine()
    {
        yield return new WaitForEndOfFrame();
        _deltaMousePosition = Vector2.zero;
    }

    public void OnInputMiddleClick(InputAction.CallbackContext context)
    {
        _isDragClickPressed = context.ReadValueAsButton();
    }
}

public enum ControlScheme
{
    Keyboard,
    Mouse,
    Gamepad
}