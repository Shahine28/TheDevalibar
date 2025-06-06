using System.Linq;
using MyUtilities;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;


public class CharacterFollowerBehavior : MonoBehaviour
{
    [Header("Character Follower")]
    [SerializeField] private CharacterFollower _characterFollower;
    private bool _isCharacterAssigned => _characterFollower != null;
    [SerializeField, Range(0, 100), HideIf("_isCharacterAssigned")]
    private float _disabilityChance = 20;
    [SerializeField] private Transform _characterToFollowTransform;
    
    [Header("Character Follower Disabilty")]
    [SerializeField, ReadOnly] private string _characterDisability = string.Empty;
    private Constraint _characterConstraint;
    
    
    [Header("Dijkstra")] [SerializeField] private NodeManager _nodeManager;
    [SerializeField] private DijkstraManager _dijkstraManager;
    [SerializeField] private DijkstraPathFollower _dijkstraPathFollower;
    
    
    [Header("Movement")] 
    [SerializeField] private int _barNodeId = 40;
    [SerializeField] private int _exitNodeId = 1;
    [SerializeField, ReadOnly] private  int _startNodeIndex;
    [SerializeField, ReadOnly] private  int _lastNodeIndex;
    [SerializeField, ReadOnly] private  int _nextNodeIndex;
    private Table _usedTable;
    private Chair _usedChair;
    
    [FormerlySerializedAs("_animationManager")]
    [Header("Animations")]
    [SerializeField] private CharacterAnimationManager characterAnimationManager;

    [SerializeField] private GameObject _wheelChair;
    [SerializeField] private string  _wheelChairDisabiltyName = "Mobilité réduite sévère";
    
    [SerializeField] private GameObject _blindCane;
    [SerializeField] private string  _blindCaneDisabiltyName = "Déficience visuelle sévère";

    
    
    [SerializeField] private CharacterSpawnManager _characterSpawnManager;

    private bool _hasBeenInit;
    private bool _isMovingToBarExit => _nextNodeIndex == _barNodeExit;
    
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

