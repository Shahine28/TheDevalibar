using System.Collections;
using MyUtilities;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public class Chair : MonoBehaviour
{
    [SerializeField] private GameObject _chair;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Vector3 _availablePosition;
    [SerializeField] private Vector3 _takenPosition;
    [SerializeField] private float _movementSpeed;
    private Coroutine _coroutine;
    [SerializeField] private AnimationCurve _movementAnimation = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public bool IsChairOccupied;

    [SerializeField, ReadOnly] private int _chairClosestNodeID =-1;
    public int ChairClosestNodeID => _chairClosestNodeID;

    void Start()
    {
        _chairClosestNodeID = -1;
        SetChairClosestNodeID();
    }
    
    
    public void SetChairClosestNodeID()
    {
        if (_chairClosestNodeID != -1) return;
        NodeManager nodeManager = ServiceLocator.Get<NodeManager>();
        if (!nodeManager || !_chair) return;
        float distance = 100;
        int NearestNode = -1;
        foreach (NodeDijkstra node in nodeManager.nodes)
        {
            if (Vector3.Distance(_chair.transform.position, node.position) < distance)
            {
                distance = Vector3.Distance(_chair.transform.position, node.position);
                NearestNode = node.NodeID;
            }
        }
        _chairClosestNodeID = NearestNode;
        MoveChairToUnoccupiedPosition();
    }
    
    [Button]
    public void MoveChairToOccupiedPosition()
    {
        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(MoveChair(_takenPosition));
        IsChairOccupied = true;
    }

    [Button]
    public void MoveChairToUnoccupiedPosition()
    {
        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(MoveChair(_availablePosition));
        IsChairOccupied = false;
    }

    public void HideChair()
    {
        _meshRenderer.enabled = false;
    }

    public void ShowChair()
    {
        _meshRenderer.enabled = true;
    }
    
    private IEnumerator MoveChair(Vector3 targetPosition)
    {
        Transform chair = _chair.transform;
        Vector3 start = chair.localPosition;
        Vector3 end = targetPosition;

        float distance = Vector3.Distance(start, end);
        float duration = distance / _movementSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float curvedT = _movementAnimation.Evaluate(t);

            chair.localPosition = Vector3.Lerp(start, end, curvedT);
            yield return new WaitForEndOfFrame();
        }

        chair.localPosition = end;
        _coroutine = null;
    }
}
