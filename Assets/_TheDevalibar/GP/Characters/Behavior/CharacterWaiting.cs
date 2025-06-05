using System.Collections;
using MyUtilities;
using UnityEngine;

public class CharacterWaiting : CharacterComponent
{
    [Header("WaitingTime")] 
    [SerializeField] private Vector2 _waitingTimeRange;
    private Coroutine _waitRoutine;
    private bool _interrupted = false;
    private bool _isPaused = false;

    private DialogueManager _dialogueManager;
    
    
    void Init()
    {
        ServiceLocator.RequireService(this, ref _dialogueManager, "No Dialogue Manager in scene");
    }
    private void OnEnable()
    {
        if (!_dialogueManager)
        {
            Init();
        }
        if (_dialogueManager)
        {
            _dialogueManager._onDialogueStart += PauseWaiting;
            _dialogueManager._onDialogueEnd += ResumeWaiting;
        }
    }

    private void OnDisable()
    {
        if (!_dialogueManager)
        {
            Init();
        }
        if (_dialogueManager)
        {
            _dialogueManager._onDialogueStart -= PauseWaiting;
            _dialogueManager._onDialogueEnd -= ResumeWaiting;
        }
    }
    
    
    public void StartWaiting()
    {
        _interrupted = false;
        _waitRoutine = StartCoroutine(WaitAtTable());
    }

    public void PauseWaiting()
    {
        if (_waitRoutine != null)
        {
            _isPaused = true;
        }
    }

    public void ResumeWaiting()
    {
        if (_waitRoutine != null && _isPaused)
        {
            _isPaused = false;
        }
    }
    public void CancelWaiting()
    {
        _interrupted = true;

        if (_waitRoutine != null)
            StopCoroutine(_waitRoutine);
    }

    private IEnumerator WaitAtTable()
    {
        float waitTime = UnityEngine.Random.Range(_waitingTimeRange.x, _waitingTimeRange.y);
        float elapsed = 0f;
        float deltaBubbleSpeech = waitTime / 4;

        while (elapsed < waitTime)
        {
            while (_isPaused) yield return null;
            if (elapsed >= deltaBubbleSpeech && elapsed < waitTime - deltaBubbleSpeech && !_characterBehavior.BubbleSpeechPanel.gameObject.activeInHierarchy) // On le fait apparaite
            {
                _characterBehavior.SetBubbleSpeech(false);
            }
            else if (elapsed >= waitTime - deltaBubbleSpeech && _characterBehavior.BubbleSpeechPanel.gameObject.activeInHierarchy) // On le fait disparaitre
            {
                _characterBehavior.BubbleSpeechPanel.gameObject.SetActive(false);
            }
            if (_interrupted)
            {
                Debug.Log("Waiting at the bar was interrupted.");
                if (_characterBehavior.BubbleSpeechPanel.gameObject.activeInHierarchy) _characterBehavior.BubbleSpeechPanel.gameObject.SetActive(false);
                // MoveToBarExit();
                
                _characterFollowerHandler.ForceCharacterFollowerToStandUp();
                _characterAnimationManager.StandUp();
                _characterMovement.FreeChair();
                if (_characterMovement.UsedTable)
                {
                    // _usedTable.IsUsedByCustomer = false;
                    _characterMovement.UsedTable = null;
                }
                yield break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Finished waiting at the bar.");
        // MoveToBarExit();
        _characterFollowerHandler.ForceCharacterFollowerToStandUp();
        _characterAnimationManager.StandUp();
        if (_characterDisabilityHandler.CharacterConstraint is { CanTakeStairs: false } || _characterDisabilityHandler.CharacterDisability == _characterAssets.WheelChairDisabiltyName)
        {
            _characterMovement.UsedChair.ShowChair();
        }
        else
        {
            _characterMovement.UsedChair.MoveChairToUnoccupiedPosition();
        }
    }
}
