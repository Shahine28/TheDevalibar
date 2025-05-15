using NaughtyAttributes;
using UnityEngine;


[CreateAssetMenu(fileName = "Upgrade", menuName = "Scriptable Objects/TableUpgrade")]
public class TableUpgrade : Upgrade
{
    [Header("Aesthetics Upgrades")] 
    [ReadOnly] public string useless;
    [Foldout("Table")] public Mesh NewUpgradeTableMesh;
    [Foldout("Table")]public Material NewUpgradeTableMaterial;
    [Foldout("Chair1")]public Mesh NewUpgradeChair1Mesh;
    [Foldout("Chair1")]public Material NewUpgradeChair1Material;
    [Foldout("Chair2")]public Mesh NewUpgradeChair2Mesh;
    [Foldout("Chair2")]public Material NewUpgradeChair2Material;
}
