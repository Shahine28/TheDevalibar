using MyUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tab : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image _tabImage;
    [SerializeField] private Button _tabButton;
    [SerializeField] private TextMeshProUGUI _tabIDText;
    private int _tabID = -1;
    public int TabID => _tabID;
    [SerializeField] private ObjectType _objectType;
    
    private CameraZoomToTarget _cameraZoomToTarget;
    private UpgradePanelManager _upgradePanelManager;
    
    void Start()
    {
        _cameraZoomToTarget = ServiceLocator.Get<CameraZoomToTarget>();
        _upgradePanelManager = ServiceLocator.Get<UpgradePanelManager>();
        if (_cameraZoomToTarget == null)
        {
            Debug.LogError("CameraZoomToTarget is null");
        }
        _tabButton?.onClick.AddListener(FocusCameraOnTabObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void Initialize(Sprite image, ObjectType objectType, int tabID = -1)
    {
        _objectType = objectType;
        _tabImage.sprite = image;
        if (tabID != -1)
        {
            _tabIDText.text = tabID.ToString();
            _tabID = tabID;
            _tabIDText.gameObject.SetActive(true); // sécurité si jamais désactivé auparavant
        }
        else
        {
            _tabIDText.gameObject.SetActive(false);
        }
    }

    public void FocusCameraOnTabObject()
    {
        if (_cameraZoomToTarget == null)
        {
            Debug.LogError("CameraZoomToTarget is null");
            return;
        }
        switch (_objectType)
        {
            case ObjectType.Table:
            {
                TablesManager tablesManager = ServiceLocator.Get<TablesManager>();
                if (tablesManager == null)
                {
                    Debug.LogError("There is no table manager in the scene.");
                    return;
                }
                _cameraZoomToTarget.ZoomTo(tablesManager.Tables[_tabID].gameObject.transform);
                if (_upgradePanelManager == null)
                {
                    Debug.LogError("There is no upgrade panel manager in the scene.");
                    return;
                }
                _upgradePanelManager.SetUpTableUpgradePanels(tablesManager.Tables[_tabID]);
                break;
            }
        }
    }
}
