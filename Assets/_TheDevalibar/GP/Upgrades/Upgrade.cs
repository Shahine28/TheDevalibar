using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Scriptable Objects/Upgrade")]
public class Upgrade : ScriptableObject
{
    public Sprite UpgradeSprite;
    public string UpgradeName;
    [TextArea] public string UpgradeDescription;
    public int UpgradeCost;
    public ConstraintBoolDictionary AccessibiltiesToAddWithUpgrade;


    

    /// <summary>
    /// Get First Upgrade 
    /// </summary>
    /// <returns></returns>
    public string GetUpgrade()
    {
        return AccessibiltiesToAddWithUpgrade.keys
            .Where((key, i) => i < AccessibiltiesToAddWithUpgrade.values.Count && AccessibiltiesToAddWithUpgrade.values[i])
            .FirstOrDefault();
    }
        

    public List<string> GetUpgrades()
    {
        return AccessibiltiesToAddWithUpgrade.keys
            .Where((key, i) => i < AccessibiltiesToAddWithUpgrade.values.Count && AccessibiltiesToAddWithUpgrade.values[i])
            .ToList();
    }
    

}
