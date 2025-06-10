using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using UnityEngine;


public class UpgradablePropsManager : MonoBehaviour
{
    [SerializedDictionary("Upgrade", "Is Purchased")]
    public SerializedDictionary<AestheticUpgrade, bool> PurchasedUpgrades = new SerializedDictionary<AestheticUpgrade, bool>();
    
    protected virtual void Awake()
    {
        // _stairUpgrade?.OnUpgrade.AddListener(BuyUpgrade);
        foreach (var purchasedUpgrade in PurchasedUpgrades)
        {
            purchasedUpgrade.Key.upgrade.OnUpgrade.AddListener(() => BuyUpgrade(purchasedUpgrade.Key));
        }
    }
    

    [Button]
    public void BuyUpgrade()
    {
        foreach (var purchasedUpgrade in PurchasedUpgrades)
        {
            if (!purchasedUpgrade.Value)
            {
                BuyUpgrade(purchasedUpgrade.Key);
                return;
            }
        }
    }
    
    public void BuyUpgrade(Upgrade upgrade)
    {
        foreach (var purchasedUpgrade in PurchasedUpgrades)
        {
            if (purchasedUpgrade.Key.upgrade == upgrade)
            {
                BuyUpgrade(purchasedUpgrade.Key);
                return;
            }
        }
    }

    public void BuyUpgrade(AestheticUpgrade upgrade)
    {
        PurchasedUpgrades[upgrade] = true;
        upgrade.BuyAestheticUpgrade();
    }
    
    [Button]
    public void SellUpgrade()
    {
        foreach (var purchasedUpgrade in PurchasedUpgrades)
        {
            if (purchasedUpgrade.Value)
            {
                SellUpgrade(purchasedUpgrade.Key);
                return;
            }
        }
    }
    
    public void SellUpgrade(AestheticUpgrade upgrade)
    {
        PurchasedUpgrades[upgrade] = false;
        upgrade.SellAestheticUpgrade();
    }
}

[Serializable]
public struct AestheticUpgrade
{
    public Upgrade upgrade;
    public List<GameObject> upgradeItems;

    public void BuyAestheticUpgrade()
    {
        upgradeItems?.ForEach(item => item.SetActive(true));
    }

    public void SellAestheticUpgrade()
    {
        upgradeItems?.ForEach(item => item.SetActive(false));
    }
    
}