using System;
using MyUtilities;
using UnityEngine;

public class CharacterAssets : CharacterComponent
{
    
    [Header("Character Assets")]
    [SerializeField] private GameObject _wheelChair;
    public GameObject WheelChair => _wheelChair;
    
    [SerializeField] private string  _wheelChairDisabiltyName = "Mobilité réduite sévère";
    public string WheelChairDisabiltyName => _wheelChairDisabiltyName;
    
    [SerializeField] private GameObject _blindCane;
    public GameObject BlindCane => _blindCane;
    
    [SerializeField] private string  _blindCaneDisabilityName = "Déficience visuelle sévère";
    public string BlindCaneDisabilityName => _blindCaneDisabilityName;
    
    private CharacterSpawnManager _characterSpawnManager;
    
    public override void Init(CharacterBehavior characterBehavior)
    {
        base.Init(characterBehavior);
        ServiceLocator.RequireService(this, ref _characterSpawnManager, "No CharacterSpawnManager in scene");
        if (_character == null) SetNPC();
        
    }


    private void SetNPC()
    {  
        NPCMeshMaterialController npcMeshMaterialController = _characterSpawnManager.GetRandomNPCAssets();
        SetNPC(npcMeshMaterialController.Mesh, npcMeshMaterialController.Material, npcMeshMaterialController.AnimatorController);
    }

    private void SetNPC(Mesh npcMesh, Material npcMaterial, RuntimeAnimatorController runtimeAnimatorController)
    {
        _characterBehavior.CharacterAnimationManager?.SetAnimation(npcMesh,
            npcMaterial,
            _characterBehavior.CharacterDisabilityHandler.CharacterDisability == _wheelChairDisabiltyName ? runtimeAnimatorController : null);
    }
}