    void Start()
    {
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
        
        
        if (_characterSpawnManager == null)
        {
            _characterSpawnManager = ServiceLocator.Get<CharacterSpawnManager>();
        }

        if (!characterAnimationManager)
        {
            characterAnimationManager = GetComponent<CharacterAnimationManager>();
        }
        characterAnimationManager.OnCharacterStandUp += OnCharacterStandUp;
        characterAnimationManager.OnCharacterSitDown += OnCharacterSitDown;
    }
    
    
    public void Initialize(CharacterFollower characterFollower)
    {
        _characterFollower = characterFollower;
        if (_characterFollower.CharacterMesh == null)
        {
            _animationManager?.gameObject.SetActive(false);
        }

        _barNodeExit = _characterBehavior.CharacterMovement.ExitNodeId;
        _characterToFollowTransform = _characterBehavior.CharacterFollowerHandler.CharacterFollowerPointToFollow;
        _characterFollower = _characterBehavior?.Character?.CharacterFollower;
        
        characterAnimationManager?.gameObject.SetActive(true);
        if (_characterFollower != null)
        {
            if (_characterFollower?.CharacterMesh == null)
            {
                Debug.LogWarning("Character Follower is null in CharacterFollowerBehavior");
                return;
            }
            characterAnimationManager?.SetAnimation(_characterFollower.CharacterMesh,
                _characterFollower.CharacterMaterial,
                _characterFollower.HasSpecificRuntimeAnimationController ? _characterFollower.CharacterRuntimeAnimatorController : null);
        }
        else
        {
            NPCMeshMaterialController npcMeshMaterialController = _characterSpawnManager.GetRandomNPCAssets();
            characterAnimationManager?.SetAnimation(npcMeshMaterialController.Mesh, npcMeshMaterialController.Material);
        }
        
    }
    
    
    public void SetCharacterDisabilities()
    {
        if (_characterFollower)
        {
            _characterDisability = _characterFollower.constraintDict.keys
                .Select((key, i) => new { key, isActive = _characterFollower.constraintDict.values[i] })
                .FirstOrDefault(x => x.isActive)?.key ?? "";
        }
        else
        {
            float randomNumber = Random.Range(0, 100);
            if (randomNumber <= _disabilityChance)
            {
                if (!_nodeManager) _nodeManager = ServiceLocator.Get<NodeManager>();
                int randomDisabiltyIndex = Random.Range(0, _nodeManager.constraints.Count-1);
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
    
    private void OnCharacterSitDown()
    {
        
    }

    private void OnCharacterStandUp()
    {
        _usedChair?.MoveChairToUnoccupiedPosition();
        
        if (_usedChair)
        {
            _isFollowing = true;
            characterAnimationManager.StartMovement();
        }
    }


    private void HandlePathEnd()
    {
        if (_isFollowing)
        {
            _isFollowing = false;
            characterAnimationManager.StopMovement();
        }
    }
    
    private void RotateTowardsTarget(Transform self, Transform target, float rotationSpeed = 5f)
    {
        if (self == null || target == null) return;

        Vector3 direction = (target.position - self.position).normalized;

        if (direction == Vector3.zero) return;

        self.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }
    
    
    private void MoveToNode(int NodeId)
    {
        Init();// Sécurité
        if (_lastNodeIndex == -1)
        {
            _lastNodeIndex = _characterBehavior != null ? _characterBehavior.CharacterMovement.LastNodeIndex : GetClosestNode();
        }
        _nodeManager.SetNewStartAndEndNodes(_lastNodeIndex, NodeId);
        _nextNodeIndex = NodeId;
        _dijkstraPathFollower.FollowPath();
        characterAnimationManager?.StartMovement();
    }

    public void MoveToBarExit()
    {
        if (_isMovingToBarExit) return;
        MoveToNode(_barNodeExit);
    }

    public void MoveToSameTableAsCharacter(Table table)
    {
        if (table == null) return;
        _usedTable = table;
        _usedChair = table.GetFirstAvailableChair();
        if (_usedChair == null)
        {
            Debug.LogWarning("No available chair found in table");
            return;
        }
        _usedChair.IsChairOccupied = true;
        MoveToNode(_usedChair.ChairClosestNodeID);
        
    }
    
     private void HandlePathEnd()
    {
        characterAnimationManager.StopMovement();
        
        if (_nextNodeIndex != -1)
        {
            _lastNodeIndex = _nextNodeIndex;
            _nextNodeIndex = -1;
        }
        
        if (_usedTable != null && _usedChair != null && _usedChair.ChairClosestNodeID == _lastNodeIndex)
        {
            _usedChair.MoveChairToOccupiedPosition();
            characterAnimationManager?.SitDown();
            RotateTowardsTarget(_dijkstraPathFollower.ObjectToRotate.transform, _usedTable.transform);
        }

        if (_lastNodeIndex == _barNodeExit)
        {
            Destroy(gameObject);
        }
    }
#endregion
#region OnCharacterStandUp/SitDown
    
    public void OnCharacterSitDown()
    {
        
    }
    
    public void ForceCharacterToStandUp()
    {
        characterAnimationManager.StandUp();
    }

    public void OnCharacterStandUp()
    {
        _usedChair?.MoveChairToUnoccupiedPosition();
        
        if (_usedChair)
        {
            _usedChair = null;
        }

        if (_usedTable != null)
        {
            if (_followCoroutine != null)
                StopCoroutine(_followCoroutine);

            _followCoroutine = StartCoroutine(FollowToPosition(new Vector3(_usedTable.ExitTransform.position.x, transform.position.y,
                _usedTable.ExitTransform.position.z)));
        }
        
    }
    
#endregion
    
    
}    





