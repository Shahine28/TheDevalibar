using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] PlayerInput _playerController;
    [SerializeField] InputActionReference _click;

    [SerializeField] NavMeshAgent _agent;

    [Header("clickable")]
    [SerializeField] LayerMask _Clickablelayers;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _click.action.performed += OnClick;
        _click.action.canceled += OnClick;

        _agent = GetComponent<NavMeshAgent>();
        OnClick(new());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnClick(InputAction.CallbackContext ctx)
    {
        var dir = ctx.ReadValue<Vector3>();

        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, _Clickablelayers))
        {
            _agent.destination = hit.point;
            
        }
    }

}
