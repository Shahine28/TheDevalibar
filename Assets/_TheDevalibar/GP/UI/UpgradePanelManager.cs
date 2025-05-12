using System.Collections.Generic;
using MyUtilities;
using UnityEngine;

public class UpgradePanelManager : MonoBehaviour
{
    [SerializeField, ReadOnly] private List<UpgradePanel> upgradePanels = new List<UpgradePanel>();

    private UpgradePanel currentUpgradePanel;
    private Coroutine _currentUpgradePanelCoroutine;
    void Awake()
    {
        ServiceLocator.Register(this);
        upgradePanels.Clear();
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


    public void AddUpgradePanel(UpgradePanel upgradePanel)
    {
        upgradePanels.Add(upgradePanel);
    }

    public void RemoveUpgradePanel(UpgradePanel upgradePanel)
    {
        upgradePanels.Remove(upgradePanel);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
