using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using MyUtilities;

using Event = UnityEngine.Event;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(NodeManager))]
public class NodeEditor : Editor
{
    private NodeDijkstra firstSelectedNode = null;
    private NodeDijkstra _selectedNode = null;
    private EdgeDijkstra _selectedEdge = null;
    void OnSceneGUI()
    {
        Event e = Event.current;
        NodeManager manager = (NodeManager)target;
        manager.CleanUpNodes();

        if (e.type == EventType.MouseDown && e.button == 0 && !e.control)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            
            bool clickedOnNode = false;

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                foreach (NodeDijkstra n in manager.nodes)
                {
                    if (Vector3.Distance(hit.point, n.position) < 0.5f && _selectedNode != n)
                    {
                        e.Use();  // Consomme l'événement pour empêcher d'autres sélections
                        _selectedNode = n;  // Focus sur le Node cliqué
                        clickedOnNode = true;
                        break;
                    }
                }
                if (!clickedOnNode && _selectedNode != null && Vector3.Distance(hit.point, _selectedNode.position) > 1 
                    && HandleUtility.nearestControl > 2)
                {
                    e.Use();  // Empêche la sélection d'autres objets dans la scène
                    _selectedNode = null;  // Retire le focus du Node sélectionné
                }
            }
        }
        else if (e.type == EventType.MouseDown && e.button == 0 && e.control)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Vérifie si on clique sur un nœud existant
                foreach (NodeDijkstra n in manager.nodes)
                {
                    if (Vector3.Distance(hit.point, n.position) < 0.5f)
                    {
                        if (firstSelectedNode == null)
                        {
                            // Premier nœud sélectionné
                            firstSelectedNode = n;
                        }
                        else if (firstSelectedNode != n)
                        {
                            // Crée une connexion si un deuxième nœud est sélectionné
                            Undo.RecordObject(manager, "Add Edge");  // Enregistre l'action pour Undo

                            EdgeDijkstra newEdge = new EdgeDijkstra
                            {
                                StartNodeNumber = firstSelectedNode.NodeID,
                                EndNodeNumber = n.NodeID,
                                cost = 1f
                            };
                    
                            manager.connections.Add(newEdge);
                            EditorUtility.SetDirty(manager);
                            manager.OnInspectorChange();
                            manager.RemoveDuplicateEdges();
                            firstSelectedNode = null;
                        }

                        e.Use();
                        return;
                    }
                }
                
                Undo.RecordObject(manager, "Add Node");
                NodeDijkstra newNode = new NodeDijkstra
                {
                    position = hit.point
                };

                manager.nodes.Add(newNode);
                EditorUtility.SetDirty(manager);
            }
        }

        else if (e.type == EventType.MouseDown && e.button == 1 && e.control)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                foreach (NodeDijkstra n in manager.nodes)
                {
                    if (Vector3.Distance(hit.point, n.position) < 0.5f)
                    {
                        NodeDijkstra startNode = manager.HaveAStartNode();
                        NodeDijkstra endNode = manager.HaveAEndNode();
                        bool stateChanged = false;

                        // ✅ Modification de l'état Start Node avec Undo
                        if (startNode != null)
                        {
                            if (startNode == n)
                            {
                                Undo.RecordObject(manager, "Unset Start Node");  // 🔄 Enregistre l'action pour Undo
                                n.isStartNode = false;
                                stateChanged = true;
                            }
                        }
                        else
                        {
                            if (endNode != n)
                            {
                                Undo.RecordObject(manager, "Set Start Node");  // 🔄 Enregistre l'action pour Undo
                                n.isStartNode = true;
                                stateChanged = true;
                            }
                        }

                        // ✅ Modification de l'état End Node avec Undo
                        if (!stateChanged)
                        {
                            if (endNode != null)
                            {
                                if (endNode == n)
                                {
                                    Undo.RecordObject(manager, "Unset End Node");  // 🔄 Enregistre l'action pour Undo
                                    n.isEndNode = false;
                                    stateChanged = true;
                                }
                            }
                            else
                            {
                                if (startNode != n)
                                {
                                    Undo.RecordObject(manager, "Set End Node");  // 🔄 Enregistre l'action pour Undo
                                    n.isEndNode = true;
                                    stateChanged = true;
                                }
                            }
                        }

                        // ✅ Met à jour l'inspecteur et la scène
                        if (stateChanged)
                        {
                            EditorUtility.SetDirty(manager);
                            SceneView.RepaintAll();
                        }
                        e.Use();
                        break;
                        
                        
                    }
                }
            }
        }

        if (_selectedNode != null && e.type == EventType.KeyDown && e.keyCode == KeyCode.Delete)
        {
            Undo.RecordObject(manager, "Delete Node");
            manager.nodes.Remove(_selectedNode);
            _selectedNode = null;  // ✅ Réinitialise la sélection
            EditorUtility.SetDirty(manager);
            e.Use();
        }
        
        // Gizmos et Handles pour chaque nœud
        foreach (NodeDijkstra n in manager.nodes)
        {
            if (_selectedNode == n)
            {
                EditorGUI.BeginChangeCheck();
                Vector3 newPos = Handles.PositionHandle(n.position, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(manager, "Move Node Position");
                    n.position = newPos;
                    EditorUtility.SetDirty(manager);
                }
            }

            // Gizmo : sphère bleue pour représenter le nœud
            float zoomScale = HandleUtility.GetHandleSize(n.position);
            float baseSize = 0.7f;

            // Limite la taille min et max pour ne pas disparaître ou devenir géant
            float clampedSize = Mathf.Clamp(baseSize / zoomScale, 0.8f, 0.2f);

            Handles.color = n.isStartNode ? Color.green : n.isEndNode ? Color.red : Color.blue;
            Handles.SphereHandleCap(0, n.position, Quaternion.identity, clampedSize, EventType.Repaint);
            
            // Initialisation de l'identifiant unique du Node si pas déjà fait
            if (n.NodeID == -1)
            {
                int NodeID = manager.nodes.IndexOf(n);
                bool IsUniqueID = false;
                while (!IsUniqueID)
                {
                    IsUniqueID = true;
                    foreach (NodeDijkstra node in manager.nodes)
                    {
                        if (node.NodeID == NodeID)
                        {
                            IsUniqueID = false;
                            NodeID++;
                        }
                        
                    }
                }
                n.NodeID = NodeID;
            }
            
            // Affichage de l'index du nœud
            GUIStyle indexStyle = new GUIStyle();
            indexStyle.normal.textColor = Color.cyan;
            indexStyle.fontSize = 28;
            indexStyle.alignment = TextAnchor.MiddleCenter;
            Handles.Label(n.position + Vector3.up * 0.75f, n.NodeID.ToString(), indexStyle);
            
        }
        
        
        for (int i = 0; i < manager.connections.Count; i++)
        {
            EdgeDijkstra edge = manager.connections[i];
            int startNodeNumber = edge.StartNodeNumber;
            int endNodeNumber = edge.EndNodeNumber;
            if (edge != null && startNodeNumber != -1 && endNodeNumber != -1 
                && endNodeNumber != startNodeNumber 
                && GetNode(edge.StartNodeNumber) != null && GetNode(edge.EndNodeNumber) != null)
            {
                NodeDijkstra StartNode = GetNode(edge.StartNodeNumber);
                NodeDijkstra EndNode = GetNode(edge.EndNodeNumber);
                
                if (StartNode == null ||  EndNode == null) continue;
                Vector3 startPos = StartNode.position;
                Vector3 endPos = EndNode.position;

                // Calcul du point médian pour placer le bouton
                Vector3 midPoint = (startPos + endPos) / 2;

                // Bouton invisible mais interactif
                Handles.color = Color.white;
                float buttonSize = 0.2f;

                if (Handles.Button(midPoint, Quaternion.identity, buttonSize, buttonSize, Handles.SphereHandleCap))
                {
                    int edgeIndex = manager.connections.IndexOf(edge);

                    if (edgeIndex != -1)
                    {
                        // ✅ Focus sur l'objet dans l'Inspector
                        Selection.activeObject = manager;
                        // EditorGUIUtility.PingObject(manager);
                        EditorUtility.SetDirty(manager);
                        _selectedEdge = edge;
                        Debug.Log($"Edge {edgeIndex} selected between nodes {edge.StartNodeNumber} and {edge.EndNodeNumber}");
                        string propertyPath = $"connections.Array.data[{edgeIndex}]"; // Je déplie au clique la egde en question
                        if (EdgeDrawer.edgeFoldouts.ContainsKey(propertyPath))
                        {
                            EdgeDrawer.edgeFoldouts[propertyPath] = true;
                            
                        }
                        ForceInspectorUpdate();
                        
                    }
                }
                
                if (manager.dijkstraManager && ( manager.dijkstraManager.PathEdges.Contains((StartNode, EndNode)) ||
                    manager.dijkstraManager.PathEdges.Contains((EndNode, StartNode))))
                {
                    Handles.color = Color.red;
                }
                else
                {
                    Handles.color = Color.black;
                }
                Handles.DrawLine(startPos, endPos);

                // Affiche le coût de l'arête
                GUIStyle costStyle = new GUIStyle();
                costStyle.normal.textColor = Color.black;
                costStyle.fontSize = 28;
                costStyle.alignment = TextAnchor.MiddleCenter;
                Handles.Label(midPoint, edge.cost.ToString("F0"), costStyle);
            }
            else
            {
                Undo.RecordObject(manager, "Deleting Edge");
                manager.connections.Remove(edge);
            }
        }
        
    }
    
    private void ForceInspectorUpdate()
    {
        // Marque l'objet comme modifié pour que Unity l'enregistre
        EditorUtility.SetDirty(Selection.activeObject);

        // Force Unity à rafraîchir l'éditeur
        EditorApplication.QueuePlayerLoopUpdate();

        // Force la mise à jour de la fenêtre de l'Inspector
        ActiveEditorTracker.sharedTracker.ForceRebuild();

        // Actualise la vue de la scène
        SceneView.RepaintAll();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUI.changed)
        {
            NodeManager manager = (NodeManager)target;
            manager.OnInspectorChange();
            ConstraintSyncEvent.Raise(manager.constraints);
        }
            
    }

    public NodeDijkstra GetNode(int nodeID)
    {
        NodeManager manager = (NodeManager)target;
        foreach (NodeDijkstra n in manager.nodes)
        {
            if (n.NodeID == nodeID) return n;
        }
        return null;
    }
}
#endif


