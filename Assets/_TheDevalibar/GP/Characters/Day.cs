using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[System.Serializable]
public class Day
{
    public List<DialogueAffinity> Dialogues;
}

[System.Serializable]
public struct DialogueAffinity
{
    public string AffinityName;
    [SerializedDictionary("LanguageCode", "DialogueContainer")]
    public SerializedDictionary<CodeLanguage, DialogueContainer> DialogueContainerFromLanguageCode;
}

[System.Serializable]
public struct DialogueLanguage
{
    [SerializedDictionary("LanguageCode", "DialogueContainer")]
    public SerializedDictionary<CodeLanguage, DialogueContainer> DialogueContainerFromLanguageCode;
}


[System.Serializable]
public enum CodeLanguage
{
    [InspectorName("en")] English,  // English
    [InspectorName("fr")] French,  // French (Français)
    // [InspectorName("es")] Spanish,  // Spanish (Español)
    // [InspectorName("de")] German,  // German (Deutsch)
    // [InspectorName("it")] Italian, // Italian (Italiano) 
    // [InspectorName("pt")] Portuguese, // Portuguese (Português) 
    // [InspectorName("nl")] Dutch, // Dutch (Nederlands)
    // [InspectorName("ru")] Russian, // Russian (Русский) 
    // [InspectorName("zh-Hans")] Chinese,  // Chinese (中文 - Mandarin)
    // [InspectorName("ja")] Japanese,  // Japanese (日本語)
    // [InspectorName("ko")] Korean,  // Korean (한국어)
    // [InspectorName("ar")] Arabic  // Arabic (العربية)
}
