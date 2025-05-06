using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class NodeDijkstra
{
    [Header("Node Properties")] 
    [ReadOnly] public int NodeID = -1;
    public Vector3 position;
    public float waitTime = 0f; 
    [ReadOnly] public bool isStartNode = false;
    [ReadOnly] public bool isEndNode = false;
}
