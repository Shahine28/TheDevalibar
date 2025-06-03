using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _TheDevalibar.GP.Characters;
using MyUtilities;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterBehavior : MonoBehaviour
{
    [Header("Character Behavior")]
    [SerializeField] private Character _character;
    public Character Character => _character;
    [SerializeField, ReadOnly] private string _characterDisability = string.Empty;
    private Constraint _characterConstraint;
    private CharacterSpawnManager _characterSpawnManager;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private bool _isCharacterAssigned => _character != null;
    [SerializeField, Range(0, 100), HideIf("_isCharacterAssigned")]
    private float _disabilityChance = 20;
        
    [Header("Character Follower")] 
    [SerializeField] private bool _hasAFollower;
    [SerializeField] private CharacterFollowerBehavior _characterFollowerBehavior;
    [SerializeField] private Transform _characterFollowerPointToFollow;
    
    
    [Header("Dijkstra")]
    [SerializeField] private NodeManager _nodeManager;
    [SerializeField] private DijkstraManager _dijkstraManager;
    [SerializeField] private DijkstraPathFollower _dijkstraPathFollower;

    [Header("WaitingTime")] 
    [SerializeField] private Vector2 _waitingTimeRange;
    private Coroutine _waitRoutine;
    private bool _interrupted = false;
    private bool _isPaused = false;

    [Header("Movement")] 
    private FloorLevel _floorLevel; 
    [SerializeField] private int _barNodeId = 40;
    [SerializeField] private int _exitNodeId = 1;
    [SerializeField, ReadOnly] private  int _startNodeIndex;
    [SerializeField, ReadOnly] private  int _lastNodeIndex;
    [SerializeField, ReadOnly] private  int _nextNodeIndex;
    private TablesManager _tablesManager;
    private Table _usedTable;
    private Chair _usedChair;
    private ShowHideUI _showHideUI;
    
    
    [Header("CharacterState")]
    [SerializeField, ReadOnly] private CharacterState _characterState = CharacterState.Idle;
    
    
    [Header("Dialogue")]
    [SerializeField] private DialogueManager _dialogueManager;
    [SerializeField] private Button _dialogueButton;
    
    [Header("Character Feedback")]
    [SerializeField, ReadOnly] private CustomersFeedback _customerFeedback = CustomersFeedback.Good;
    [SerializeField] private CharacterFeedback _characterFeedback;
    [SerializeField] private RawImage FeedBackImage;
    
    
    private GameManager _gameManager;
    private ElevatorManager _elevatorManager;
    
    
    [Header("Bubble Speech")]
    [SerializeField] private GameObject _bubbleSpeechPanel;
    [SerializeField] private TextMeshProUGUI _bubbleSpeechText;
    private BubbleSpeechManager _bubbleSpeechManager;
    
    [Header("Animations")]
    [SerializeField] private AnimationManager _animationManager;

    [SerializeField] private GameObject _wheelChair;
    [SerializeField] private string  _wheelChairDisabiltyName = "Mobilité réduite sévère";
    
    [SerializeField] private GameObject _blindCane;
    [SerializeField] private string  _blindCaneDisabiltyName = "Déficience visuelle sévère";
    

    ///  Camera
    private CameraMovementAndZoomControl _cameraMovementAndZoomControl;

#region OnEnable/OnDisable
    public void OnEnable()
    {
        if (_dijkstraPathFollower)
        {
            _dijkstraPathFollower.OnFollowPathEnd += HandlePathEnd;
        }
    }

    public void OnDisable()
    {
        if (_dijkstraPathFollower)
        {
            _dijkstraPathFollower.OnFollowPathEnd -= HandlePathEnd;
        }
    }
#endregion
#region UnityDefault
    public void Initialize(Character character)
    {
        _character = character;
        if (_character.CharacterMesh == null)
        {
            _spriteRenderer.sprite = _character.CharacterSprite; 
            _spriteRenderer.gameObject.SetActive(true);// temporary
            _animationManager?.gameObject.SetActive(false);
        }
        else
        {
            _spriteRenderer?.gameObject.SetActive(false);
            _animationManager?.gameObject.SetActive(true);
            
            _animationManager?.SetAnimation(_character.CharacterMesh,
                _character.CharacterMaterial,
                _character.HasSpecificRuntimeAnimationController ? _character.CharacterRuntimeAnimatorController : null);
        }
    }

    public void SetNPC()
    {  
        NPCMeshMaterialController npcMeshMaterialController = _characterSpawnManager.GetRandomNPCAssets();
        SetNPC(npcMeshMaterialController.Mesh, npcMeshMaterialController.Material, npcMeshMaterialController.AnimatorController);
    }
    public void SetNPC(Mesh npcMesh, Material npcMaterial, RuntimeAnimatorController runtimeAnimatorController)
    {
        _animationManager?.SetAnimation(npcMesh,
            npcMaterial,
            _characterDisability == _wheelChairDisabiltyName ? runtimeAnimatorController : null);
    }

    void Start()
    {   
        _gameManager = ServiceLocator.Get<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("Game manager is null.");
        }
        if (!_dijkstraManager)
        {
            _dijkstraManager = ServiceLocator.Get<DijkstraManager>();
        }
        if (!_nodeManager)
        {
            _nodeManager = ServiceLocator.Get<NodeManager>();
        }

        if (!_dijkstraPathFollower)
        {
            _dijkstraPathFollower = GetComponent<DijkstraPathFollower>();
        }

        if (!_dialogueManager)
        {
            _dialogueManager = ServiceLocator.Get<DialogueManager>();
        }
        
        _dialogueManager._onDialogueStart += PauseWaiting;
        _dialogueManager._onDialogueEnd += ResumeWaiting;

        _elevatorManager = ServiceLocator.Get<ElevatorManager>();
        if (_elevatorManager == null)
        {
            Debug.LogError("No Elevator manager in scene");
        }
        else
        {
            _elevatorManager.OnElevatorMovementEnd += OnElevatorMovementEnd;
        }

        _characterSpawnManager = ServiceLocator.Get<CharacterSpawnManager>();
        _tablesManager = ServiceLocator.Get<TablesManager>();
        
        if (_hasAFollower) _characterFollowerBehavior.gameObject.SetActive(true);
        
        _startNodeIndex = GetClosestNode();
        _lastNodeIndex = _startNodeIndex;
        FeedBackImage.gameObject.SetActive(false);
        UpdateFeedBackImage();
        SetCharacterDisabilities();
        if (_character == null) SetNPC();
        _characterConstraint = GetCharacterConstraint();
        _bubbleSpeechManager = ServiceLocator.Get<BubbleSpeechManager>();
        if (_bubbleSpeechManager == null)
        {
            Debug.LogError("No Bubble speech manager in scene");
        }
        if (_character)
        {
            MoveToNode(_barNodeId);
            // MoveToBestTable(); // Pour les test
        }
        else
        {
            _spriteRenderer?.gameObject.SetActive(false);
            MoveToBestTable();
        }

        
        _dialogueButton?.onClick.AddListener(StartCharacterDialogue);
        
        _showHideUI = ServiceLocator.Get<ShowHideUI>();
        if (_showHideUI == null)
        {
            Debug.LogError("No show hide UI found");
        }

        if (_animationManager == null)
        {
            _animationManager = GetComponent<AnimationManager>();
        }

        _animationManager.OnCharacterStandUp += OnCharacterStandUp;
        _animationManager.OnCharacterSitDown += OnCharacterSitDown;
        
        _cameraMovementAndZoomControl = ServiceLocator.Get<CameraMovementAndZoomControl>();
    }
