using System.Collections;
using MyUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image _upgradeImage;
    [SerializeField] private TextMeshProUGUI _upgradeTitleText;
    [SerializeField] private Button _showUpgradeButton;
    [SerializeField] private TextMeshProUGUI _upgradeDescriptionText;
    [SerializeField] private Button _buyUpgradeButton;
    [SerializeField] private TextMeshProUGUI _upgradePriceText;

    [Header("Panel Movements")]
    [SerializeField] private RectTransform _upgradePanelRectTransform;
    [SerializeField] private bool _isUpgradeFullyDisplayed;
    public bool IsUpgradeFullyDisplayed => _isUpgradeFullyDisplayed;
    private Vector2 _panelStartPosition;
    [SerializeField] private float _panelEndPositionX;
    private Vector2 _panelEndPosition => new Vector2(_panelEndPositionX, _panelStartPosition.y);
    [SerializeField] private float _transitionSpeed = 2;
    [SerializeField] private AnimationCurve _transitionCurve;
    private Coroutine _transitionCoroutine;
    
    
    
    public bool IsPanelTransitioning { get; private set; }
    private UpgradePanelManager _upgradePanelManager;
    
    [Header("Upgrade")]
    [SerializeField, ReadOnly] private Upgrade _currentUpgrade;
    private ObjectType _currentObjectType;
    [SerializeField, ReadOnly] private Table _currentTable;
    
    private GameManager _gameManager;

    public void StopTransitionCoroutine()
    {
        if (_transitionCoroutine != null)
        {
            StopCoroutine(_transitionCoroutine);
            _transitionCoroutine = null;
        }
    }

    public void MovePanelToEndPosition()
    {
        StopTransitionCoroutine();
        if (_upgradePanelRectTransform.anchoredPosition == _panelEndPosition) return;
        _transitionCoroutine = StartCoroutine(MoveUpgradePanelCoroutine(_panelEndPosition));
    }
    
    public void MovePanelToStartPosition()
    {
        StopTransitionCoroutine();
        if (_upgradePanelRectTransform.anchoredPosition == _panelStartPosition) return;
        _transitionCoroutine = StartCoroutine(MoveUpgradePanelCoroutine(_panelStartPosition));
    }

    private IEnumerator MoveUpgradePanelCoroutine(Vector2 targetPosition)
    {
        IsPanelTransitioning = true;

        // On part de la position actuelle
        Vector2 from = _upgradePanelRectTransform.anchoredPosition;
        Vector2 target = targetPosition;

        // Mesure la distance pour adapter la durée restante
        float fullDistance = Vector2.Distance(_panelStartPosition, _panelEndPosition);
        float remainingDistance = Vector2.Distance(from, target);

        float duration = (1f / _transitionSpeed) * (remainingDistance / fullDistance);
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float curvedT = _transitionCurve.Evaluate(t);

            _upgradePanelRectTransform.anchoredPosition = Vector2.Lerp(from, target, curvedT);
            yield return null;
        }
        
        _isUpgradeFullyDisplayed = targetPosition == _panelEndPosition;
        _upgradePanelRectTransform.anchoredPosition = target;
        IsPanelTransitioning = false;
    }
    

    public void ResetUpgradePanelPosition()
    {
        _isUpgradeFullyDisplayed = false;
        _upgradePanelRectTransform.anchoredPosition = _panelStartPosition;
        IsPanelTransitioning = false;
    }

    public void SetUpPanel(Upgrade upgrade, Table table = null, ObjectType objectType = ObjectType.None)
    {
        if (!upgrade)
        {
            Debug.LogError("The upgrade is null");
            return;
        }
        _upgradeImage.sprite = upgrade.UpgradeSprite;
        _upgradeTitleText.text = upgrade.UpgradeName;
        _upgradeDescriptionText.text = upgrade.UpgradeDescription;
        _upgradePriceText.text = upgrade.UpgradeCost + "€";
        _currentUpgrade = upgrade;
        if (table != null)
        {
            _currentObjectType = ObjectType.Table;
            _currentTable = table;
        }
        else if (objectType != ObjectType.None)
        {
            _currentObjectType = objectType;
        }
        
        
    }


    public void TryBuyUpgrade()
    {
        if ((_currentTable == null && _currentObjectType == ObjectType.Table) || _currentUpgrade == null)
        {
            Debug.LogError("The upgrade is null or the current table is null");
            return;
        }

        if (_gameManager == null)
        {
            Debug.LogError("The game manager is null");
            return;
        }

        if (_gameManager.GameData == null)
        {
            Debug.LogError("The game data is null");
            return;
        }

        if (_gameManager.GameData.Gold >= _currentUpgrade.UpgradeCost)
        {
            _gameManager.GameData.Gold -= _currentUpgrade.UpgradeCost;
            _gameManager.UpdateGoldValue();
            switch (_currentObjectType)
            {
                case ObjectType.Table:
                {
                    if (_currentTable != null)
                    {
                        TableUpgrade tableUpgrade = _currentUpgrade as TableUpgrade;
                        _currentTable.BuyUpgrade(tableUpgrade);
                    }
                    break;
                }
                default:
                {
                    _currentUpgrade.InvokeUpgrade();
                    break;
                }
                    
            }
            
            // Destroy(gameObject.transform.parent.gameObject); // On détruit le panel quand il est acheté
            _upgradePanelManager?.ResetCurrentUpgradePanel(this);
            ResetUpgradePanelPosition();
            gameObject.transform.parent.gameObject.SetActive(false);
            _upgradePanelManager?.SelectFirstAvailableUpgradePanel();
        }
        else
        {
            Debug.LogWarning("The upgrade cost is out of range");
        }
    }
    


    public void MoveUpgradePanel()
    {
        _upgradePanelManager?.UpdateCurrentUpgradePanel(this);
    }

    private void OnEnable()
    {
        _showUpgradeButton?.onClick.AddListener(() => EventSystem.current.SetSelectedGameObject(transform.parent.gameObject));
    }

    private void OnDisable()
    {
        _showUpgradeButton?.onClick.RemoveListener(() => EventSystem.current.SetSelectedGameObject(transform.parent.gameObject));
    }

    void Start()
    {
        _panelStartPosition = _upgradePanelRectTransform.anchoredPosition;
        _upgradePanelManager = ServiceLocator.Get<UpgradePanelManager>();
        _gameManager = ServiceLocator.Get<GameManager>();
        _buyUpgradeButton?.onClick.AddListener(TryBuyUpgrade);
    }
    
}

public enum ObjectType
{
    None,
    Table,
    Stairs,
    Elevator,
    WC
}
