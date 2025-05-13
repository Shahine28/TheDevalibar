using System;
using System.Collections.Generic;
using MyUtilities;
using UnityEngine;

public class TabsManager : MonoBehaviour
{
    private List<Tab> _tabs = new List<Tab>();
    [SerializeField] private GameObject TabPrefab;
    private TablesManager _tablesManager;
    private UpgradePanelManager _upgradePanelManager;
    
    [Header("Sprites")]
    [SerializeField] private Sprite _tableSprite;
    [SerializeField] private Sprite _wcSprite;
    [SerializeField] private Sprite _elevatorSprite;
    [SerializeField] private Sprite _stairSprite;

    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    void Start()
    {
        _tablesManager = ServiceLocator.Get<TablesManager>();
        if (_tablesManager == null)
        {
            Debug.LogError("There is no TablesManager in the scene");
        }
        _upgradePanelManager = ServiceLocator.Get<UpgradePanelManager>();
        if (_upgradePanelManager == null)
        {
            Debug.LogError("There is no UpgradePanelManager in the scene");
        }

        if (TabPrefab == null)
        {
            Debug.LogWarning("There is no TabPrefab");
        }
        
        InitializeTabs();
    }

    void InitializeTabs()
    {
        // Tables
        ClearTabs();
        if (_tablesManager == null) return;
        for (int i = 0; i < _tablesManager.Tables.Count; i++)
        {
            Tab tab = Instantiate(TabPrefab, transform).GetComponent<Tab>();
            if (tab == null)
            {
                Debug.LogWarning("There is no TabPrefab in the prefab");
                continue;
            }
            tab.Initialize(_tableSprite, ObjectType.Table,i);
        }
    }

    void ClearTabs()
    {
        _tabs.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
