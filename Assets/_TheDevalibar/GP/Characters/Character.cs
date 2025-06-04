using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;

#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu(fileName = "Character", menuName = "Scriptable Objects/Character"), System.Serializable]
public class Character : ScriptableObject
{
    public AffinityManager AffinityManager;

    [Header("Character Information")]
    public string CharacterName;
    public string CharacterPseudo;
    public Sprite CharacterProfilePicture;
    public Sprite CharacterSprite;
    public Mesh CharacterMesh;
    public Material CharacterMaterial;
    public bool HasSpecificRuntimeAnimationController;
    [ShowIf("HasSpecificRuntimeAnimationController")] public RuntimeAnimatorController CharacterRuntimeAnimatorController;
    
    
    [Header("Character Follower")] 
    [SerializeField] private bool _hasAFollower;
    [SerializeField, ShowIf("_hasAFollower")] private CharacterFollower _characterFollower;
    public CharacterFollower CharacterFollower => _characterFollower;
    
    public int Affinity;
    [HideInInspector] public int NextAffinityValue = 75;
    
    [Header("Dialogues")]
    public List<Day> Dialogues;
    public int DialogueIndex = 0;

    [Header("Character Reviews")] 
    [SerializeField] private CharacterReviews _characterReviews;
    public CharacterReviews CharacterReviews => _characterReviews;
    
    [Header("Character Bubble Speech")]
    [SerializeField] private CustomersBubbleSpeech _bubbleSpeech;
    public CustomersBubbleSpeech BubbleSpeech => _bubbleSpeech;
    
    public ConstraintBoolDictionary constraintDict = new ConstraintBoolDictionary();
    
#if UNITY_EDITOR  
    private void OnEnable()
    {
        // SaveInitialValues();
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode) // Quand on quitte le mode Play
        {
            DialogueIndex = 0;
            Affinity = 50;
        }
    }

#endif
    
    public Sprite GetCharacterSprite()
    {
        return null;
    }

    public DialogueContainer GetCharacterDialogue(CodeLanguage language = CodeLanguage.English )
    {
        if (!AffinityManager)
        {
            Debug.LogError("Affinity Manager not found");
            return null;
        }

        if (DialogueIndex >= Dialogues.Count || DialogueIndex < 0) 
        {
            Debug.LogWarning($"DayIndex {DialogueIndex} is out of range!");
            return null;
        }

        foreach (Mood mood in AffinityManager.Moods)
        {
            if (IsInRange(Affinity, mood.Range))
            {
                DialogueAffinity dialogueAffinity = Dialogues[DialogueIndex].Dialogues.FirstOrDefault(x => x.AffinityName == mood.Name);
                if (dialogueAffinity.DialogueContainerFromLanguageCode.ContainsKey(language))
                {
                    DialogueContainer dialogueContainer = dialogueAffinity.DialogueContainerFromLanguageCode[language];
                    return dialogueContainer; 
                }
            }
        }
        return null;
    }

    private bool IsInRange(int affinity, Vector2 moodRange)
    {
        return affinity >= moodRange.x && affinity <= moodRange.y;
    }
    
    

}

[Serializable]
public struct DescriptionTextArea
{
    [TextArea(1, 10)] public string Description;
}

[Serializable]
public struct CharacterReviews
{
    public List<ReviewSO> GoodReviews;
    public List<ReviewSO> AverageReviews;
    public List<ReviewSO> BadReviews;
}
