using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;

using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Character", menuName = "Scriptable Objects/Character"), System.Serializable]
public class Character : ScriptableObject
{
    public AffinityManager AffinityManager;

    [Header("Character Information")]
    public string CharacterName;
    public string CharacterPseudo;
    public Sprite CharacterProfilePicture;
    public Sprite CharacterSprite;
    
    public int Affinity;

    [HideInInspector] public int NextAffinityValue = 75;

    [Header("First Dialogue")]
    public bool IsFirstDialogue = true;
    [SerializedDictionary("LanguageCode", "DialogueContainer")]
    public SerializedDictionary<CodeLanguage, DialogueContainer> FirstDialogueContainerFromLanguageCode;
    public Sprite FirstDialogueSprite;

    [FormerlySerializedAs("DaysDialogue")] [Header("Days Dialogues")]
    public List<Day> Dialogues;
    
    
    public ConstraintBoolDictionary constraintDict = new ConstraintBoolDictionary();
    
    public Sprite GetCharacterSprite()
    {
        return null;
    }

    public DialogueContainer GetCharacterDialogue(int DialogueIndex, CodeLanguage language = CodeLanguage.English )
    {
        if (!AffinityManager)
        {
            Debug.LogError("Affinity Manager not found");
            return null;
        }

        if (DialogueIndex - 1 >= Dialogues.Count || DialogueIndex - 1 < 0) 
        {
            Debug.LogWarning($"DayIndex {DialogueIndex} is out of range!");
            return null;
        }

        foreach (Mood mood in AffinityManager.Moods)
        {
            if (IsInRange(Affinity, mood.Range))
            {
                DialogueAffinity dialogueAffinity = Dialogues[DialogueIndex - 1].Dialogues.FirstOrDefault(x => x.AffinityName == mood.Name);
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

