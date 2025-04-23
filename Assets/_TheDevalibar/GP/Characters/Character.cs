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
    
    [SerializedDictionary("LanguageCode", "Description")]
    public SerializedDictionary<CodeLanguage, DescriptionTextArea> DescriptionFromCodeLanguage;
    
    public int Affinity;

    [HideInInspector] public int NextAffinityValue = 75;

    [Header("First Dialogue")]
    public bool IsFirstDialogue = true;
    [SerializedDictionary("LanguageCode", "DialogueContainer")]
    public SerializedDictionary<CodeLanguage, DialogueContainer> FirstDialogueContainerFromLanguageCode;
    public Sprite FirstDialogueSprite;

    [Header("Sprites & Days Dialogues")]
    public List<CharacterSpriteFromAffinity> CharacterSprites;
    public List<Day> DaysDialogue;
    


    public Sprite GetCharacterSprite()
    {
        return null;
    }

    public DialogueContainer GetCharacterDialogue(int DayIndex, CodeLanguage language = CodeLanguage.English )
    {
        if (!AffinityManager)
        {
            Debug.LogError("Affinity Manager not found");
            return null;
        }

        if (DayIndex - 1 >= DaysDialogue.Count || DayIndex - 1 < 0) 
        {
            Debug.LogWarning($"DayIndex {DayIndex} is out of range!");
            return null;
        }

        foreach (Mood mood in AffinityManager.Moods)
        {
            if (IsInRange(Affinity, mood.Range))
            {
                DialogueAffinity dialogueAffinity = DaysDialogue[DayIndex - 1].Dialogues.FirstOrDefault(x => x.AffinityName == mood.Name);
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
        throw new NotImplementedException();
    }
}

public class CharacterSpriteFromAffinity
{
}

[Serializable]
public struct DescriptionTextArea
{
    [TextArea(1, 10)] public string Description;
}