public class NodeManager : MonoBehaviour
{
    public List<Constraint> constraints = new List<Constraint>();
    public List<NodeDijkstra> nodes = new List<NodeDijkstra>();
    public List<EdgeDijkstra> connections = new List<EdgeDijkstra>();
    public DijkstraManager dijkstraManager;

    private List<Constraint> previousConstraintsSnapshot = new List<Constraint>();
    private List<EdgeDijkstra> previousEdgesSnapshot = new List<EdgeDijkstra>();

    public void CleanUpNodes()
    {
        nodes.RemoveAll(node => node == null);
    }

    private void ResetStartAndEndNodes()
    {
        nodes.Where(x => x.isStartNode || x.isEndNode).ToList().ForEach(x =>
        {
            x.isStartNode = false;
            x.isEndNode = false;
        });
    }

    public void SetNewStartAndEndNodes(int startNodeID, int endNodeID)
    {
        ResetStartAndEndNodes();
        int startNodeIndex = nodes.IndexOf(nodes.First(x => x.NodeID == startNodeID));
        int endNodeIndex = nodes.IndexOf(nodes.Last(x => x.NodeID == endNodeID));
        nodes[startNodeIndex].isStartNode = true;
        nodes[endNodeIndex].isEndNode = true;
    }

    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    private void Start()
    {
        if (dijkstraManager == null) Debug.LogError("DijkstraManager is null");
    }

