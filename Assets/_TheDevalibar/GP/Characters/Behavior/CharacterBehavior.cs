using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _TheDevalibar.GP.Characters;
using MyUtilities;
using NaughtyAttributes;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterBehavior : MonoBehaviour
{
    [Header("Character Behavior")]
    [SerializeField] private Character _character;
    public Character Character => _character;
    private CharacterSpawnManager _characterSpawnManager;
    public CharacterSpawnManager CharacterSpawnManager => _characterSpawnManager;
    
    [Header("Bubble Speech")]
    [SerializeField] private GameObject _bubbleSpeechPanel;
    public GameObject BubbleSpeechPanel => _bubbleSpeechPanel;
    
    [SerializeField] private TextMeshProUGUI _bubbleSpeechText;
    public TextMeshProUGUI BubbleSpeechText => _bubbleSpeechText;
    
    private BubbleSpeechManager _bubbleSpeechManager;
    public BubbleSpeechManager BubbleSpeechManager => _bubbleSpeechManager;

    
    
    [Header("Character Components")]
    [SerializeField] private CharacterAnimationManager _characterAnimationManager;
    public CharacterAnimationManager CharacterAnimationManager => _characterAnimationManager;
    
    [SerializeField] private CharacterAssets _characterAssets;
    public CharacterAssets CharacterAssets => _characterAssets;
    
    [SerializeField] private CharacterDialogue _characterDialogue;
    public CharacterDialogue CharacterDialogue => _characterDialogue;
    
    [SerializeField] private CharacterDisabilityHandler _characterDisabilityHandler;
    public CharacterDisabilityHandler CharacterDisabilityHandler => _characterDisabilityHandler;
    
    [SerializeField] private CharacterFeedback _characterFeedback;
    public CharacterFeedback CharacterFeedback => _characterFeedback;
    
    [SerializeField] private CharacterFollowerHandler _characterFollowerHandler;
    public CharacterFollowerHandler CharacterFollowerHandler => _characterFollowerHandler;
    
    [SerializeField] private CharacterMovement _characterMovement;
    public CharacterMovement CharacterMovement => _characterMovement;
    
    [SerializeField] private CharacterWaiting _characterWaiting;
    public CharacterWaiting CharacterWaiting => _characterWaiting;
    

    void Start()
    {
        TextResizerManager.Instance?.UpdateTextRegistry();
    }

    private void OnDestroy()
    {
        TextResizerManager.Instance?.UpdateTextRegistry();
    }

    public void InitVar()
    {
        ServiceLocator.RequireComponent(this, ref _characterAnimationManager, "No Character Animation Found");
        ServiceLocator.RequireComponent(this, ref _characterAssets, "No Character Assets Found");
        ServiceLocator.RequireComponent(this, ref _characterDialogue, "No Character Dialogue Found");
        ServiceLocator.RequireComponent(this, ref _characterDisabilityHandler, "No Character Disability Handler Found");
        ServiceLocator.RequireComponent(this, ref _characterFeedback, "No Character Feedback Found");
        ServiceLocator.RequireComponent(this, ref _characterFollowerHandler, "No Character Follower Handler Found");
        ServiceLocator.RequireComponent(this, ref _characterMovement, "No Character Movement Found");
        ServiceLocator.RequireComponent(this, ref _characterWaiting, "No Character Waiting Found");
        
        ServiceLocator.RequireService(this, ref _characterSpawnManager, "No character spawn manager in scene");
        ServiceLocator.RequireService(this, ref _bubbleSpeechManager, "No Bubble speech manager in scene");
    }

    public void Init()
    {
        _characterMovement.Init(this);
        _characterFeedback.Init(this);
        _characterDisabilityHandler.Init(this);
        _characterAssets.Init(this);
        _characterFollowerHandler.Init(this);
        _characterDialogue.Init(this);
        _characterWaiting.Init(this);
        _characterAnimationManager.Init(this);
    }
    
    public void SetBubbleSpeech(bool isCustomerLeaving)
    {
        string speech = _character != null
            ? BubbleSpeechManager.GetSpeech(_characterFeedback.CustomerFeedback, isCustomerLeaving, _character.BubbleSpeech)
            : BubbleSpeechManager.GetSpeech(_characterFeedback.CustomerFeedback, isCustomerLeaving);
        if (speech == string.Empty)
        {
            Debug.LogWarning("No bubble speech found.");
            return;
        }
        _bubbleSpeechText.text = speech;
        _bubbleSpeechPanel.gameObject.SetActive(true);
    }

    public void SetCharacter(Character character)
    {
        _character = character;
    }

    
}

public enum CharacterState
{
    Idle,
    AtTheBar,
    AtTheBestTable
}

public enum CustomersFeedback
{
    Good,
    Average,
    Bad,
}


