using System;
using System.Collections.Generic;
using System.Linq;
using MyUtilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class CharacterMovement : CharacterComponent
{
    
    [Header("Dijkstra")]
    [SerializeField] private NodeManager _nodeManager;
    [SerializeField] private DijkstraManager _dijkstraManager;
    [SerializeField] private DijkstraPathFollower _dijkstraPathFollower;
    
    
    [Header("Movement")] 
    private FloorLevel _floorLevel; 
    [SerializeField] private int _barNodeId = 40;
    public int BarNodeId => _barNodeId;
    [SerializeField] private int _exitNodeId = 1;
    public int ExitNodeId => _exitNodeId;
    [SerializeField, ReadOnly] private  int _startNodeIndex;
    [SerializeField, ReadOnly] private  int _lastNodeIndex;
    public int LastNodeIndex => _lastNodeIndex;
    [SerializeField, ReadOnly] private  int _nextNodeIndex;
    
    private TablesManager _tablesManager;
    public Table UsedTable;
    private Chair _usedChair;
    public Chair UsedChair => _usedChair;
    
    
    private ShowHideUI _showHideUI;
    private ElevatorManager _elevatorManager;
    
    [Header("CharacterState")]
    [SerializeField, ReadOnly] private CharacterState _characterState = CharacterState.Idle;


    private CharacterSpawnManager _characterSpawnManager;
    private bool _isInit;

#region OnEnable/OnDisable
    private void OnEnable()
    {
        if (!_character) // ça veut dire que c'est pas initialisé
        {
            InitVar();
        }
        if (_dijkstraPathFollower)
        {
            _dijkstraPathFollower.OnFollowPathEnd += HandlePathEnd;
        }
        
        if (_elevatorManager)
        {
            _elevatorManager.OnElevatorMovementEnd += OnElevatorMovementEnd;
        }
        
        if (_characterAnimationManager)
        {
            _characterAnimationManager.OnCharacterStandUp += OnCharacterStandUp;
            _characterAnimationManager.OnCharacterSitDown += OnCharacterSitDown;
        }
    }

    private void OnDisable()
    {
        if (!_character) // ça veut dire que c'est pas initialisé
        {
            InitVar();
        }
        if (_dijkstraPathFollower)
        {
            _dijkstraPathFollower.OnFollowPathEnd -= HandlePathEnd;
        }
        
        if (_elevatorManager)
        {
            _elevatorManager.OnElevatorMovementEnd -= OnElevatorMovementEnd;
        }
        
        if (_characterAnimationManager)
        {
            _characterAnimationManager.OnCharacterStandUp -= OnCharacterStandUp;
            _characterAnimationManager.OnCharacterSitDown -= OnCharacterSitDown;
        }
    }
#endregion
#region UnityDefault

    void Start()
    {
        if (_character)
        {
            MoveToNode(_barNodeId);
            // MoveToBestTable(); // Pour les test
        }
        else
        {
            MoveToBestTable();
        }
    }
    public override void Init(CharacterBehavior characterBehavior)
    {
        base.Init(characterBehavior);
        InitVar();
        _startNodeIndex = GetClosestNode();
        _lastNodeIndex = _startNodeIndex;
    }

    void InitVar()
    {
        ServiceLocator.RequireComponent(this, ref _dijkstraPathFollower, "No Dijkstra Path follower found");
        ServiceLocator.RequireComponent(this, ref _characterAnimationManager, "No Animation Manager found");
        
        ServiceLocator.RequireService(this, ref _nodeManager, "No Node Manager in Scene");
        ServiceLocator.RequireService(this, ref _dijkstraManager, "No Dijkstra Manager in Scene");
        ServiceLocator.RequireService(this, ref _elevatorManager, "No Elevator Manager in Scene");
        
        ServiceLocator.RequireService(this, ref _tablesManager, "No Tables Manager in Scene");
        ServiceLocator.RequireService(this, ref _showHideUI, "No Show Hide UI in Scene");
        ServiceLocator.RequireService(this, ref _characterSpawnManager, "No Character Spawn Manager in scene");
    }
    

#endregion
    
    private void RotateTowardsTarget(Transform self, Transform target, float rotationSpeed = 5f)
    {
        if (self == null || target == null) return;

        Vector3 direction = (target.position - self.position).normalized;

        if (direction == Vector3.zero) return;

        self.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }
    
    private void MoveToNode(int NodeId)
    {
        _nodeManager.SetNewStartAndEndNodes(_lastNodeIndex, NodeId);
        _nextNodeIndex = NodeId;
        _dijkstraPathFollower.FollowPath();
        _characterAnimationManager?.StartMovement();
    }

    private void MoveToBarExit()
    {
        _characterBehavior?.SetBubbleSpeech(true);
        if (_floorLevel == FloorLevel.GroundFloor)
        {
            MoveToNode(_exitNodeId);
        }
        else
        {
            MoveToElevatorWaitingPosition();
        }
        _characterFeedback.FeedBackImage.gameObject.SetActive(true);
    }
    
    private void MoveToElevatorWaitingPosition()
    {
        if (_elevatorManager == null)
        {
            Debug.LogError("No Elevator manager in scene");
            return;
        }
        int elevatorWaitingPositionNodeIndex = 
            _floorLevel == FloorLevel.GroundFloor
            ? _elevatorManager.ElevatorWaitingPositionGroundFloorNodeId
            : _elevatorManager.ElevatorWaitingPositionUpperFloorNodeId; 
        MoveToNode(elevatorWaitingPositionNodeIndex);
    }

    private void OnElevatorMovementEnd()
    {
        int elevatorWaitingPositionNodeIndex = 
            _floorLevel == FloorLevel.GroundFloor
            ? _elevatorManager.ElevatorWaitingPositionGroundFloorNodeId
            : _elevatorManager.ElevatorWaitingPositionUpperFloorNodeId;
        int elevatorNodeIndex = 
            _floorLevel == FloorLevel.GroundFloor
            ? _elevatorManager.GroundFloorNodeId
            : _elevatorManager.UpperFloorNodeId;
        int nextElevatorNodeIndex = 
            _floorLevel != FloorLevel.GroundFloor
                ? _elevatorManager.GroundFloorNodeId
                : _elevatorManager.UpperFloorNodeId;

        if (_elevatorManager.FloorLevel == _floorLevel
            && _lastNodeIndex == elevatorWaitingPositionNodeIndex)
        {
            MoveToNode(elevatorNodeIndex);
            return;
        }
        if (_lastNodeIndex == elevatorNodeIndex)
        {
            _lastNodeIndex = nextElevatorNodeIndex;
            _floorLevel = _elevatorManager.FloorLevel; 
            if (UsedTable != null)
            {
                transform.SetParent(_characterSpawnManager.CharacterSpawnPoint);
                _elevatorManager.currentPassenger = null;
                MoveToNode(UsedTable.TableNodeNumber);
            }
            else
            {
                transform.SetParent(_characterSpawnManager.CharacterSpawnPoint);
                _elevatorManager.currentPassenger = null;
                MoveToBarExit();
            }
        }
    }
    
    public void MoveToBestTable()
    {
        //Récupère les tables accessibles (déjà triées par priorité)
        List<Table> bestTables = _tablesManager.GetAccessibleTables(_characterDisabilityHandler.CharacterDisability);

        if (bestTables.Count == 0)
        {
            Debug.Log("No tables found, customers need to leave");
            _characterFeedback.SetCustomerFeedback(null);
            MoveToBarExit();
            return;
        }
        
        // Détermine la meilleure table selon la distance
        Table bestTable = FindBestTable(bestTables);
        if (!bestTable)
        {
            _characterFeedback.SetCustomerFeedback(null);
            MoveToBarExit();
            _characterFollowerHandler?.StopCharacterFollower();
            if ( _characterFollowerHandler != null && _characterFollowerHandler.CharacterFollowerBehavior) _characterFollowerHandler.CharacterFollowerBehavior.MoveToBarExit();
            return;
        }
        UsedTable = bestTable;
        _usedChair = UsedTable.GetFirstAvailableChair();
        _usedChair.IsChairOccupied = true;
        
        
        _characterFeedback.SetCustomerFeedback(bestTable);
        // Debug.Log($"Best table chosen: {bestTable.name}");
        
        if (_floorLevel == UsedTable.TableFloorLevel ||(_characterDisabilityHandler.CharacterConstraint != null && _characterDisabilityHandler.CharacterConstraint.CanTakeStairs) || _characterDisabilityHandler.CharacterConstraint == null )
        {
            MoveToNode(_usedChair.ChairClosestNodeID);
            
            _characterFollowerHandler.StopCharacterFollower();
            _characterFollowerHandler.ForceCharacterFollowerToGoToTable(UsedTable);
            // _dijkstraManager.EnableConstraint(_characterDisability);
        }
        else if (_elevatorManager.IsElevatorBuyed)
        {
            MoveToElevatorWaitingPosition();
        }
        else
        {
            _characterFeedback.SetCustomerFeedback(null);
            MoveToBarExit();
            _characterFollowerHandler?.StopCharacterFollower();
            if ( _characterFollowerHandler != null && _characterFollowerHandler.CharacterFollowerBehavior) _characterFollowerHandler.CharacterFollowerBehavior.MoveToBarExit();
        }
    }
    
    public Table FindBestTable(List<Table> bestTables)
    {
        if (bestTables.Count == 1)
        {
            return bestTables[0];
        }
       
        if (_characterDisabilityHandler.CharacterDisability == String.Empty)
        {
            Table firstTable = bestTables[0];
            bool hasFirstTableContraintAdaptabilty = firstTable.constraintDict.values.FirstOrDefault(x => x);

            Table secondTable = bestTables[1];
            bool hasSecondTableContraintAdaptabilty = secondTable.constraintDict.values.FirstOrDefault(x => x);

            if (hasFirstTableContraintAdaptabilty && hasSecondTableContraintAdaptabilty)
            {
                List<Table> tables = new List<Table> { firstTable, secondTable };
                return tables
                    .OrderBy(t => Vector3.Distance(transform.position, _nodeManager.nodes[t.TableNodeNumber].position))
                    .First();
            }
            return firstTable;
            
        }

        Constraint constraint = _nodeManager.constraints.FirstOrDefault(c => c.name == _characterDisabilityHandler.CharacterDisability);
        if (constraint == null)
        {
            Debug.LogError("No valid constraint in character");
            return null;
        }
        if (!constraint.IsBlockingConstraint)
        {
            
            Table firstTable = bestTables[0];
            bool isFirstTableConstraintCompatible = Enumerable.Range(0, firstTable.constraintDict.keys.Count)
                .Where(i => firstTable.constraintDict.values[i]) // garde les indices où la contrainte est accessible
                .Select(i => firstTable.constraintDict.keys[i])  // récupère les clés correspondantes
                .FirstOrDefault() == _characterDisabilityHandler.CharacterDisability;

            Table secondTable = bestTables[1];
            bool isSecondTableConstraintCompatible = Enumerable.Range(0, secondTable.constraintDict.keys.Count)
                .Where(i => secondTable.constraintDict.values[i]) // garde les indices où la contrainte est accessible
                .Select(i => secondTable.constraintDict.keys[i])  // récupère les clés correspondantes
                .FirstOrDefault() == _characterDisabilityHandler.CharacterDisability;
            if (isFirstTableConstraintCompatible && isSecondTableConstraintCompatible)
            {
                List<Table> tables = new List<Table> { firstTable, secondTable };
                return tables
                    .OrderBy(t => Vector3.Distance(transform.position, _nodeManager.nodes[t.TableNodeNumber].position))
                    .First();
            }
            return firstTable; 
            
        }
        // Ma liste ne contient forcément que des tables adapté au handicap bloquant et disponible
        return bestTables
            .OrderBy(t => Vector3.Distance(transform.position, _nodeManager.nodes[t.TableNodeNumber].position))
            .First();
        
    }
    
    private void HandlePathEnd()
    {
        Debug.Log("Le chemin est terminé !");
        
        _characterAnimationManager.StopMovement();
        
        
        if (_nextNodeIndex != -1)
        {
            _lastNodeIndex = _nextNodeIndex;
            _nextNodeIndex = -1;
        }
        
        if (_lastNodeIndex == _barNodeId)
        {
            
            _characterState = CharacterState.AtTheBar;
            if (_characterSpawnManager != null)
            {
                _characterSpawnManager.CanSpawnCharacter = false;
            }
            _characterDialogue.DialogueButton?.gameObject.SetActive(true);
            if ( _characterDialogue.DialogueButton != null) EventSystem.current.SetSelectedGameObject( _characterDialogue.DialogueButton.gameObject);
        }
        else if (_lastNodeIndex == _exitNodeId)
        {
            _characterState = CharacterState.Idle;
            ReviewManager reviewManager = ServiceLocator.Get<ReviewManager>();
            reviewManager?.AddReview(_characterFeedback.CustomerFeedback, _characterDisabilityHandler.CharacterDisability, _character);
            
            GameManager gameManager = ServiceLocator.Get<GameManager>();
            if (gameManager)
            {
                gameManager.GameData.Gold += _characterFeedback.GetTipValue();
                gameManager.UpdateGoldValue();
            }
            
            
            if (_characterSpawnManager && _characterSpawnManager.HaveAllCharactersAndNCPBeenSpawned && _characterSpawnManager.CharacterSpawnPoint.childCount.Equals(1))
            {
                _tablesManager?.ShowUpgradeButtonTables();
                _showHideUI?.ShowUI();
                reviewManager?.SetReviews();
            }
            
            Destroy(gameObject);
        }
        else
        {
            if (UsedTable != null && _usedChair != null && _usedChair.ChairClosestNodeID == _lastNodeIndex)
            {
                _characterState = CharacterState.AtTheBestTable;

                if (_characterDisabilityHandler.CharacterConstraint is { CanTakeStairs: false } || _characterDisabilityHandler.CharacterDisability == _characterAssets.WheelChairDisabiltyName)
                {
                    _usedChair.HideChair();
                }
                else
                {
                    _usedChair.MoveChairToOccupiedPosition();
                }
                _characterAnimationManager?.SitDown();
                RotateTowardsTarget(_dijkstraPathFollower.ObjectToRotate.transform, UsedTable.transform);
                // StartWaiting();
                return;
            }
            

            if (_lastNodeIndex == _elevatorManager.ElevatorWaitingPositionGroundFloorNodeId)
            {
                _elevatorManager.MoveToGroundFloor();
                return;
            }

            if (_lastNodeIndex == _elevatorManager.ElevatorWaitingPositionUpperFloorNodeId)
            {
                _elevatorManager.MoveToUpperFloor();
                return;
            }
            
            bool isOnElevatorGroundFloor = _lastNodeIndex == _elevatorManager.GroundFloorNodeId;
            bool isOnElevatorUpperFloor = _lastNodeIndex == _elevatorManager.UpperFloorNodeId;
            bool hasTable = UsedTable != null;

            if (isOnElevatorGroundFloor)
            {
                if (hasTable) // On veut monter
                {
                    transform.SetParent(_elevatorManager.elevatorPlateform.transform);
                    _elevatorManager.currentPassenger = _characterBehavior;
                    _elevatorManager.CloseDoor(_floorLevel);
                }
                return;
            }

            if (isOnElevatorUpperFloor) // On veut descendre
            {
                transform.SetParent(_elevatorManager.elevatorPlateform.transform);
                _elevatorManager.currentPassenger = _characterBehavior;
                _elevatorManager.CloseDoor(_floorLevel);
            }

        }
    }

    private int GetClosestNode()
    {
        NodeManager nodeManager = ServiceLocator.Get<NodeManager>();
        if (!nodeManager) return -1;
        float distance = 100;
        int NearestNode = -1;
        foreach (NodeDijkstra node in nodeManager.nodes)
        {
            if (Vector3.Distance(transform.position, node.position) < distance)
            {
                distance = Vector3.Distance(transform.position, node.position);
                NearestNode = node.NodeID;
            }
        }
        return NearestNode;
    }
    
    
    private void OnCharacterSitDown()
    {
        _characterWaiting.StartWaiting();
    }

    private void OnCharacterStandUp()
    {
        if (UsedTable)
        {
            UsedTable = null;
        }
        
        if (_characterDisabilityHandler.CharacterConstraint is { CanTakeStairs: false } || _characterDisabilityHandler.CharacterDisability == _characterAssets.WheelChairDisabiltyName)
        {
            _usedChair.ShowChair();
        }
        else
        {
            _usedChair.MoveChairToUnoccupiedPosition();
        }
        
        if (_usedChair)
        {
            _usedChair = null;
        }
        
        MoveToBarExit();
    }
}
