using System.Collections.Generic;
using System.Linq;
using MyUtilities;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradePanelManager : MonoBehaviour
{
    [SerializeField, ReadOnly] private List<UpgradePanel> _upgradePanels = new List<UpgradePanel>();

    private UpgradePanel currentUpgradePanel;
    public UpgradePanel CurrentUpgradePanel;
    private Coroutine _currentUpgradePanelCoroutine;
    [SerializeField] private GameObject _upgradePanelPrefab;
    private TablesManager _tablesManager;
    [SerializeField] private MoveUI _moveUI;
    
    void Awake()
    {
        ServiceLocator.Register(this);
        ClearUpgradePanels();
    }

    // public void StartSetUp()
    // {
    //     SetUpTableUpgradePanels(_tablesManager.Tables[0]);
    // }
    void Start()
    {
        if (_moveUI == null)
        {
            Debug.LogError("_moveUI is null");
        }
        
        _upgradePanels.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            _upgradePanels.Add(transform.GetChild(i).GetComponentInChildren<UpgradePanel>());
        }
    }
    
#region UpgradePanelsMovement
    public void ResetCurrentUpgradePanel(UpgradePanel upgradePanel)
    {
        if (upgradePanel == null || upgradePanel != currentUpgradePanel) return;

        ResetUpgradePanel(upgradePanel);

        currentUpgradePanel = null;
    }
    
    public void ResetUpgradePanel(UpgradePanel upgradePanel)
    {
        if (upgradePanel == null)
            return;
        
        upgradePanel.StopTransitionCoroutine();
        
        if (upgradePanel.gameObject.activeInHierarchy)
        {
           upgradePanel.MovePanelToStartPosition();
        }
    }

    public void ResetUpgradePanelsPosition(bool ignoreCurrentUpgradePanel = false)
    {
        foreach (UpgradePanel upgradePanel in _upgradePanels)
        {
            if (upgradePanel == null || (ignoreCurrentUpgradePanel && upgradePanel == currentUpgradePanel)) return;
            if (upgradePanel.IsPanelTransitioning || upgradePanel.IsUpgradeFullyDisplayed)
            {
                if (currentUpgradePanel == upgradePanel) ResetCurrentUpgradePanel(upgradePanel);
                else ResetUpgradePanel(upgradePanel);
            }
        }
    }

    public void UpdateCurrentUpgradePanel(UpgradePanel newUpgradePanel)
    {

        if (newUpgradePanel == null) return;
        currentUpgradePanel = newUpgradePanel;
        ResetUpgradePanelsPosition(true);
        if (currentUpgradePanel.gameObject.activeInHierarchy)
        {
            currentUpgradePanel.MovePanelToEndPosition();
        }
    }
#endregion

    public void ClearUpgradePanels(bool HidePanels = false)
    {
        if (HidePanels)
        {
            _moveUI.LaunchMoveUI(true);
        }
        _upgradePanels?.ForEach(panel => panel?.transform.parent.gameObject.SetActive(false));
        ResetUpgradePanelsPosition();
    }

    public void SelectFirstAvailableUpgradePanel()
    {
        if (_upgradePanels.Count == 0) return;
        UpgradePanel firstAvailableUpgradePanel = _upgradePanels.FirstOrDefault(x => x.transform.parent.gameObject.activeInHierarchy);
        if (firstAvailableUpgradePanel != null)
        {
            EventSystem.current.SetSelectedGameObject(firstAvailableUpgradePanel.transform.parent.gameObject);
        }
        else
        {
            TabsManager tabsManager = ServiceLocator.Get<TabsManager>();
            if (tabsManager != null)
            {
                tabsManager.SelectFirstTabs();
            }
        }
    }
    
    public void SetUpTableUpgradePanels(Table table)
    {
        if (!_moveUI.IsUIAtTargetPoint())
        {
            _moveUI.LaunchMoveUI();
        }
        if (table == null || _upgradePanelPrefab == null)
        {
            Debug.LogError("Table is null or no upgrade panel prefab found!");
            return;
        }
        
        ClearUpgradePanels();
        
        for (int i = 0; i < table.PurchasedTableUpgrades.Count; i++)
        {
            if (table.PurchasedTableUpgrades.Keys.ToList()[i] == null) continue;
            if (table.PurchasedTableUpgrades[table.PurchasedTableUpgrades.Keys.ToList()[i]]) continue;

            UpgradePanel upgradePanel = _upgradePanels[i];
            if (!upgradePanel)
            {
                Debug.LogWarning($"No UpgradePanel component found on instance at index {i}");
                continue;
            }

            upgradePanel.SetUpPanel(table.PurchasedTableUpgrades.Keys.ToList()[i], table);
            upgradePanel.transform.parent.gameObject.SetActive(true); // On le fait apparaitre
            
        }
    }
    
    
    public void SetUpElevatorUpgradePanel(ElevatorManager elevatorManager)
    {
        if (!_moveUI.IsUIAtTargetPoint())
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
            UpgradePanel upgradePanel = _upgradePanels[0];
            if (!upgradePanel)
            {
                Debug.LogWarning($"No UpgradePanel component found on instance at index {0}");
                return;
            }

            upgradePanel.SetUpPanel(elevatorManager.ElevatorUpgrade, null, ObjectType.Elevator);
            upgradePanel.transform.parent.gameObject.SetActive(true);
            
            // _upgradePanels.Add(upgradePanel);
        }
    }
    
    public void SetUpPropsUpgradePanel(UpgradablePropsManager upgradablePropsManager, ObjectType objectType)
    {
        if (!_moveUI.IsUIAtTargetPoint())
        {
            _moveUI.LaunchMoveUI();
        }
        if (upgradablePropsManager == null || _upgradePanelPrefab == null)
        {
            Debug.LogError("Upgradable Props Manager is null or no upgrade panel prefab found!");
            return;
        }
        
        ClearUpgradePanels(); // pour éviter les doublons
        
        for  (int i = 0; i <upgradablePropsManager.PurchasedUpgrades.Count; i++)
        {
            var purchasedUpgrade = upgradablePropsManager.PurchasedUpgrades.ElementAt(i);
            if (!purchasedUpgrade.Value)
            {
                UpgradePanel upgradePanel = _upgradePanels[i];
                if (!upgradePanel)
                {
                    Debug.LogWarning($"No UpgradePanel component found on instance at index {0}");
                    return;
                }

                upgradePanel.SetUpPanel(purchasedUpgrade.Key.upgrade, null, objectType);
                upgradePanel.transform.parent.gameObject.SetActive(true);
            
                // _upgradePanels.Add(upgradePanel);
            }
        }
        
    }
}
