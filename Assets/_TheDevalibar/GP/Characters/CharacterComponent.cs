using UnityEngine;

public class CharacterComponent : MonoBehaviour
{
    protected Character _character;
    protected CharacterBehavior _characterBehavior;
    
    protected CharacterAnimationManager _characterAnimationManager;
    protected CharacterAssets _characterAssets;
    protected CharacterDialogue _characterDialogue;
    protected CharacterDisabilityHandler _characterDisabilityHandler;
    protected CharacterFeedback _characterFeedback;
    protected CharacterFollowerHandler _characterFollowerHandler;
    protected CharacterMovement _characterMovement;
    protected CharacterWaiting _characterWaiting;
    

    public virtual void Init(CharacterBehavior characterBehavior)
    {
        _characterBehavior = characterBehavior;
        if (!_characterBehavior) return;
        
        _character = _characterBehavior.Character;
        _characterAnimationManager = _characterBehavior.CharacterAnimationManager;
        _characterAssets = _characterBehavior.CharacterAssets;
        _characterDialogue = _characterBehavior.CharacterDialogue;
        _characterDisabilityHandler = _characterBehavior.CharacterDisabilityHandler;
        _characterFeedback = _characterBehavior.CharacterFeedback;
        _characterFollowerHandler = _characterBehavior.CharacterFollowerHandler;
        _characterMovement = _characterBehavior.CharacterMovement;
        _characterWaiting = _characterBehavior.CharacterWaiting;
        
        
    }
}
