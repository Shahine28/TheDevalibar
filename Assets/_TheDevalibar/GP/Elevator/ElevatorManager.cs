using System;
using System.Collections;
using MyUtilities;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class ElevatorManager : MonoBehaviour
{
    [SerializeField] private GameObject _elevatorPrefab;   
    [SerializeField] private GameObject _elevatorPlateformGameObject;
    public GameObject elevatorPlateform => _elevatorPlateformGameObject;
    [SerializeField] private Animator _elevatorAnimator;
    
    public GameObject ElevatorGameObject => _elevatorPlateformGameObject;
    [SerializeField, ReadOnly] private FloorLevel _floorLevel = FloorLevel.GroundFloor;
    
    public FloorLevel FloorLevel=>_floorLevel;
    [Header("Node")] 
    [SerializeField] private int _GroundFloorNodeId;
    public int GroundFloorNodeId => _GroundFloorNodeId;
    [SerializeField] private int _UpperFloorNodeId;
    
    [SerializeField] private int _elevatorWaitingPositionGroundFloorNodeId = 1;
    public int ElevatorWaitingPositionGroundFloorNodeId => _elevatorWaitingPositionGroundFloorNodeId;
    [SerializeField] private int _elevatorWaitingPositionUpperFloorNodeId = 1;
    public int ElevatorWaitingPositionUpperFloorNodeId => _elevatorWaitingPositionUpperFloorNodeId;
    public int UpperFloorNodeId => _UpperFloorNodeId;
    
    [Header("Movement")]
    [SerializeField] private Vector3 _groundFloorPosition;
    [SerializeField] private Vector3 _upperFloorPosition;
    [SerializeField] private float _movementSpeed = 1f;
    [SerializeField] private AnimationCurve _movementCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Coroutine _movementCoroutine;

    [SerializeField, ReadOnly] private bool _isElevatorBuyed;
    public bool IsElevatorBuyed => _isElevatorBuyed;
    
    [SerializeField] private Upgrade _elevatorUpgrade;
    public Upgrade ElevatorUpgrade => _elevatorUpgrade;

    public CharacterBehavior currentPassenger;
    
    
    public event Action OnElevatorMovementEnd;

    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    private void Start()
    {
        _elevatorPlateformGameObject.transform.localPosition = _groundFloorPosition;
        _floorLevel = FloorLevel.GroundFloor;
        _movementCoroutine = null;
        _elevatorUpgrade.OnUpgrade.AddListener(BuyElevator);
    }

    public void OpenDoor(FloorLevel floorLevel)
    {
        if (floorLevel == FloorLevel.GroundFloor)
        {
            _elevatorAnimator?.SetTrigger("OpenFrontDoor");
        }
        else
        {
            _elevatorAnimator?.SetTrigger("OpenBackDoor");
        }
    }
    
    public void CloseDoor(FloorLevel floorLevel)
    {
        if (floorLevel == FloorLevel.GroundFloor)
        {
            _elevatorAnimator?.SetTrigger("CloseFrontDoor");
        }
        else
        {
            _elevatorAnimator?.SetTrigger("CloseBackDoor");
        }
    }

    public void OnDoorClosed()
    {
        if (currentPassenger != null)
        {
            if (_floorLevel == FloorLevel.GroundFloor)
            {
                MoveToUpperFloor();
            }
            else
            {
                MoveToGroundFloor();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CloseDoor(_floorLevel);
    }



    public void OnDoorOpened()
    {
        OnElevatorMovementEnd?.Invoke();
    }

    [Button]
    public void BuyElevator()
    {
        _isElevatorBuyed = true;
        _elevatorPrefab?.gameObject.SetActive(true);
        _elevatorPlateformGameObject?.SetActive(true);
    }
    
    [Button]
    public void SellElevator()
    {
        _isElevatorBuyed = false;
        _elevatorPrefab?.gameObject.SetActive(false);
        _elevatorPlateformGameObject?.SetActive(false);
    }

    [Button]
    public void MoveToUpperFloor()
    {
        if (_floorLevel == FloorLevel.UpperFloor)
        {
            if (_movementCoroutine == null)
            {
                OpenDoor(_floorLevel);
            }
            return;
        }

        if (_movementCoroutine != null)
        {
            StopCoroutine(_movementCoroutine);
            _movementCoroutine = null;
        }
        _movementCoroutine = StartCoroutine(MoveElevator(_upperFloorPosition, FloorLevel.UpperFloor));
    }

    [Button]
    public void MoveToGroundFloor()
    {
        if (_floorLevel == FloorLevel.GroundFloor)
        {
            if (_movementCoroutine == null)
            {
                OpenDoor(_floorLevel);
            }
            return;
        }

        if (_movementCoroutine != null)
        {
            StopCoroutine(_movementCoroutine);
            _movementCoroutine = null;
        }
        _movementCoroutine = StartCoroutine(MoveElevator(_groundFloorPosition, FloorLevel.GroundFloor));
    }

    private IEnumerator MoveElevator(Vector3 targetPosition, FloorLevel destinationLevel)
    {
        Transform elevator = _elevatorPlateformGameObject.transform;
        Vector3 start = elevator.localPosition;
        Vector3 end = targetPosition;

        float distance = Vector3.Distance(start, end);
        float duration = distance / _movementSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float curvedT = _movementCurve.Evaluate(t);

            elevator.localPosition = Vector3.Lerp(start, end, curvedT);
            yield return null;
        }

        elevator.localPosition = end;
        _floorLevel = destinationLevel;
        _movementCoroutine = null;
        OpenDoor(destinationLevel);
    }
}

