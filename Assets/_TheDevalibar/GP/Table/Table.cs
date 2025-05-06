using System;
using MyUtilities;
using Unity.VisualScripting;
using UnityEngine;

public class Table : MonoBehaviour
{
    [SerializeField] private GameObject TableGameObject;

    [SerializeField, ReadOnly] private int _tableNumber = -1;
    
    public int TableNumber => _tableNumber;
    
    // Dictionnaire pour gérer les contraintes activées/désactivées
    public ConstraintBoolDictionary constraintDict = new ConstraintBoolDictionary();
    
    public bool IsUsedByCustomer  = false;

    void Awake()
    {
        _tableNumber = -1;
    }
    void Start()
    {
        SetTableNode();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetTableNode()
    {
        NodeManager nodeManager = ServiceLocator.Get<NodeManager>();
        if (!nodeManager || !TableGameObject) return;
        float distance = 100;
        int NearestNode = -1;
        foreach (NodeDijkstra node in nodeManager.nodes)
        {
            if (Vector3.Distance(TableGameObject.transform.position, node.position) < distance)
            {
                distance = Vector3.Distance(TableGameObject.transform.position, node.position);
                NearestNode = nodeManager.nodes.IndexOf(node);
            }
        }
        _tableNumber = NearestNode;
    }
}

