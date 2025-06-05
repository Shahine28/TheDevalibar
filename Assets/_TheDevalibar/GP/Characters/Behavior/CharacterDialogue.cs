using System;
using MyUtilities;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDialogue : CharacterComponent
{
    [Header("Dialogue")]
    [SerializeField] private DialogueManager _dialogueManager;
    [SerializeField] private Button _dialogueButton;
    public Button DialogueButton => _dialogueButton;

    ///  Camera
    private CameraMovementAndZoomControl _cameraMovementAndZoomControl;
    private GameManager _gameManager;
    private CharacterSpawnManager _characterSpawnManager;
    private CameraZoomToTarget _cameraZoomToTarget;

    public override void Init(CharacterBehavior characterBehavior)
    {
        base.Init(characterBehavior);
        ServiceLocator.RequireService(this, ref _dialogueManager, "No Dialogue Manager in scene");
        ServiceLocator.RequireService(this, ref _cameraMovementAndZoomControl, "No CameraMovementAndZoomControl in scene");
        ServiceLocator.RequireService(this, ref _gameManager, "No GameManager in scene");
        ServiceLocator.RequireService(this, ref _characterSpawnManager, "No CharacterSpawnManager in scene");
        ServiceLocator.RequireService(this, ref _cameraZoomToTarget, "No camera zoom to camera");
    }
    private void OnEnable()
    {
        _dialogueButton?.onClick.AddListener(StartCharacterDialogue);
    }

    private void OnDisable()
    {
        
        _dialogueButton?.onClick.RemoveListener(StartCharacterDialogue);
    }

    private void StartCharacterDialogue()
    {
        if (!_dialogueManager)
        {
            Debug.LogError("No Dialogue Manager in scene");
            return;
        }
        _dialogueManager?.InitCharacterDialogue(_characterBehavior, false, _gameManager != null ? _gameManager.GameData.LanguageCode : CodeLanguage.French);
        _dialogueButton?.gameObject.SetActive(false);
        
        _cameraMovementAndZoomControl.CanZoom = false;
    }
    

    public void OnDialogueEnd()
    {
        if (_characterSpawnManager)
        {
            _characterSpawnManager.CanSpawnCharacter = true;
        }
        _cameraZoomToTarget.ResetCamera();
        _cameraMovementAndZoomControl.CanZoom = true;
        _characterBehavior.CharacterMovement.MoveToBestTable();
    }
}
