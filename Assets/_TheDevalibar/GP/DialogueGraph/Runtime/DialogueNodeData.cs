using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class DialogueNodeData
{
    [FormerlySerializedAs("Guid")] public string NodeGUID;
    public string DialogueText;
    public Vector2 Position;
    public int StatModifier;
    
    [SerializeField] public List<SerializablePort> Ports = new List<SerializablePort>();
    public bool TriggersEvent;
    public Sprite CharacterMoodSprite;
}