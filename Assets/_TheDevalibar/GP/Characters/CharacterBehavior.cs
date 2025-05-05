using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _TheDevalibar.GP.Characters;
using AYellowpaper.SerializedCollections;
using MyUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = Unity.Mathematics.Random;

public class CharacterBehavior : MonoBehaviour
{


    [SerializeField] private Character character;
    public Character Character => character;
    
    [Header("Dijkstra")]
    [SerializeField] private NodeManager nodeManager;
    [SerializeField] private DijkstraManager dijkstraManager;
    [SerializeField] private DijkstraPathFollower dijkstraPathFollower;

    [Header("WaitingTime")] 
    [SerializeField] private Vector2 _waitingTimeRange;
    private Coroutine _waitRoutine;
    private bool _interrupted = false;

    [Header("Movement")] [SerializeField] private int _BarNodeId = 40;
    [SerializeField, ReadOnly] private  int _startNodeIndex;
    [SerializeField, ReadOnly] private  int _lastNodeIndex;
    [SerializeField, ReadOnly] private  int _nextNodeIndex;
    private TablesManager _tablesManager;
    
    
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
        if (dijkstraPathFollower)
        {
            dijkstraPathFollower.OnFollowPathEnd += HandlePathEnd;
        }
    }

    public void OnDisable()
    {
        if (dijkstraPathFollower)
        {
            dijkstraPathFollower.OnFollowPathEnd -= HandlePathEnd;
        }
    }
#endregion

    void Start()
    {
        if (!dijkstraManager)
        {
            dijkstraManager = ServiceLocator.Get<DijkstraManager>();
            if (dijkstraPathFollower)
            {
                dijkstraPathFollower.OnFollowPathEnd += HandlePathEnd;
            }
        }
        if (!nodeManager)
        {
            nodeManager = ServiceLocator.Get<NodeManager>();
        }

        if (!dijkstraPathFollower)
        {
            dijkstraPathFollower = GetComponent<DijkstraPathFollower>();
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
        if (character)
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
                yield break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Finished waiting at the bar.");
        MoveToBarExit();
    }

#endregion
    
    private void MoveToBar()
    {
        nodeManager.SetNewStartAndEndNodes(_lastNodeIndex, _BarNodeId);
        _lastNodeIndex = _BarNodeId;
        dijkstraPathFollower.FollowPath();
    }
    private void MoveToBarExit()
    {
        nodeManager.SetNewStartAndEndNodes(_lastNodeIndex, _startNodeIndex);
        _lastNodeIndex = _startNodeIndex;
        dijkstraPathFollower.FollowPath();
        FeedBackImage.gameObject.SetActive(true);
    }
    
    
    private void MoveToBestTable()
    {
        // 1. Récupère le handicap actif (ou "" s’il n’y en a pas)
        string characterDisability = character.constraintDict.keys
            .Select((key, i) => new { key, isActive = character.constraintDict.values[i] })
            .FirstOrDefault(x => x.isActive)?.key ?? "";

        //Récupère les tables accessibles (déjà triées par priorité)
        List<Table> bestTables = _tablesManager.GetAccessibleTables(characterDisability);

        if (bestTables.Count == 0)
        {
            Debug.Log("No tables found, customers need to leave");
            SetCustomerFeedback(null);
            MoveToBarExit();
            return;
        }

        // 3. Détermine la meilleure table selon la distance
        Table bestTable = bestTables
            .OrderBy(t => Vector3.Distance(transform.position, nodeManager.nodes[t.TableNumber].position))
            .First();
        SetCustomerFeedback(bestTable);
        Debug.Log($"Best table chosen: {bestTable.name}");

        // Ex : déplacement vers la table
        _nextNodeIndex = bestTable.TableNumber;
        dijkstraManager.EnableConstraint(characterDisability);
        nodeManager.SetNewStartAndEndNodes(_lastNodeIndex,  _nextNodeIndex);
        dijkstraPathFollower.FollowPath();
        bestTable.IsUsedByCustomer = true;
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

        string characterDisability = character.constraintDict.keys
            .Select((key, i) => new { key, isActive = character.constraintDict.values[i] })
            .FirstOrDefault(x => x.isActive)?.key ?? "";

        // Aucun handicap → satisfait
        if (string.IsNullOrEmpty(characterDisability))
        {
            _customerFeedback = CustomersFeedback.Good;
            return;
        }

        // Handicap présent → vérifie si table adaptée
        bool isTableAdapted = table.constraintDict.ContainsKey(characterDisability) && table.constraintDict[characterDisability];

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
        else if (_lastNodeIndex == _startNodeIndex)
        {
            _characterState = CharacterState.Idle;
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

