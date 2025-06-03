using System.Linq;
using MyUtilities;
using NaughtyAttributes;
using UnityEngine;


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
    
    [Header("Animations")]
    [SerializeField] private AnimationManager _animationManager;

    [SerializeField] private GameObject _wheelChair;
    [SerializeField] private string  _wheelChairDisabiltyName = "Mobilité réduite sévère";
    
    [SerializeField] private GameObject _blindCane;
    [SerializeField] private string  _blindCaneDisabiltyName = "Déficience visuelle sévère";

    
    
    
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
        
        SetCharacterDisabilities();
    }
    
    
    public void Initialize(CharacterFollower characterFollower)
    {
        _characterFollower = characterFollower;
        if (_characterFollower.CharacterMesh == null)
        {
            _animationManager?.gameObject.SetActive(false);
        }
        else
        {
            _animationManager?.gameObject.SetActive(true);
            
            _animationManager?.SetAnimation(_characterFollower.CharacterMesh,
                _characterFollower.CharacterMaterial,
                _characterFollower.HasSpecificRuntimeAnimationController ? _characterFollower.CharacterRuntimeAnimatorController : null);
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
            _usedChair = null;
        }
    }


    private void HandlePathEnd()
    {
        throw new System.NotImplementedException();
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
        _nodeManager.SetNewStartAndEndNodes(_lastNodeIndex, NodeId);
        _nextNodeIndex = NodeId;
        _dijkstraPathFollower.FollowPath();
        _animationManager?.StartMovement();
    }
}    





