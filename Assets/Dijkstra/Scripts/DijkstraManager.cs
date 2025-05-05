using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using MyUtilities;

public class DijkstraManager : MonoBehaviour
{
    public NodeManager nodeManager;
    private List<NodeDijkstra> allNodes => nodeManager.nodes;
    private List<(NodeDijkstra, NodeDijkstra)> pathEdges = new List<(NodeDijkstra, NodeDijkstra)>();
    public List<(NodeDijkstra, NodeDijkstra)> PathEdges => pathEdges;

    public bool TakeNodeWaitTimeIntoAccount;
        
    // Dictionnaire pour gérer les contraintes activées/désactivées
    public ConstraintBoolDictionary constraintDict = new ConstraintBoolDictionary();

    public void Awake()
    {
        ServiceLocator.Register(this);
    }

    private void ResetConstraints()
    {
        for (int i = 0; i < constraintDict.values.Count; i++)
        {
            constraintDict.values[i] = false;
        }
    }

    public void EnableConstraint(string ConstaintToEnable ="")
    {
        ResetConstraints();
        if (ConstaintToEnable == "" || !constraintDict.ContainsKey(ConstaintToEnable)) return;
        constraintDict[ConstaintToEnable] = true;
    }

    /// <summary>
    /// Trouve le plus court chemin en prenant en compte les contraintes.
    /// </summary>
    public void FindShortestPath()
    {
        nodeManager.CleanUpNodes();

        NodeDijkstra startNode = allNodes.FirstOrDefault(node => node.isStartNode);
        NodeDijkstra endNode = allNodes.FirstOrDefault(node => node.isEndNode);

        if (startNode != null && endNode != null)
            FindShortestPath(startNode, endNode);
        else
            Debug.LogWarning("Aucun nœud de départ ou de fin n'a été défini.");
    }

    /// <summary>
    /// Algorithme de Dijkstra modifié avec contraintes.
    /// </summary>
    public void FindShortestPath(NodeDijkstra startNode, NodeDijkstra endNode)
    {
        Dictionary<NodeDijkstra, float> distances = new Dictionary<NodeDijkstra, float>();
        Dictionary<NodeDijkstra, NodeDijkstra> previousNodes = new Dictionary<NodeDijkstra, NodeDijkstra>();
        List<NodeDijkstra> unvisitedNodes = new List<NodeDijkstra>(allNodes);

        // Initialisation des distances
        foreach (NodeDijkstra node in allNodes)
        {
            distances[node] = float.PositiveInfinity;
            previousNodes[node] = null;
        }

        distances[startNode] = 0;

        while (unvisitedNodes.Count > 0)
        {
            NodeDijkstra currentNode = GetClosestNode(unvisitedNodes, distances);
            
            // Si la plus petite distance est infinie, il n'y a plus de chemin possible
            if (currentNode == null || distances[currentNode] == float.PositiveInfinity)
            {
                Debug.LogWarning("Aucun chemin valide trouvé entre le nœud de départ et le nœud de fin.");
                pathEdges.Clear();  // Nettoie le chemin précédent
                return;
            }

            unvisitedNodes.Remove(currentNode);

            if (currentNode == endNode)
            {
                BuildPath(previousNodes, endNode);
                return;
            }

            // Vérifie les voisins via les connexions avec NodeID
            foreach (EdgeDijkstra edge in nodeManager.connections)
            {
                NodeDijkstra startEdgeNode = nodeManager.nodes.FirstOrDefault(n => n.NodeID == edge.StartNodeNumber);
                NodeDijkstra endEdgeNode = nodeManager.nodes.FirstOrDefault(n => n.NodeID == edge.EndNodeNumber);

                if (startEdgeNode == null || endEdgeNode == null)
                    continue;  // Ignore les connexions invalides

                NodeDijkstra neighborNode = null;

                if (startEdgeNode == currentNode)
                    neighborNode = endEdgeNode;
                else if (endEdgeNode == currentNode)
                    neighborNode = startEdgeNode;

                if (neighborNode == null)
                    continue;

                float alt = distances[currentNode] + CalculateEdgeCost(edge);

                // Prend en compte le temps d'attente si activé
                if (TakeNodeWaitTimeIntoAccount)
                    alt += neighborNode.waitTime;

                if (alt < distances[neighborNode])
                {
                    distances[neighborNode] = alt;
                    previousNodes[neighborNode] = currentNode;
                }
            }
        }

        // Aucun chemin trouvé
        Debug.LogWarning("Aucun chemin valide n'a pu être construit.");
        pathEdges.Clear();
    }

    /// <summary>
    /// Calcule le coût d'une Edge en fonction des contraintes activées.
    /// </summary>
    private float CalculateEdgeCost(EdgeDijkstra edge)
    {
        float totalCost = edge.cost; // le cout de l'edge

        foreach (Constraint constraint in edge.constraints)
        {
            // Vérifie si la contrainte doit être prise en compte dans l'algo
            if (constraintDict.ContainsKey(constraint.name) && constraintDict[constraint.name])
            {
                switch (constraint.type)
                {
                    case ConstraintType.Int:
                        totalCost += Mathf.Max(0, constraint.intValue);  // On grantit une valeur positive
                        break;

                    case ConstraintType.Float:
                        totalCost += Mathf.Max(0f, constraint.floatValue);  // On garantit une valeur positive
                        break;

                    case ConstraintType.Bool:
                        if (!constraint.boolValue)
                            return float.PositiveInfinity;  //On ignore cette Edge si false (on ajoute l'infini donc forcément ça l'élimine)
                        break;

                    case ConstraintType.Vector3:
                        totalCost += constraint.vector3Value.magnitude;  // On prend la magnitude du Vector3
                        break;
                }
            }
        }

        return totalCost;
    }

    /// <summary>
    /// Récupère le Node avec la plus petite distance.
    /// </summary>
    private NodeDijkstra GetClosestNode(List<NodeDijkstra> nodes, Dictionary<NodeDijkstra, float> distances)
    {
        NodeDijkstra closestNode = null;
        float shortestDistance = float.PositiveInfinity;

        foreach (NodeDijkstra node in nodes)
        {
            float totalDistance = distances[node] + (TakeNodeWaitTimeIntoAccount ? node.waitTime : 0);
            if (totalDistance < shortestDistance)
            {
                shortestDistance = totalDistance;
                closestNode = node;
            }
        }

        return closestNode;
    }

    /// <summary>
    /// Reconstruit le chemin optimal.
    /// </summary>
    private void BuildPath(Dictionary<NodeDijkstra, NodeDijkstra> previousNodes, NodeDijkstra endNode)
    {
        pathEdges.Clear();
        NodeDijkstra currentNode = endNode;

        while (currentNode != null && previousNodes[currentNode] != null)
        {
            NodeDijkstra previousNode = previousNodes[currentNode];
            pathEdges.Add((previousNode, currentNode));
            currentNode = previousNode;
        }

        pathEdges.Reverse();
    }
}
