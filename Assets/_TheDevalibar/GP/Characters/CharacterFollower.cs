using UnityEngine;
using NaughtyAttributes;



[CreateAssetMenu(fileName = "CharacterFollower", menuName = "Scriptable Objects/CharacterFollower"), System.Serializable]
public class CharacterFollower : ScriptableObject
{
    public AffinityManager AffinityManager;

    [Header("Character Information")]
    public Mesh CharacterMesh;
    public Material CharacterMaterial;
    public bool HasSpecificRuntimeAnimationController;
    [ShowIf("HasSpecificRuntimeAnimationController"), SerializeField] public RuntimeAnimatorController CharacterRuntimeAnimatorController;
    
    
    public ConstraintBoolDictionary constraintDict = new ConstraintBoolDictionary();
}

