using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyUtilities;
using UnityEngine;
using UnityEngine.EventSystems;

public class TabsManager : MonoBehaviour
{
    [SerializeField, ReadOnly] private List<Tab> _tabs = new List<Tab>();
    // [SerializeField] private GameObject TabPrefab;
    private TablesManager _tablesManager;
    private UpgradePanelManager _upgradePanelManager;
    private CameraZoomToTarget _cameraZoom;
    
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
        _cameraZoom = ServiceLocator.Get<CameraZoomToTarget>();
        if (_cameraZoom== null)
        {
            Debug.LogError("There is no Camera Zoom To Target in the scene");
        }
        // if (TabPrefab == null)
        // {
        //     Debug.LogWarning("There is no TabPrefab");
        // }
        
        _tabs = new List<Tab>();
        for (int i = 0; i < transform.childCount; i++)
        {
            _tabs.Add(transform.GetChild(i).GetComponent<Tab>());
        }
        
        
        InitializeTabs();
        
    }

    public void CheckIfSelectedGameObjectIsNull()
    {
        StartCoroutine(CheckIfSelectedGameObjectIsNullCoroutine());
    }

    IEnumerator CheckIfSelectedGameObjectIsNullCoroutine()
    {
        yield return new WaitForSeconds(1);
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            SelectFirstTabs();
        }
    }
    public void SelectFirstTabs()
    {
        if (_tabs.Count == 0) return;
        EventSystem.current.SetSelectedGameObject(_tabs[0].gameObject);
        ResetFocus();
    }

    void InitializeTabs()
    {
        // Tables
        ClearTabs();
        if (_tablesManager == null) return;

        for (int i = 0; i < _tablesManager.Tables.Count; i++)
        {
            // Instantiate(TabPrefab, transform);
            if (_tabs[i] == null)
            {
                Debug.LogWarning("There is no TabPrefab in the prefab");
                continue;
            }
            _tabs[i].Initialize(_tableSprite, ObjectType.Table,i);
            _tabs[i].gameObject.SetActive(true);
        }
        
        // Elevator
        // tab = Instantiate(TabPrefab, transform).GetComponent<Tab>();
        Tab tab = _tabs.FirstOrDefault(x => !x.gameObject.activeInHierarchy);
        if (tab == null)
        {
            Debug.LogWarning("There is no TabPrefab available");
            return;
        }
        ElevatorManager elevator = ServiceLocator.Get<ElevatorManager>();
        if (elevator == null) return;
        tab.Initialize(_elevatorSprite, ObjectType.Elevator);
        tab.gameObject.SetActive(true);
    }

    void ClearTabs()
    {
        // _tabs.Clear();
        // for (int i = 0; i < transform.childCount; i++)
        // {
        //     Destroy(transform.GetChild(i).gameObject);
        // }
        if (_tablesManager == null) return;
        _tabs.ForEach(x => x.gameObject.SetActive(false));
    }
    

    public void ResetFocus()
    {
        _cameraZoom.ResetCamera(); // Appelle la fonction de retour caméra
        _upgradePanelManager.ClearUpgradePanels(true);
    }

    public void FocusCameraOnTab(int TabID)
    {
        foreach (Tab tab in _tabs)
        {
            if (tab.TabID == TabID)
            {
                tab.FocusCameraOnTabObject();
                return;
            }
        }
        Debug.LogError("There is no tab with the ID " + TabID);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