    private void OnValidate()
    {
        
        // ✅ Vérifie les modifications dans les contraintes (nom, type, taille)
        if (ConstraintsHaveChanged(previousConstraintsSnapshot, constraints))
        {
            SyncConstraintsWithEdges();
            previousConstraintsSnapshot = CloneConstraints(constraints);  // ✅ Clonage au lieu d'une simple assignation
        }
        
    }

    public void OnInspectorChange()
    {
        if (previousEdgesSnapshot.Count != connections.Count)
        {
            SyncConstraintsWithEdges();
            RemoveDuplicateEdges();
            previousEdgesSnapshot = CloneEdges(connections);
            
        }
    }


    /// <summary>
    ///  Synchronise les contraintes avec les Edges.
    /// </summary>
    public void SyncConstraintsWithEdges()
    {
        foreach (EdgeDijkstra edge in connections)
        {
            // Synchronisation des contraintes selon l'index
            for (int i = 0; i < constraints.Count; i++)
            {
                Constraint globalConstraint = constraints[i];

                // Si l'Edge n'a pas assez de contraintes, on les ajoute
                if (i >= edge.constraints.Count)
                {
                    edge.constraints.Add(new Constraint
                    {
                        name = globalConstraint.name,
                        type = globalConstraint.type
                    });
                }
                else
                {
                    // Si la contrainte existe, on vérifie et met à jour si nécessaire
                    if (edge.constraints[i].name != globalConstraint.name)
                    {
                        edge.constraints[i].name = globalConstraint.name;
                    }

                    if (edge.constraints[i].type != globalConstraint.type)
                    {
                        edge.constraints[i].type = globalConstraint.type;
                    }
                }
            }

            // Suppression des contraintes en trop
            if (edge.constraints.Count > constraints.Count)
            {
                edge.constraints.RemoveRange(constraints.Count, edge.constraints.Count - constraints.Count);
            }
        }
    }

