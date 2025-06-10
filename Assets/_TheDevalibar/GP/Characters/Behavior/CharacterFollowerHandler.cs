using UnityEngine;

public class CharacterFollowerHandler : CharacterComponent
{
    [Header("Character Follower")] 
    public bool HasAFollower;
    [SerializeField] private GameObject _characterFollowerPrefab;
    [SerializeField] private Transform _characterFollowerPointToFollow;
    private CharacterFollowerBehavior _characterFollowerBehavior;
    public CharacterFollowerBehavior CharacterFollowerBehavior => _characterFollowerBehavior;
    public Transform CharacterFollowerPointToFollow => _characterFollowerPointToFollow;

    private void Start()
    {
        SetCharacterFollower();
    }
    
    private void SetCharacterFollower()
    {
        if ((_character != null && _character.CharacterFollower != null) || HasAFollower)
        {
            _characterFollowerBehavior = Instantiate(_characterFollowerPrefab, _characterFollowerPointToFollow.position, Quaternion.identity).GetComponent<CharacterFollowerBehavior>();
            if (_characterFollowerBehavior == null)
            {
                Debug.LogError("CharacterFollower is NULL");
                return;
            }

            HasAFollower = true;
            _characterFollowerBehavior.Initialize(_characterBehavior);
        }
    }

    public void StartCharacterFollower()
    {
        if (_characterFollowerBehavior != null)
        {
            _characterFollowerBehavior.StartFollowing();
        }
        else if (HasAFollower)
        {
            Debug.LogError("CharacterFollower is NULL");
        }
    }
    
    public void StopCharacterFollower()
    {
        if (_characterFollowerBehavior != null)
        {
            _characterFollowerBehavior.StopFollowing();
        }
        else if (HasAFollower)
        {
            Debug.LogError("CharacterFollower is NULL");
        }
    }

    public void ForceCharacterFollowerToStandUp()
    {
        if (_characterFollowerBehavior != null)
        {
            _characterFollowerBehavior.ForceCharacterToStandUp();
        }
        else if (HasAFollower)
        {
            Debug.LogError("CharacterFollower is NULL");
        }
    }

    public void ForceCharacterFollowerToGoToTable(Table table)
    {
        if (_characterFollowerBehavior != null)
        {
            _characterFollowerBehavior.MoveToSameTableAsCharacter(table);
        }
        else if (HasAFollower)
        {
            Debug.LogError("CharacterFollower is NULL");
        }
    }

    public void ForeCharacterFollowerToMoveToBarExit()
    {
        if (_characterFollowerBehavior != null)
        {
            _characterFollowerBehavior.StopFollowing();
            _characterFollowerBehavior.MoveToBarExit();
        }
        else if (HasAFollower)
        {
            Debug.LogWarning("CharacterFollower is NULL");
        }
    }
}
