using MyUtilities;
using UnityEngine;

public class EndUpgradePhase : MonoBehaviour
{
    private TablesManager _tablesManager;
    private ShowHideUI _showHideUI;
    private TabsManager _tabsManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _tablesManager = ServiceLocator.Get<TablesManager>();
        if (_tablesManager == null)
        {
            Debug.LogError("There is no TablesManager in the scene.");
        }
        _showHideUI = ServiceLocator.Get<ShowHideUI>();
        if (_showHideUI == null)
        {
            Debug.LogError("There is no ShowHideUI in the scene.");
        }
        _tabsManager = ServiceLocator.Get<TabsManager>();
        if (_tabsManager == null)
        {
            Debug.LogError("There is no TabsManager in the scene.");
        }
    }


    public void EndUpgradePhaseButton()
    {
        if (_tabsManager == null || _showHideUI == null || _tablesManager == null)
        {
            Debug.LogError("One or more component are null");
            return;
        }
        _tabsManager.ResetFocus();
        _showHideUI.HideUI();
        _tablesManager.HideUpgradeButtonTables();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
