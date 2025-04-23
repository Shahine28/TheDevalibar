using UnityEngine;
using UnityEditor.Experimental.GraphView;
public class DialogueNode : Node
{
    public string GUID;

    public string DialogueText;

    public bool EntryPoint = false; 
    
    public int StatModifier = 0;
    
    public Sprite CharacterMoodSprite;
}
