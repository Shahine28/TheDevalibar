using System.Collections;
using System.Linq;
using MyUtilities;
using NaughtyAttributes;
using UnityEngine;


public class CharacterFollowerBehavior : MonoBehaviour
{
    [Header("Character Follower")]
    [SerializeField] private CharacterFollower _characterFollower;
    private CharacterBehavior _characterBehavior;
    [SerializeField] private Transform _characterToFollowTransform;

    private float _followThreshold => _characterBehavior != null
        ? Vector3.Distance(
            _characterToFollowTransform.transform.position, _characterBehavior.transform.position)
        : 0.01f;
    
    
    [Header("Dijkstra")] [SerializeField] private NodeManager _nodeManager;
    [SerializeField] private DijkstraManager _dijkstraManager;
    [SerializeField] private DijkstraPathFollower _dijkstraPathFollower;


    [Header("Movement")]
    private int _barNodeExit;
    [SerializeField, ReadOnly] private  int _startNodeIndex = -1;
    [SerializeField, ReadOnly] private  int _lastNodeIndex = -1;
    [SerializeField, ReadOnly] private  int _nextNodeIndex = -1;
    private Table _usedTable;
    private Chair _usedChair;
    private bool _canFollowCharacter = true;
    private bool _canGoToPoint = true;
    private Vector3 _lastTargetPosition;
    private bool _isFollowing = false;
    private Coroutine _followCoroutine;
    
    [Header("Animations")]
    [SerializeField] private AnimationManager _animationManager;

    [SerializeField] private GameObject _wheelChair;
    [SerializeField] private string  _wheelChairDisabiltyName = "Mobilité réduite sévère";
    
    [SerializeField] private GameObject _blindCane;
    [SerializeField] private string  _blindCaneDisabiltyName = "Déficience visuelle sévère";
    
    
    [SerializeField] private CharacterSpawnManager _characterSpawnManager;

    private bool _hasBeenInit;
    
    void Start()
    {
        Init();   
    }
    
    void Init()
    {
        if (_hasBeenInit) return;
        _hasBeenInit = true;
        _startNodeIndex = -1;
        _lastNodeIndex = -1;
        _nextNodeIndex = -1;
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
        _dijkstraPathFollower.OnFollowPathEnd += HandlePathEnd;
        
        
        if (_characterSpawnManager == null)
        {
            _characterSpawnManager = ServiceLocator.Get<CharacterSpawnManager>();
        }

        if (!_animationManager)
        {
            _animationManager = GetComponent<AnimationManager>();
        }
        _animationManager.OnCharacterStandUp += OnCharacterStandUp;
        _animationManager.OnCharacterSitDown += OnCharacterSitDown;
    }
    
    void Update()
    {
        if (_canFollowCharacter)
        {
            FollowCheck();
        }
        else if (_isFollowing && !_canGoToPoint)
        {
            StopMovement();
        }
    }
    
    public void Initialize(CharacterBehavior characterBehavior)
    {
        Init();
        _characterBehavior = characterBehavior;
        if (_characterBehavior == null)
        {
            Debug.LogWarning("Character Behavior is null in CharacterFollowerBehavior");
            return;
        }

        _barNodeExit = _characterBehavior.ExitNodeId;
        _characterToFollowTransform = _characterBehavior.CharacterFollowerPointToFollow;
        _characterFollower = _characterBehavior?.Character?.CharacterFollower;
        
        _animationManager?.gameObject.SetActive(true);
        if (_characterFollower != null)
        {
            if (_characterFollower?.CharacterMesh == null)
            {
                Debug.LogWarning("Character Follower is null in CharacterFollowerBehavior");
                return;
            }
            _animationManager?.SetAnimation(_characterFollower.CharacterMesh,
                _characterFollower.CharacterMaterial,
                _characterFollower.HasSpecificRuntimeAnimationController ? _characterFollower.CharacterRuntimeAnimatorController : null);
        }
        else
        {
            NPCMeshMaterialController npcMeshMaterialController = _characterSpawnManager.GetRandomNPCAssets();
            _animationManager?.SetAnimation(npcMeshMaterialController.Mesh, npcMeshMaterialController.Material);
        }
        
    }
    
#region Follow
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
    
    public void StartFollowing()
    {
        _canFollowCharacter = true;
    }

    public void StopFollowing()
    {
        _canFollowCharacter = false;
    }
    
    private void FollowCheck()
    {
        if (_characterToFollowTransform == null) return;

        float distanceMoved = Vector3.Distance(_characterToFollowTransform.position, _lastTargetPosition);
        // Si la target bouge suffisamment, on relance le suivi avec délai
        if (distanceMoved > _followThreshold)
        {
            _lastTargetPosition = _characterToFollowTransform.position;

            if (_followCoroutine != null)
                StopCoroutine(_followCoroutine);

            _followCoroutine = StartCoroutine(FollowWithDelay());
        }
    }

    private IEnumerator FollowWithDelay()
    {
        StartMovement();

        while (Vector3.Distance(transform.position, _characterToFollowTransform.position) > _followThreshold)
        {
            Vector3 targetPos = _characterToFollowTransform.position;
            Vector3 direction = (targetPos - transform.position).normalized;
            
            // Déplacement
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * _dijkstraPathFollower.MoveSpeed);

            // Orientation vers la direction de déplacement (sur l'axe Y uniquement)
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                _dijkstraPathFollower.ObjectToRotate.transform.rotation = Quaternion.Slerp(
                    _dijkstraPathFollower.ObjectToRotate.transform.rotation,
                    targetRotation, Time.deltaTime * 10f);
            }
            yield return null;
        }

        StopMovement();
    }
    
    private IEnumerator FollowToPosition(Vector3 targetPos)
    {
        _canGoToPoint = true;
        StartMovement();

        while (Vector3.Distance(transform.position, targetPos) > 0.001f)
        {
            Vector3 direction = (targetPos - transform.position).normalized;
            
            // Déplacement
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * _dijkstraPathFollower.MoveSpeed);
            
            // Orientation vers la direction de déplacement (sur l'axe Y uniquement)
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                _dijkstraPathFollower.ObjectToRotate.transform.rotation = Quaternion.Slerp(
                    _dijkstraPathFollower.ObjectToRotate.transform.rotation,
                    targetRotation, Time.deltaTime * 10f);
            }
            yield return null;
        }
        
        StopMovement();
        _canGoToPoint = false;
        _lastNodeIndex = GetClosestNode();
        MoveToBarExit();
    }

#endregion
#region Movement
    public void StartMovement()
    {
        if (!_isFollowing)
        {
            _isFollowing = true;
            _animationManager.StartMovement();
        }
    }

    public void StopMovement()
    {
        if (_isFollowing)
        {
            _isFollowing = false;
            _animationManager.StopMovement();
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
            _lastNodeIndex = _characterBehavior != null ? _characterBehavior.LastNodeIndex : GetClosestNode();
        }
        _nodeManager.SetNewStartAndEndNodes(_lastNodeIndex, NodeId);
        _nextNodeIndex = NodeId;
        _dijkstraPathFollower.FollowPath();
        _animationManager?.StartMovement();
    }

    public void MoveToBarExit()
    {
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
        _animationManager.StopMovement();
        
        if (_nextNodeIndex != -1)
        {
            _lastNodeIndex = _nextNodeIndex;
            _nextNodeIndex = -1;
        }
        
        if (_usedTable != null && _usedChair != null && _usedChair.ChairClosestNodeID == _lastNodeIndex)
        {
            _usedChair.MoveChairToOccupiedPosition();
            _animationManager?.SitDown();
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
        _animationManager.StandUp();
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





