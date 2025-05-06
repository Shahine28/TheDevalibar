using System;
using System.Collections;
using System.Collections.Generic;
using MyUtilities;
using UnityEngine;

public class DijkstraPathFollower : MonoBehaviour
{
    public DijkstraManager dijkstraManager;
    public float MoveSpeed = 7.5f;
    public bool FollowPathAtStart = true;

    public event Action OnFollowPathEnd;
    private void Start()
    {
        if (!dijkstraManager)
        {
            dijkstraManager = ServiceLocator.Get<DijkstraManager>();
        }
    }

    public void FollowPath()
    {
        if (!dijkstraManager)
        {
            Debug.LogError("DijkstraManager is null");
            return;
        }
        
        dijkstraManager.FindShortestPath();
        List<(NodeDijkstra, NodeDijkstra)> path = dijkstraManager.PathEdges;

        if (path.Count == 0)
        {
            Debug.LogError("No path found");
            return;
        }
        StartCoroutine(MoveAlongPath(path));
    }
    
    IEnumerator MoveAlongPath(List<(NodeDijkstra, NodeDijkstra)> path)
    {
        List<(NodeDijkstra, NodeDijkstra)> safePath = new List<(NodeDijkstra, NodeDijkstra)>(path);
        if (!safePath[0].Item1.isStartNode) // Si le chemin n'est pas dans le bon ordre on inverse la list
        {
            safePath.Reverse();
            for (int i = 0; i < safePath.Count; i++) // J'inverse aussi l'ordre des targets, current nodes.
            {
                NodeDijkstra Item1 = safePath[i].Item1;
                NodeDijkstra Item2 = safePath[i].Item2;
                safePath[i] = (Item2, Item1);
            }
        }
        
        if (transform.position != safePath[0].Item1.position && FollowPathAtStart) // si le game object n'est pas déjà sur le point de départ, on l'y met
        {
            NodeDijkstra startNode = safePath[0].Item1;
            Vector3 startPos = new Vector3(startNode.position.x, transform.position.y, startNode.position.z);
            while (Vector3.Distance(transform.position, startPos) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, startPos, MoveSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(startNode.waitTime);
        }
        
        foreach (var edge in safePath)
        {
            NodeDijkstra startNode = edge.Item1;
            NodeDijkstra targetNode = edge.Item2;
            float deltaY = targetNode.position.y - startNode.position.y;
            Vector3 nextPos = new Vector3(targetNode.position.x, transform.position.y+deltaY, targetNode.position.z);
            while (Vector3.Distance(transform.position, nextPos) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, nextPos, MoveSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();  
            }
            
            yield return new WaitForSeconds(targetNode.waitTime);
        }
        OnFollowPathEnd?.Invoke();
    }
}

