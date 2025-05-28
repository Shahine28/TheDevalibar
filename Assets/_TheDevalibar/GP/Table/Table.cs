using System;
using AYellowpaper.SerializedCollections;
using MyUtilities;
using UnityEngine;
using UnityEngine.UI;

public class Table : MonoBehaviour
{
    [SerializeField] private GameObject _tableGameObject;
    public GameObject TableGameObject => _tableGameObject;

    [SerializeField, ReadOnly] private int _tableNodeNumber = -1;
    [SerializeField, ReadOnly] private int _tableID = 0;
    private TabsManager _tabsManager;
    public int TableNodeNumber => _tableNodeNumber;

    [Header("Chairs")] 
    [SerializeField] private Chair _chair1;
    [SerializeField] private Chair _chair2;
    
    
    
    [Header("Table Floor Level")]
    [SerializeField] private FloorLevel _tableFloorLevel = FloorLevel.GroundFloor;
    public FloorLevel TableFloorLevel => _tableFloorLevel;

    [SerializeField] private Button _upgradeButton;
    // Dictionnaire pour gérer les contraintes activées/désactivées
    public ConstraintBoolDictionary constraintDict = new ConstraintBoolDictionary();
    
    public bool IsUsedByCustomer =>
        (_chair1 != null && _chair1.IsChairOccupied) ||
        (_chair2 != null && _chair2.IsChairOccupied);

    
    [SerializedDictionary("Table Upgrade", "Is Purchased")]
    public SerializedDictionary<TableUpgrade, bool> PurchasedTableUpgrades = new SerializedDictionary<TableUpgrade, bool>();
    
    [Header("Table Meshes")]
    [SerializeField] private MeshFilter _tableMeshFilter;
    [SerializeField] private MeshRenderer _tableMeshRenderer;
    [SerializeField] private MeshFilter _chairMesh1Filter;
    [SerializeField] private MeshRenderer _chairMeshRenderer1;
    [SerializeField] private MeshFilter _chairMesh2Filter;
    [SerializeField] private MeshRenderer _chairMeshRenderer2;

    [SerializedDictionary("Unlockable Item", "Accessibility Upgrade Type")]
    public SerializedDictionary<AccessibilityUpgrade, GameObject> UnloclableItems = new SerializedDictionary<AccessibilityUpgrade, GameObject>();

    void Awake()
    {
        _tableNodeNumber = -1;
    }
    void Start()
    {
        HideUpgradeButton();
        _tabsManager = ServiceLocator.Get<TabsManager>();
        if (_tabsManager == null)
        {
            Debug.LogError("There is no tabs manager in the scene.");
        }
        _upgradeButton?.onClick.AddListener(GetFocusOnTable);
    }

    public void ShowUpgradeButton()
    {
        _upgradeButton?.gameObject.SetActive(true);
    }

    public void HideUpgradeButton()
    {
        _upgradeButton?.gameObject.SetActive(false);
    }

    public Chair GetFirstAvailableChair()
    {
        if (!_chair1.IsChairOccupied) return _chair1;
        if (!_chair2.IsChairOccupied) return _chair2;
        return null;
    }

    private void GetFocusOnTable()
    {
        if (_tabsManager== null)
        {
            Debug.LogError("There is no tab object in the scene.");
            return;
        }
        _tabsManager.FocusCameraOnTab(_tableID);
    }

    public void SetTableNode(int TableId)
    {
        if (_tableNodeNumber != -1) return;
        _tableID = TableId;
        TablesManager manager = ServiceLocator.Get<TablesManager>();
        manager?.InvokeOnTablesIDSetUp();
        NodeManager nodeManager = ServiceLocator.Get<NodeManager>();
        if (!nodeManager || !_tableGameObject) return;
        float distance = 100;
        int NearestNode = -1;
        foreach (NodeDijkstra node in nodeManager.nodes)
        {
            if (Vector3.Distance(_tableGameObject.transform.position, node.position) < distance)
            {
                distance = Vector3.Distance(_tableGameObject.transform.position, node.position);
                NearestNode = node.NodeID;
            }
        }
        _tableNodeNumber = NearestNode;
    }

    public void BuyUpgrade(TableUpgrade upgrade)
    {
        if (upgrade == null)
        {
            Debug.LogError("Upgrade can't be null");
        }
        PurchasedTableUpgrades[upgrade] = true;
        string constraintUpgraded = upgrade.GetUpgrade();
        if (!string.IsNullOrEmpty(constraintUpgraded))
        {
            constraintDict[constraintUpgraded] = true;
        }
        else
        {
            Debug.LogError("Upgrade name can't be null");
        }

        TryApplyMesh(_tableMeshFilter, upgrade.NewUpgradeTableMesh);
        TryApplyMaterial(_tableMeshRenderer, upgrade.NewUpgradeTableMaterial);

        TryApplyMesh(_chairMesh1Filter, upgrade.NewUpgradeChair1Mesh);
        TryApplyMaterial(_chairMeshRenderer1, upgrade.NewUpgradeChair1Material);

        TryApplyMesh(_chairMesh2Filter, upgrade.NewUpgradeChair2Mesh);
        TryApplyMaterial(_chairMeshRenderer2, upgrade.NewUpgradeChair2Material);

        if (UnloclableItems.ContainsKey(upgrade.AccessibilityUpgrade))
        {
            UnloclableItems[upgrade.AccessibilityUpgrade].gameObject.SetActive(true);
        }
    }
    
    private void TryApplyMesh(MeshFilter filter, Mesh mesh)
    {
        if (filter && mesh)
            filter.mesh = mesh;
    }

    private void TryApplyMaterial(MeshRenderer renderer, Material material)
    {
        if (renderer && material)
            renderer.material = material;
    }

}

[Serializable]
public enum FloorLevel
{
    GroundFloor,
    UpperFloor
}