    public void RemoveDuplicateEdges()
    {
        HashSet<(int, int)> uniqueEdges = new HashSet<(int, int)>();
        List<EdgeDijkstra> edgesToRemove = new List<EdgeDijkstra>();

        foreach (EdgeDijkstra edge in connections)
        {
            // Crée une paire unique pour identifier les connexions (ordre non sensible)
            (int, int) edgeKey = edge.StartNodeNumber < edge.EndNodeNumber
                ? (edge.StartNodeNumber, edge.EndNodeNumber)
                : (edge.EndNodeNumber, edge.StartNodeNumber);

            // Si l'Edge existe déjà, elle est marquée pour suppression
            if (!uniqueEdges.Add(edgeKey))
            {
                edgesToRemove.Add(edge);
            }
        }

        // Supprime les Edges marquées pour suppression
        foreach (EdgeDijkstra edge in edgesToRemove)
        {
            connections.Remove(edge);
        }

        if (edgesToRemove.Count!=0) Debug.Log($"{edgesToRemove.Count} duplicate edges removed.");
    }
    private bool ConstraintsHaveChanged(List<Constraint> oldList, List<Constraint> newList)
    {
        if (oldList.Count != newList.Count)
            return true;

        for (int i = 0; i < oldList.Count; i++)
        {
            if (oldList[i].name != newList[i].name || oldList[i].type != newList[i].type)
                return true;
        }

        return false;
    }

    /// <summary>
    /// ✅ Clone la liste de contraintes pour les comparaisons.
    /// </summary>
    private List<Constraint> CloneConstraints(List<Constraint> original)
    {
        List<Constraint> clone = new List<Constraint>();
        foreach (var constraint in original)
        {
            clone.Add(new Constraint
            {
                name = constraint.name,
                type = constraint.type
            });
        }
        return clone;
    }

    private List<EdgeDijkstra> CloneEdges(List<EdgeDijkstra> original)
    {
        List<EdgeDijkstra> clone = new List<EdgeDijkstra>();
        foreach (EdgeDijkstra edge in original)
        {
            clone.Add(new EdgeDijkstra
            {
                StartNodeNumber = edge.StartNodeNumber,
                EndNodeNumber = edge.EndNodeNumber,
            });
        }
        return clone;
    }

    public NodeDijkstra HaveAStartNode()
    {
        foreach (NodeDijkstra n in nodes)
        {
            if (n.isStartNode) return n;
        }
        return null;
    }

    public NodeDijkstra HaveAEndNode()
    {
        foreach (NodeDijkstra n in nodes)
        {
            if (n.isEndNode) return n;
        }
        return null;
    }
}


