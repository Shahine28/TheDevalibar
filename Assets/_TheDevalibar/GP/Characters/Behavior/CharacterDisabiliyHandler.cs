using System.Linq;
using MyUtilities;
using NaughtyAttributes;
using UnityEngine;

public class CharacterDisabilityHandler : CharacterComponent
{
    [SerializeField, ReadOnly] private string _characterDisability = string.Empty;
    public string CharacterDisability => _characterDisability;

    public Constraint CharacterConstraint { get; private set; }
    
    private bool _isCharacterAssigned => _character != null;
    [SerializeField, Range(0, 100), HideIf("_isCharacterAssigned")]
    private float _disabilityChance = 20;

    private NodeManager _nodeManager;
    
    public override void Init(CharacterBehavior characterBehavior)
    {
        base.Init(characterBehavior);
        
        ServiceLocator.RequireService(this, ref _nodeManager, "No node manager");
        
        
        SetCharacterDisabilities();
        CharacterConstraint = GetCharacterConstraint();
        
        
    }
    
    public void SetCharacterDisabilities()
    {
        if (_character)
        {
            _characterDisability = _character.constraintDict.keys
                .Select((key, i) => new { key, isActive = _character.constraintDict.values[i] })
                .FirstOrDefault(x => x.isActive)?.key ?? "";
        }
        else
        {
            float randomNumber = UnityEngine.Random.Range(0, 100);
            if (randomNumber <= _disabilityChance)
            {
                if (!_nodeManager) _nodeManager = ServiceLocator.Get<NodeManager>();
                int randomDisabiltyIndex = UnityEngine.Random.Range(0, _nodeManager.constraints.Count-1);
                _characterDisability = _nodeManager.constraints[randomDisabiltyIndex].name;
            }
            else
            {
                _characterDisability = string.Empty;
            }
        }

        if (_characterDisability != _characterAssets.WheelChairDisabiltyName)
        {
            _characterAssets.WheelChair?.gameObject.SetActive(false);
        }
        if (_characterDisability != _characterAssets.BlindCaneDisabilityName)
        {
            _characterAssets.BlindCane?.gameObject.SetActive(false);
        }
    }

    private Constraint GetCharacterConstraint()
    {
        if (_characterDisability == "")
        {
            return null;
        }

        return _nodeManager.constraints.FirstOrDefault(x => x.name == _characterDisability);
    }
}
