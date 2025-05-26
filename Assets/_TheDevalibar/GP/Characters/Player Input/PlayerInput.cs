using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInput : MonoBehaviour, IPointerDownHandler, ISelectHandler
{
    [SerializeField] PlayerInput _playerController;
    [SerializeField] InputActionReference _Interact;
    [SerializeField] InputActionReference _joystickMove;
    [SerializeField] InputActionReference _mousePosition;
    [SerializeField] InputActionReference _pauseMenu;

    [Header("UI Manager")]
    [SerializeField] UIManager _menu;
    [SerializeField] bool _isPaused = false;

    //[Header("Player Param")]
    //[SerializeField] float speed;

    [Header("Cursor")]
    [SerializeField] GameObject _cursor;
    private Vector2 _mousePos;
    //private Vector2 _moveInputValue;

    void Start()
    {
        //_click.action.performed += OnClick;
        //_click.action.canceled += OnClick;
        _pauseMenu.action.performed += OnPause;
        _pauseMenu.action.canceled += OnPause;

    }

    void Update()
    {
        _mousePos = _mousePosition.action.ReadValue<Vector2>();
        _cursor.transform.position = _mousePos;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("coucou");
        //OnClick();
    }

    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("test");
    }

    private void OnInteract()
    {
        //var dir = ctx.ReadValue<Vector2>();
        var dir = _mousePosition.action.ReadValue<Vector2>();

        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(dir), out hit, 100))
        {
            //_agent.destination = hit.point;
            Debug.Log("test");
        }
    }

    //private void OnMove(InputAction.CallbackContext ctx , InputValue val)
    //{
    //    _moveInputValue = val.Get<Vector2>();
    //    Debug.Log(_moveInputValue);
    //    Vector2 result = _moveInputValue * speed * Time.fixedDeltaTime;
    //    _cursor.transform.position = result;
    //}


    void OnPause(InputAction.CallbackContext ctx)
    {
        if (_isPaused == false)
        {
            _menu.OnPause();
            _isPaused = true;
        }
        else
        {
            _menu.resume();
            _isPaused = false;
        }
    }

}
