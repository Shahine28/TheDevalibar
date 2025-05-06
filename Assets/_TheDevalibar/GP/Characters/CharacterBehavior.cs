using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _TheDevalibar.GP.Characters;
using AYellowpaper.SerializedCollections;
using MyUtilities;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = Unity.Mathematics.Random;

public class CharacterBehavior : MonoBehaviour
{


    [FormerlySerializedAs("character")]
    [Header("Character Behavior")]
    [SerializeField] private Character _character;
    public Character Character => _character;
    [SerializeField, ReadOnly] private string _characterDisability;
    private bool _isCharacterAssigned => _character != null;

    [SerializeField, Range(0, 100), HideIf("_isCharacterAssigned")]
    private float _disabilityChance = 20;
    
    [Header("Dijkstra")]
    [SerializeField] private NodeManager _nodeManager;
    [SerializeField] private DijkstraManager _dijkstraManager;
    [SerializeField] private DijkstraPathFollower _dijkstraPathFollower;

    [Header("WaitingTime")] 
    [SerializeField] private Vector2 _waitingTimeRange;
    private Coroutine _waitRoutine;
    private bool _interrupted = false;

    [Header("Movement")] 
    [SerializeField] private int _BarNodeId = 40;

    [SerializeField] private int _BarExitNodeId = 1;
    [SerializeField, ReadOnly] private  int _startNodeIndex;
    [SerializeField, ReadOnly] private  int _lastNodeIndex;
    [SerializeField, ReadOnly] private  int _nextNodeIndex;
    private TablesManager _tablesManager;
    private Table _usedTable;
    
    
    [Header("CharacterState")]
    [SerializeField, ReadOnly] private CharacterState _characterState = CharacterState.Idle;
    
    [Header("Dialogue")]
    [SerializeField] private DialogueManager _dialogueManager;
    
    [Header("Character Feedback")]
    [SerializeField, ReadOnly] private CustomersFeedback _customerFeedback = CustomersFeedback.Good;
    [SerializeField] private CharacterFeedback _characterFeedback;
    [SerializeField] private RawImage FeedBackImage;

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

    public void Initialize(Character character)
    {
        _character = character;
    }

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
        }
    }
    void Start()
    {
        if (!_dijkstraManager)
        {
            _dijkstraManager = ServiceLocator.Get<DijkstraManager>();
            if (_dijkstraPathFollower)
            {
                _dijkstraPathFollower.OnFollowPathEnd += HandlePathEnd;
            }
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
        _tablesManager = ServiceLocator.Get<TablesManager>();
        _startNodeIndex = GetClosestNode();
        _lastNodeIndex = _startNodeIndex;
        FeedBackImage.gameObject.SetActive(false);
        UpdateFeedBackImage();
        SetCharacterDisabilities();
        if (_character)
        {
            MoveToBar();
        }
        else
        {
            MoveToBestTable();
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
                NearestNode = nodeManager.nodes.IndexOf(node);
            }
        }
        return NearestNode;
    }

#region Waiting
    public void StartWaiting()
    {
        _interrupted = false;
        _waitRoutine = StartCoroutine(WaitAtTheBar());
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

        while (elapsed < waitTime)
        {
            if (_interrupted)
            {
                Debug.Log("Waiting at the bar was interrupted.");
                MoveToBarExit();
                if (_usedTable)
                {
                    _usedTable.IsUsedByCustomer = false;
                    _usedTable = null;
                }
                yield break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Finished waiting at the bar.");
        MoveToBarExit();
        if (_usedTable)
        {
            _usedTable.IsUsedByCustomer = false;
            _usedTable = null;
        }
    }
#endregion
    
    private void MoveToBar()
    {
        _nodeManager.SetNewStartAndEndNodes(_lastNodeIndex, _BarNodeId);
        _lastNodeIndex = _BarNodeId;
        _dijkstraPathFollower.FollowPath();
    }
    
    private void MoveToBarExit()
    {
        _nodeManager.SetNewStartAndEndNodes(_lastNodeIndex, _BarExitNodeId);
        _lastNodeIndex = _BarExitNodeId;
        _dijkstraPathFollower.FollowPath();
        FeedBackImage.gameObject.SetActive(true);
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

        // 3. Détermine la meilleure table selon la distance
        Table bestTable = bestTables
            .OrderBy(t => Vector3.Distance(transform.position, _nodeManager.nodes[t.TableNumber].position))
            .First();
        SetCustomerFeedback(bestTable);
        Debug.Log($"Best table chosen: {bestTable.name}");

        // Ex : déplacement vers la table
        _nextNodeIndex = bestTable.TableNumber;
        _dijkstraManager.EnableConstraint(_characterDisability);
        _nodeManager.SetNewStartAndEndNodes(_lastNodeIndex,  _nextNodeIndex);
        _dijkstraPathFollower.FollowPath();
        _usedTable = bestTable;
        _usedTable.IsUsedByCustomer = true;
        _lastNodeIndex = _nextNodeIndex;
    }

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


    private void HandlePathEnd()
    {
        Debug.Log("Le chemin est terminé !");
        if (_lastNodeIndex == _BarNodeId)
        {
            _characterState = CharacterState.AtTheBar;
            if (!_dialogueManager) return;
            _dialogueManager.InitCharacterDialogue(this, false, CodeLanguage.English);
            
        }
        else if (_lastNodeIndex == _BarExitNodeId)
        {
            _characterState = CharacterState.Idle;
            Destroy(gameObject);
        }
        else
        {
            _characterState = CharacterState.AtTheBestTable;
            StartWaiting();
        }
    }

    public void OnDialogueEnd()
    {
        switch (_characterState)
        {
            case CharacterState.AtTheBar:
            {
                MoveToBestTable();
                break;
            }
            default:
                break;
        }
    }
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

