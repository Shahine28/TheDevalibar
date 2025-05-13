using System;
using System.Collections;
using MyUtilities;
using TMPro;
using UnityEngine;
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
    private Vector2 _panelStartPosition;
    [SerializeField] private float _panelEndPositionX;
    private Vector2 _panelEndPosition => new Vector2(_panelEndPositionX, _panelStartPosition.y);
    [SerializeField] private float _transitionSpeed = 2;
    [SerializeField] private AnimationCurve _transitionCurve;
    public bool IsPanelTransitioning { get; private set; }
    private UpgradePanelManager _upgradePanelManager;
    
    
    public IEnumerator MoveUpgradePanelCoroutine()
    {
        IsPanelTransitioning = true;

        // On part de la position actuelle
        Vector2 from = _upgradePanelRectTransform.anchoredPosition;
        Vector2 target = _isUpgradeFullyDisplayed ? _panelStartPosition : _panelEndPosition;

        // Mesure la distance pour adapter la durée restante
        float fullDistance = Vector2.Distance(_panelStartPosition, _panelEndPosition);
        float remainingDistance = Vector2.Distance(from, target);

        float duration = (1f / _transitionSpeed) * (remainingDistance / fullDistance);
        float elapsed = 0f;

        // Inverse l'état pour la prochaine fois
        _isUpgradeFullyDisplayed = !_isUpgradeFullyDisplayed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float curvedT = _transitionCurve.Evaluate(t);

            _upgradePanelRectTransform.anchoredPosition = Vector2.Lerp(from, target, curvedT);
            yield return null;
        }

        _upgradePanelRectTransform.anchoredPosition = target;
        IsPanelTransitioning = false;
    }

    public void SetUpPanel(Upgrade upgrade)
    {
        if (!upgrade)
        {
            Debug.LogError("The upgrade is null");
            return;
        }
        _upgradeImage.sprite = upgrade.UpgradeSprite;
        _upgradeTitleText.text = upgrade.UpgradeName;
        _upgradeDescriptionText.text = upgrade.UpgradeDescription;
        _upgradePriceText.text = upgrade.UpgradeCost.ToString() + "€";
    }
    
    
    


    public void MoveUpgradePanel()
    {
        _upgradePanelManager?.UpdateCurrentUpgradePanel(this);
    }

    private void OnEnable()
    {
        _showUpgradeButton?.onClick.AddListener(MoveUpgradePanel);
    }

    private void OnDisable()
    {
        _showUpgradeButton?.onClick.RemoveListener(MoveUpgradePanel);
    }

    void Start()
    {
        _panelStartPosition = _upgradePanelRectTransform.anchoredPosition;
        _upgradePanelManager = ServiceLocator.Get<UpgradePanelManager>();
        if (_upgradePanelManager)
        {
            _upgradePanelManager.AddUpgradePanel(this);
        }
    }

    void OnDestroy()
    {
        if (_upgradePanelManager)
        {
            _upgradePanelManager.RemoveUpgradePanel(this);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
