using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Scriptable Objects/Upgrade")]
public class Upgrade : ScriptableObject
{
    public Sprite UpgradeSprite;
    public string UpgradeName;
    [TextArea] public string UpgradeDescription;
    public int UpgradeCost;
    public ConstraintBoolDictionary AccessibiltiesToAddWithUpgrade;
}
