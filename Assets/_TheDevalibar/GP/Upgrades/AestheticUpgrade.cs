using UnityEngine;
using UnityEngine.Events;

public class AestheticUpgrade : MonoBehaviour
{

    [SerializeField] private bool _isUpgradePurchased;
    public UnityEvent OnUpgradePurchased;

    public void BuyUpgrade()
    {
        _isUpgradePurchased = true;
        OnUpgradePurchased?.Invoke();
    }
    
    
}

