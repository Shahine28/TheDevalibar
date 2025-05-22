using NaughtyAttributes;
using UnityEngine;


[CreateAssetMenu(fileName = "Upgrade", menuName = "Scriptable Objects/TableUpgrade")]
public class TableUpgrade : Upgrade
{
    [Header("Aesthetics Upgrades")] 
    [Foldout("Table")] public Mesh NewUpgradeTableMesh;
    [Foldout("Table")]public Material NewUpgradeTableMaterial;
    [Foldout("Chair1")]public Mesh NewUpgradeChair1Mesh;
    [Foldout("Chair1")]public Material NewUpgradeChair1Material;
    [Foldout("Chair2")]public Mesh NewUpgradeChair2Mesh;
    [Foldout("Chair2")]public Material NewUpgradeChair2Material;

    [Header("Is Upgrade adding object on table")]
    public AccessibilityUpgrade AccessibilityUpgrade;
}

[System.Serializable]
public enum AccessibilityUpgrade
{
    None,
    ErgonomicCutlery,
    ArticulatedArmRest,
    NoiseCancellingHeadphones,
    BrailleMenu,
    InductionLoopSystem
}
