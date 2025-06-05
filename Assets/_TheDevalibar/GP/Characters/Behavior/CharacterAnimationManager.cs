using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class CharacterAnimationManager : CharacterComponent
{
    [SerializeField] private Animator animator;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    public event Action OnCharacterSitDown;
    public event Action OnCharacterStandUp;
    
    
    public void Initialize(Character character)
    {
        _character = character;
        if (_character.CharacterMesh == null)
        {
            _spriteRenderer.sprite = _character.CharacterSprite; 
            _spriteRenderer.gameObject.SetActive(true);// temporary
            gameObject.SetActive(false);
        }
        else
        {
            _spriteRenderer?.gameObject.SetActive(false);
            gameObject.SetActive(true);
            
            SetAnimation(_character.CharacterMesh,
                _character.CharacterMaterial,
                _character.HasSpecificRuntimeAnimationController ? _character.CharacterRuntimeAnimatorController : null);
        }
    }
    public void SetAnimation(Mesh characterMesh, Material characterMaterial,
        RuntimeAnimatorController animatorController = null)
    {
        skinnedMeshRenderer.sharedMesh = characterMesh;
        skinnedMeshRenderer.materials = new[] { characterMaterial };
        if (animatorController != null) animator.runtimeAnimatorController = animatorController;
    }


    public void StartMovement()
    {
        animator.SetFloat("Speed", 1);
    }
    
    public void StopMovement()
    {
        animator.SetFloat("Speed", 0);
    }

    public void SitDown()
    {
        animator.SetTrigger("SitDown");
    }

    public void CharacterSitDown()
    {
        OnCharacterSitDown?.Invoke();
    }
    
    public void StandUp()
    {
        animator.SetTrigger("StandUp");
    }

    public void CharacterStandUp()
    {
        OnCharacterStandUp?.Invoke();
    }

    public void IsTalking(bool isTalking)
    {
        animator?.SetBool("Talking", isTalking);
    }
}
