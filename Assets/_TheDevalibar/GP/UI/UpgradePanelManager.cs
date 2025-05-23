using System.Collections.Generic;
using System.Linq;
using MyUtilities;
using UnityEngine;

public class UpgradePanelManager : MonoBehaviour
{
    [SerializeField, ReadOnly] private List<UpgradePanel> _upgradePanels = new List<UpgradePanel>();

    private UpgradePanel currentUpgradePanel;
    private Coroutine _currentUpgradePanelCoroutine;
    [SerializeField] private GameObject _upgradePanelPrefab;
    private TablesManager _tablesManager;
    [SerializeField] private MoveUI _moveUI;
    
    void Awake()
    {
        ServiceLocator.Register(this);
        ClearUpgradePanels();
    }

    private void ResetCurrentUpgradePanel(UpgradePanel upgradePanel)
    {
        if (upgradePanel == null || upgradePanel != currentUpgradePanel)
            return;

        // Stoppe toute animation existante
        if (_currentUpgradePanelCoroutine != null)
        {
            StopCoroutine(_currentUpgradePanelCoroutine);
            _currentUpgradePanelCoroutine = null;
        }

        // Sécurise l'appel si le panel est encore actif
        if (upgradePanel.gameObject.activeInHierarchy)
        {
            _currentUpgradePanelCoroutine = StartCoroutine(upgradePanel.MoveUpgradePanelCoroutine());
        }

        currentUpgradePanel = null;
    }

    public void UpdateCurrentUpgradePanel(UpgradePanel newUpgradePanel)
    {
        // Ne rien faire si null ou déjà en transition vers celui-là
        if (newUpgradePanel == null) return;

        // Si un autre est déjà affiché, le fermer proprement
        if (currentUpgradePanel != null)
        {
            ResetCurrentUpgradePanel(currentUpgradePanel);
            if (currentUpgradePanel == newUpgradePanel) return;
        }

        currentUpgradePanel = newUpgradePanel;

        if (currentUpgradePanel.gameObject.activeInHierarchy)
        {
            _currentUpgradePanelCoroutine = StartCoroutine(currentUpgradePanel.MoveUpgradePanelCoroutine());
        }
    }

    public void ClearUpgradePanels(bool HidePanels = false)
    {
        if (HidePanels)
        {
            _moveUI.LaunchMoveUI(true);
        }
        _upgradePanels.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
    public void SetUpElevatorUpgradePanel(ElevatorManager elevatorManager)
    {
        if (!_moveUI.IsUIAtTargetPoint())
        {
            _moveUI.LaunchMoveUI();
        }
        else if (!_moveUI.IsUIAtTargetPoint())
        {
            _moveUI.LaunchMoveUI();
        }
        if (elevatorManager == null || _upgradePanelPrefab == null)
        {
            Debug.LogError("Elevator Manager is null or no upgrade panel prefab found!");
            return;
        }
        
        ClearUpgradePanels(); // pour éviter les doublons

        if (!elevatorManager.IsElevatorBuyed)
        {
            GameObject instance = Instantiate(_upgradePanelPrefab, transform);
            if (!instance)
            {
                Debug.LogWarning($"UpgradePanel prefab instantiation failed at index 0");
                return;
            }
        
            UpgradePanel upgradePanel = instance.transform.GetChild(0).GetComponent<UpgradePanel>();
            if (!upgradePanel)
            {
                Debug.LogWarning($"No UpgradePanel component found on instance at index {0}");
                Destroy(instance); // nettoyage
                return;
            }

            upgradePanel.SetUpPanel(elevatorManager.ElevatorUpgrade, null, elevatorManager);
            _upgradePanels.Add(upgradePanel);
        }
    }
    
    public void SetUpTableUpgradePanels(Table table)
    {
        if (!_moveUI.IsUIAtTargetPoint())
        {
            _moveUI.LaunchMoveUI();
        }
        else if (!_moveUI.IsUIAtTargetPoint())
        {
            _moveUI.LaunchMoveUI();
        }
        if (table == null || _upgradePanelPrefab == null)
        {
            Debug.LogError("Table is null or no upgrade panel prefab found!");
            return;
        }
        
        ClearUpgradePanels(); // pour éviter les doublons

        for (int i = 0; i < table.PurchasedTableUpgrades.Count; i++)
        {
            if (table.PurchasedTableUpgrades.Keys.ToList()[i] == null) continue;
            if (table.PurchasedTableUpgrades[table.PurchasedTableUpgrades.Keys.ToList()[i]] == true) continue;
            GameObject instance = Instantiate(_upgradePanelPrefab, transform);
            if (!instance)
            {
                Debug.LogWarning($"UpgradePanel prefab instantiation failed at index {i}");
                continue;
            }
            
            UpgradePanel upgradePanel = instance.transform.GetChild(0).GetComponent<UpgradePanel>();
            if (!upgradePanel)
            {
                Debug.LogWarning($"No UpgradePanel component found on instance at index {i}");
                Destroy(instance); // nettoyage
                continue;
            }

            upgradePanel.SetUpPanel(table.PurchasedTableUpgrades.Keys.ToList()[i], table);
            _upgradePanels.Add(upgradePanel);
        }
    }


    // public void StartSetUp()
    // {
    //     SetUpTableUpgradePanels(_tablesManager.Tables[0]);
    // }
    void Start()
    {
        // _tablesManager = ServiceLocator.Get<TablesManager>();
        // if (!_tablesManager)
        // {
        //     Debug.LogWarning("There is no TablesManager in the scene.");
        // }
        // else
        // {
        //     _tablesManager.OnTablesIDSetUp += StartSetUp;
        // }
        if (_moveUI == null)
        {
            Debug.LogError("_moveUI is null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