#endregion
#region Constraint&Disability
    public void SetCharacterDisabilities()
    {
        if (_character)
        {
            _characterDisability = _character.constraintDict.keys
                .Select((key, i) => new { key, isActive = _character.constraintDict.values[i] })
                .FirstOrDefault(x => x.isActive)?.key ?? "";
        }
        else
        {
            float randomNumber = UnityEngine.Random.Range(0, 100);
            if (randomNumber <= _disabilityChance)
            {
                if (!_nodeManager) _nodeManager = ServiceLocator.Get<NodeManager>();
                int randomDisabiltyIndex = UnityEngine.Random.Range(0, _nodeManager.constraints.Count-1);
                _characterDisability = _nodeManager.constraints[randomDisabiltyIndex].name;
            }
            else
            {
                _characterDisability = string.Empty;
            }
        }

        if (_characterDisability != _wheelChairDisabiltyName)
        {
            _wheelChair?.gameObject.SetActive(false);
        }
        if (_characterDisability != _blindCaneDisabiltyName)
        {
            _blindCane?.gameObject.SetActive(false);
        }
    }

    public Constraint GetCharacterConstraint()
    {
        if (_characterDisability == "")
        {
            return null;
        }

        return _nodeManager.constraints.FirstOrDefault(x => x.name == _characterDisability);
    }
