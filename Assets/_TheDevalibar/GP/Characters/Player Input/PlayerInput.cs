using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInput : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] PlayerInput _playerController;
    [SerializeField] InputActionReference _click;
    [SerializeField] InputActionReference _joystickMove;

    //[Header("Player Param")]
    //[SerializeField] float speed;

    [Header("Cursor")]
    [SerializeField] GameObject _cursor;

    private Vector2 _moveInputValue;

    void Start()
    {
        _click.action.performed += OnClick;
        _click.action.canceled += OnClick;

    }

    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("coucou");
        OnClick(new());
    }

    private void OnClick(InputAction.CallbackContext ctx)
    {
        var dir = ctx.ReadValue<Vector2>();
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
        {
            //_agent.destination = hit.point;
            Debug.Log("test");
        }
    }

    private void OnMove(InputAction.CallbackContext ctx , InputValue val)
    {
        _moveInputValue = val.Get<Vector2>();
        Debug.Log(_moveInputValue);
        Vector2 result = _moveInputValue * speed * Time.fixedDeltaTime;
        _cursor.transform.position = result;
    }

}
