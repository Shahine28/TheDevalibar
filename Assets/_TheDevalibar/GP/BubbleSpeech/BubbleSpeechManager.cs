using System;
using System.Collections.Generic;
using System.Linq;
using MyUtilities;
using UnityEngine;
using Random = UnityEngine.Random;

public class BubbleSpeechManager : MonoBehaviour
{
    [SerializeField] private bool _isBubbleSpeechUnique = false;
    private List<string> _usedNPCBubbleSpeechList = new List<string>();
    private List<string> _usedCharacterBubbleSpeechList = new List<string>();
    
    [Header("NPC Bubble Speech")]
    [SerializeField] private CustomersBubbleSpeech _bubbleSpeech;
    public void Awake()
    {
        if (!GetComponent<CharacterBehavior>()) ServiceLocator.Register(this);
    }

    public void ResetUsedBubbleSpeechList()
    {
        _usedNPCBubbleSpeechList.Clear();
    }

    private string GetRandomNPCSpeech(List<String> list)
    {
        List<string> nonUsedBubbleSpeechList = list;
        if (_isBubbleSpeechUnique)
        {
            nonUsedBubbleSpeechList = list.Where(x=> !_usedNPCBubbleSpeechList.Contains(x)).ToList();
        }
        if (nonUsedBubbleSpeechList.Count == 0) return string.Empty;
        if (nonUsedBubbleSpeechList.Count == 1) return nonUsedBubbleSpeechList[0];
        string result = nonUsedBubbleSpeechList[Random.Range(0, nonUsedBubbleSpeechList.Count)];
        _usedNPCBubbleSpeechList.Add(result);
        return result; ;
    }
    
    private string GetRandomCharacterSpeech(List<String> list)
    {
        List<string> nonUsedBubbleSpeechList = list;
        if (_isBubbleSpeechUnique)
        {
            nonUsedBubbleSpeechList = list.Where(x=> !_usedCharacterBubbleSpeechList.Contains(x)).ToList();
        }
        if (nonUsedBubbleSpeechList.Count == 0) return string.Empty;
        if (nonUsedBubbleSpeechList.Count == 1) return nonUsedBubbleSpeechList[0];
        string result = nonUsedBubbleSpeechList[Random.Range(0, nonUsedBubbleSpeechList.Count)];
        _usedCharacterBubbleSpeechList.Add(result);
        return result; ;
    }

    public string GetSpeech(CustomersFeedback feedback, bool isCustomerLeaving)
    {
        return feedback switch
        {
            CustomersFeedback.Good => GetRandomNPCSpeech(isCustomerLeaving 
                ? _bubbleSpeech.GoodLeavingSpeech 
                : _bubbleSpeech.GoodTableSpeech),
            
            CustomersFeedback.Average => GetRandomNPCSpeech(isCustomerLeaving ?
                _bubbleSpeech.AverageLeavingSpeech :
                _bubbleSpeech.AverageTableSpeech),
            
            CustomersFeedback.Bad => GetRandomNPCSpeech(_bubbleSpeech.BadLeavingSpeech),
            _ => string.Empty
        };
    }
    
    public string GetSpeech(CustomersFeedback feedback, bool isCustomerLeaving, CustomersBubbleSpeech bubbleSpeech)
    {
        return feedback switch
        {
            CustomersFeedback.Good => GetRandomCharacterSpeech(isCustomerLeaving 
                ? bubbleSpeech.GoodLeavingSpeech 
                : bubbleSpeech.GoodTableSpeech),
            
            CustomersFeedback.Average => GetRandomCharacterSpeech(isCustomerLeaving ?
                bubbleSpeech.AverageLeavingSpeech :
                bubbleSpeech.AverageTableSpeech),
            
            CustomersFeedback.Bad => GetRandomCharacterSpeech(bubbleSpeech.BadLeavingSpeech),
            _ => string.Empty
        };
    }
}

[Serializable]
public struct CustomersBubbleSpeech
{
    [Header("While At Table")] 
    [SerializeField] private List<String> _goodTableSpeech;
    public List<String> GoodTableSpeech => _goodTableSpeech;
    
    [SerializeField] private List<String> _averageTableSpeech;
    public List<String> AverageTableSpeech => _averageTableSpeech;
    // Il n'y a pas de mauvais car si c'est mauvais, on ne s'assoit pas
    
    [Header("While leaving")]
    [SerializeField] private List<String> _goodLeavingSpeech;
    public List<String> GoodLeavingSpeech => _goodLeavingSpeech;
    
    [SerializeField] private List<String> _averageLeavingSpeech;
    public List<String> AverageLeavingSpeech => _averageLeavingSpeech;
    
    [SerializeField] private List<String> _badLeavingSpeech;
    public List<String> BadLeavingSpeech => _badLeavingSpeech;
}