using System;
using MyUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Table : MonoBehaviour
{
    [SerializeField] private GameObject TableGameObject;

    [SerializeField, ReadOnly] private int _tableNumber = -1;
    
    public int TableNumber => _tableNumber;

    [SerializeField] private Button _upgradeButton;
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
        HideUpgradeButton();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowUpgradeButton()
    {
        _upgradeButton?.gameObject.SetActive(true);
    }

    public void HideUpgradeButton()
    {
        _upgradeButton?.gameObject.SetActive(false);
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