#endregion
    
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

#region Waiting

    private void OnCharacterSitDown()
    {
        StartWaiting();
    }

    private void OnCharacterStandUp()
    {
        if (_characterConstraint is { CanTakeStairs: false } || _characterDisability == _wheelChairDisabiltyName)
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
    
    
    public void StartWaiting()
    {
        _interrupted = false;
        _waitRoutine = StartCoroutine(WaitAtTheBar());
    }

    public void PauseWaiting()
    {
        if (_waitRoutine != null)
        {
            _isPaused = true;
        }
    }

    public void ResumeWaiting()
    {
        if (_waitRoutine != null && _isPaused)
        {
            _isPaused = false;
        }
    }
    public void CancelWaiting()
    {
        _interrupted = true;

        if (_waitRoutine != null)
            StopCoroutine(_waitRoutine);
    }

    private IEnumerator WaitAtTheBar()
    {
        float waitTime = UnityEngine.Random.Range(_waitingTimeRange.x, _waitingTimeRange.y);
        float elapsed = 0f;
        float deltaBubbleSpeech = waitTime / 4;

        while (elapsed < waitTime)
        {
            while (_isPaused) yield return null;
            if (elapsed >= deltaBubbleSpeech && elapsed < waitTime - deltaBubbleSpeech && !_bubbleSpeechPanel.gameObject.activeInHierarchy) // On le fait apparaite
            {
                SetBubbleSpeech(false);
            }
            else if (elapsed >= waitTime - deltaBubbleSpeech && _bubbleSpeechPanel.gameObject.activeInHierarchy) // On le fait disparaitre
            {
                _bubbleSpeechPanel.gameObject.SetActive(false);
            }
            if (_interrupted)
            {
                Debug.Log("Waiting at the bar was interrupted.");
                if (_bubbleSpeechPanel.gameObject.activeInHierarchy) _bubbleSpeechPanel.gameObject.SetActive(false);
                // MoveToBarExit();
                _animationManager.StandUp();
                if (_usedTable)
                {
                    // _usedTable.IsUsedByCustomer = false;
                    _usedTable = null;
                }
                yield break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Finished waiting at the bar.");
        // MoveToBarExit();
        _animationManager.StandUp();
        
        if (_usedTable)
        {
            // _usedTable.IsUsedByCustomer = false;
            _usedTable = null;
        }
        
    }
    
#endregion
#region Movement

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
        _animationManager?.StartMovement();
    }

    private void MoveToBarExit()
    {
        SetBubbleSpeech(true);
        if (_floorLevel == FloorLevel.GroundFloor)
        {
            MoveToNode(_exitNodeId);
        }
        else
        {
            MoveToElevatorWaitingPosition();
        }
        FeedBackImage.gameObject.SetActive(true);
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
            if (_usedTable != null)
            {
                transform.SetParent(_characterSpawnManager.CharacterSpawnPoint);
                _elevatorManager.currentPassenger = null;
                MoveToNode(_usedTable.TableNodeNumber);
            }
            else
            {
                transform.SetParent(_characterSpawnManager.CharacterSpawnPoint);
                _elevatorManager.currentPassenger = null;
                MoveToBarExit();
            }
        }
    }
    
    private void MoveToBestTable()
    {
        //Récupère les tables accessibles (déjà triées par priorité)
        List<Table> bestTables = _tablesManager.GetAccessibleTables(_characterDisability);

        if (bestTables.Count == 0)
        {
            Debug.Log("No tables found, customers need to leave");
            SetCustomerFeedback(null);
            MoveToBarExit();
            return;
        }
        
        // Détermine la meilleure table selon la distance
        Table bestTable = FindBestTable(bestTables);
        _usedTable = bestTable;
        _usedChair = _usedTable.GetFirstAvailableChair();
        _usedChair.IsChairOccupied = true;
        // _usedTable.IsUsedByCustomer = true;
        SetCustomerFeedback(bestTable);
        Debug.Log($"Best table chosen: {bestTable.name}");


        if (_floorLevel == _usedTable.TableFloorLevel || _characterConstraint.CanTakeStairs)
        {
            MoveToNode(_usedChair.ChairClosestNodeID);
            // _dijkstraManager.EnableConstraint(_characterDisability);
        }
        else if (_elevatorManager.IsElevatorBuyed)
        {
            MoveToElevatorWaitingPosition();
        }
        else
        {
            SetCustomerFeedback(null);
            MoveToBarExit();
        }
    }
    
    public Table FindBestTable(List<Table> bestTables)
    {
        if (bestTables.Count == 1)
        {
            return bestTables[0];
        }
       
        if (_characterDisability == String.Empty)
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

        Constraint constraint = _nodeManager.constraints.FirstOrDefault(c => c.name == _characterDisability);
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
                .FirstOrDefault() == _characterDisability;

            Table secondTable = bestTables[1];
            bool isSecondTableConstraintCompatible = Enumerable.Range(0, secondTable.constraintDict.keys.Count)
                .Where(i => secondTable.constraintDict.values[i]) // garde les indices où la contrainte est accessible
                .Select(i => secondTable.constraintDict.keys[i])  // récupère les clés correspondantes
                .FirstOrDefault() == _characterDisability;
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
        
        _animationManager.StopMovement();
        
        
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
            _dialogueButton?.gameObject.SetActive(true);
            if (_dialogueButton != null) EventSystem.current.SetSelectedGameObject(_dialogueButton.gameObject);
        }
        else if (_lastNodeIndex == _exitNodeId)
        {
            _characterState = CharacterState.Idle;
            ReviewManager reviewManager = ServiceLocator.Get<ReviewManager>();
            reviewManager?.AddReview(_customerFeedback, _characterDisability, _character);
            
            GameManager gameManager = ServiceLocator.Get<GameManager>();
            if (gameManager)
            {
                gameManager.GameData.Gold += GetTipValue();
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
            if (_usedTable != null && _usedChair != null && _usedChair.ChairClosestNodeID == _lastNodeIndex)
            {
                _characterState = CharacterState.AtTheBestTable;

                if (_characterConstraint is { CanTakeStairs: false } || _characterDisability == _wheelChairDisabiltyName)
                {
                    _usedChair.HideChair();
                }
                else
                {
                    _usedChair.MoveChairToOccupiedPosition();
                }
                _animationManager?.SitDown();
                RotateTowardsTarget(_dijkstraPathFollower.ObjectToRotate.transform, _usedTable.transform);
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
            bool hasTable = _usedTable != null;

            if (isOnElevatorGroundFloor)
            {
                if (hasTable) // On veut monter
                {
                    transform.SetParent(_elevatorManager.elevatorPlateform.transform);
                    _elevatorManager.currentPassenger = this;
                    _elevatorManager.CloseDoor(_floorLevel);
                }
                return;
            }

            if (isOnElevatorUpperFloor) // On veut descendre
            {
                transform.SetParent(_elevatorManager.elevatorPlateform.transform);
                _elevatorManager.currentPassenger = this;
                _elevatorManager.CloseDoor(_floorLevel);
            }

        }
    }
#endregion
#region Feedbacks
    private void SetCustomerFeedback(Table table)
    {
        if (table == null)
        {
            _customerFeedback = CustomersFeedback.Bad;
            UpdateFeedBackImage();
            return;
        }

        // Aucun handicap → satisfait
        if (string.IsNullOrEmpty(_characterDisability))
        {
            _customerFeedback = CustomersFeedback.Good;
            return;
        }

        // Handicap présent → vérifie si table adaptée
        bool isTableAdapted = table.constraintDict.ContainsKey(_characterDisability) && table.constraintDict[_characterDisability];

        _customerFeedback = isTableAdapted ? CustomersFeedback.Good : CustomersFeedback.Average;
        UpdateFeedBackImage();
    }

    private void UpdateFeedBackImage()
    {
        if (!FeedBackImage || !_characterFeedback)
        {
            Debug.LogError("No feedback image found.");
            return;
        }

        Color color = Color.white;
        Texture texture = null;

        switch (_customerFeedback)
        {
            case CustomersFeedback.Good:
                color = _characterFeedback.GoodFeedbackColor;
                texture = _characterFeedback.GoodFeedbackSprite;
                break;

            case CustomersFeedback.Average:
                color = _characterFeedback.AverageFeedbackColor;
                texture = _characterFeedback.AverageFeedbackSprite;
                break;

            case CustomersFeedback.Bad:
                color = _characterFeedback.BadFeedbackColor;
                texture = _characterFeedback.BadFeedbackSprite;
                break;
        }

        if (_characterFeedback.useColorFeedback)
        {
            FeedBackImage.color = color;
        }
        else
        {
            FeedBackImage.texture = texture;
        }
    }

    private int GetTipValue()
    {
        switch (_customerFeedback)
        {
            case CustomersFeedback.Good:
                return _characterFeedback.GoodTipsValue;
            case CustomersFeedback.Average:
                return _characterFeedback.AverageTipsValue;
            case CustomersFeedback.Bad:
                return _characterFeedback.BadTipsValue;
            default:
                break;
        }

        return 0;
    }
    
#endregion
#region BubbleSpeech

private void SetBubbleSpeech(bool isCustomerLeaving)
{
    string speech = _character != null
        ? _bubbleSpeechManager.GetSpeech(_customerFeedback, isCustomerLeaving, _character.BubbleSpeech)
        : _bubbleSpeechManager.GetSpeech(_customerFeedback, isCustomerLeaving);
    if (speech == string.Empty)
    {
        Debug.LogWarning("No bubble speech found.");
        return;
    }
    _bubbleSpeechText.text = speech;
    _bubbleSpeechPanel.gameObject.SetActive(true);
}
#endregion
#region Dialogue
    private void StartCharacterDialogue()
    {
        if (!_dialogueManager) return;
        _dialogueManager?.InitCharacterDialogue(this, false, _gameManager != null ? _gameManager.GameData.LanguageCode : CodeLanguage.French);
        _dialogueButton?.gameObject.SetActive(false);
        
        _cameraMovementAndZoomControl.CanZoom = false;
    }
    

    public void OnDialogueEnd()
    {
        if (_characterSpawnManager)
        {
            _characterSpawnManager.CanSpawnCharacter = true;
        }
        _cameraMovementAndZoomControl.CanZoom = true;
        MoveToBestTable();
    }
    #endregion

}



public enum CharacterState
{
    Idle,
    AtTheBar,
    AtTheBestTable
}

public enum CustomersFeedback
{
    Good,
    Average,
    Bad,
}


