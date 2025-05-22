using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEngine.Serialization;
using MyUtilities;

public class DialogueManager : MonoBehaviour
{
    [FormerlySerializedAs("character")]
    [Header("Character")]
    [SerializeField, ReadOnly] private Character _character;
    [SerializeField, ReadOnly] private CharacterBehavior _characterBehavior;

    [SerializeField] private int _dialogueIndex;
    [SerializeField] private Image _character1Sprite;
    [SerializeField] private Image _barmaidSprite;
    
    // [Header("File")]
    // [SerializeField] private string filePath = "Assets/_NarrativeProject/Prog/Scripts/DialogueGraph/Resources";
    // [SerializeField] private string fileName = "DialogueGraph";
    
    [Header("Dialogue Canvas")]
    [SerializeField] private TextMeshProUGUI _characterNameText;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private List<TextMeshProUGUI> _dialogueButtonTexts = new List<TextMeshProUGUI>();
    [SerializeField] private List<Button> _buttons = new List<Button>();
    [SerializeField] private Button _continueButton;
    [SerializeField] private GameObject _continueButtonContainer;
    [SerializeField] private GameObject _buttonsContainer;
    [SerializeField] private Slider _affinitySlider;
    
    private Dictionary<Button, string> _NodeGUIDFromButton= new Dictionary<Button, string>();
    [SerializeField] private bool _randomizeButtonsOrder = true;

    private DialogueContainer _containerCache;
    private List<DialogueNodeData> _nodesPathHistory = new List<DialogueNodeData>();
    

    [Header("Text Machine Effect")]
    [SerializeField] private Button _skipTextMachineEffectButton;
    [SerializeField] private float typingSpeed = 0.05f;
    private string _fullText;
    private Coroutine _typingCoroutine;
    private bool _isTyping = false;
    
    [Header("Background & Foreground")]
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Image _foregroundImage;
    

    private GameManager _gameManager;
    
    [Header("Dialogue Canvas")]
    [SerializeField] private Canvas _dialogueCanvas;

    
    public event Action _onDialogueStart;
    public event Action _onDialogueEnd;
    

    void Awake()
    {
        ServiceLocator.Register(this);
    }
    void Start()
    {
        
        _gameManager = ServiceLocator.Get<GameManager>();
        if (_gameManager != null)
        {

        }
        else
        {
            Debug.LogError("Game manager not found!");
        }

        
    }

    public void SwitchCharacterFocus(Image CharaterToFocus, Image CharacterToUnfocus)
    {
        CharaterToFocus.color = Color.white;
        CharacterToUnfocus.color = Color.grey;
    }
    

    private void OnEnable()
    {
        // if (_gameManager) _gameManager.OnLanguageChanged += OnLanguageChanged;
        foreach (Button button in _buttons)
        {
            button.onClick.AddListener(() => OnChoiceButtonClicked(button));
        }
        _continueButton.onClick.AddListener(() => OnChoiceButtonClicked(_continueButton));
        _skipTextMachineEffectButton.onClick.AddListener(SkipTypingEffect);
    }

    private void OnDisable()
    {
        if (_gameManager) _gameManager.OnLanguageChanged -= OnLanguageChanged;
        foreach (Button button in _buttons)
        {
            button.onClick.RemoveListener(() => OnChoiceButtonClicked(button));
        }
        _continueButton.onClick.RemoveListener(() => OnChoiceButtonClicked(_continueButton));
        _skipTextMachineEffectButton.onClick.RemoveListener(SkipTypingEffect);
    }

    public void InitCharacterDialogue(CharacterBehavior characterBehavior, bool isEventDialogue,
        CodeLanguage currentLanguage)
    {
        if (!characterBehavior) return;
        _characterBehavior = characterBehavior;
        _character = _characterBehavior.Character;
        InitDialogue(isEventDialogue, currentLanguage);
    }

    private void InitDialogue(bool isEventDialogue, CodeLanguage currentLanguage)
    {
        if (_character == null)
        {
            Debug.LogError("[DialogueManager] Character is null!");
            return;
        }
        if (!_dialogueCanvas.gameObject.activeSelf) _dialogueCanvas.gameObject.SetActive(true);
        
        
        _containerCache = _character.GetCharacterDialogue(currentLanguage);
        if (_containerCache == null)
        {
            Debug.LogError($"[DialogueManager] No character dialogue found for day {_dialogueIndex} in {currentLanguage}!");
            return;
        }
        
        

        if (_containerCache == null)
        {
            Debug.LogError("[DialogueManager] Dialogue Graph Not Found!");
            return;
        }

        Debug.Log($"[DialogueManager] Dialogue Graph Loaded for language {currentLanguage}");
        List<NodeLinkData> nodeLinks = _containerCache.NodeLinks;

        _nodesPathHistory.Clear(); // Réinitialise l'historique des nodes du dialogue

        StartDialogue(nodeLinks);
    }
    

    private void StartDialogue(List<NodeLinkData> nodeLinks)
    {
        DialogueNodeData firstDialogueNode = GetDialogueNodeData(nodeLinks.Find(x => x.PortName == "StartPointNode").TargetNodeGuid);
        if (firstDialogueNode == null)
        {
            Debug.LogError("First Dialogue Node Not Found");
            return;
        }
        _onDialogueStart?.Invoke();
        if (!_characterNameText) Debug.LogWarning("_characterNameText is null!");
        else _characterNameText.text = _character.CharacterName;
        if (_affinitySlider)
        {
            Vector2 moodRange = _character.AffinityManager.GetRange();
            _affinitySlider.minValue = moodRange.x;
            _affinitySlider.maxValue = moodRange.y;
            _affinitySlider.value = _character.Affinity;
        }
        SetDialogueCanvas(firstDialogueNode);
    }

    private DialogueNodeData GetDialogueNodeData(string NodeGuid)
    {
        return _containerCache.DialogueNodeData.Find(x => x.NodeGUID == NodeGuid);
    }
    
    
    public void OnLanguageChanged()
    {
        if (_character == null)
        {
            Debug.LogError("[DialogueManager] Character is null!");
            return;
        }

        if (_nodesPathHistory.Count == 0)
        {
            Debug.LogError("[DialogueManager] No dialogue node history found!");
            return;
        }
        
        
        CodeLanguage languageCode = _gameManager.GameData.LanguageCode;
        
        
        _containerCache = _character.GetCharacterDialogue(languageCode);
        
        
        if (!_containerCache)
        {
            Debug.LogError($"[DialogueManager] No dialogue container found for language {languageCode}");
            return;
        }
        
        DialogueNodeData lastNode = _nodesPathHistory.Last();
        DialogueNodeData newNode = _containerCache.DialogueNodeData.Find(node => node.NodeGUID == lastNode.NodeGUID);

        if (newNode == null)
        {
            Debug.LogError($"[DialogueManager] No equivalent dialogue node found in {languageCode} for node {lastNode.NodeGUID}");
            return;
        }

        Debug.Log($"[DialogueManager] Language switched to {languageCode}, displaying node: {newNode.DialogueText}");
    
        // 🔹 Recharge le dialogue sans changer l’historique
        SetDialogueCanvas(newNode, false);
    }

    private void SetDialogueCanvas(DialogueNodeData dialogueNodeData, bool typingEffect = true)
    {
        _buttonsContainer?.gameObject.SetActive(false);
        _continueButtonContainer?.gameObject.SetActive(false);
        SwitchCharacterFocus(_character1Sprite, _barmaidSprite);
        if (_typingCoroutine != null && _isTyping)
        {
            StopCoroutine(_typingCoroutine);
            _isTyping = false;
        }
        if (typingEffect)
        {
            _typingCoroutine = StartCoroutine(TypeText(dialogueNodeData.DialogueText));
        }
        else
        {
            _dialogueText.text = dialogueNodeData.DialogueText;
            _buttonsContainer?.gameObject.SetActive(true);
            _continueButtonContainer?.gameObject.SetActive(true);
            SwitchCharacterFocus(_barmaidSprite, _character1Sprite);
        }
        
        List<NodeLinkData> Ports = _containerCache.NodeLinks.Where(links => links.BaseNodeGuid == dialogueNodeData.NodeGUID).ToList();
        _nodesPathHistory.Add(dialogueNodeData);
        SetupDialogueButtons(Ports);
        
        
        _character.NextAffinityValue += dialogueNodeData.StatModifier;
        _character.Affinity += dialogueNodeData.StatModifier;
        _affinitySlider.value += dialogueNodeData.StatModifier;
        Sprite NewSprite = dialogueNodeData.CharacterMoodSprite;
        if (NewSprite != null && _character1Sprite.sprite != NewSprite)
        {
            _character1Sprite.sprite = NewSprite;
        }
    }
    
    
    private IEnumerator TypeText(string text)
    {
        _isTyping = true;
        _fullText = text;
        _dialogueText.text = ""; // Vide le texte avant d'écrire

        foreach (char letter in text)
        {
            _dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        _isTyping = false;
        _buttonsContainer?.gameObject.SetActive(true);
        _continueButtonContainer?.gameObject.SetActive(true);
        if (_buttons.Any(x=>x.gameObject.activeInHierarchy)) SwitchCharacterFocus(_barmaidSprite, _character1Sprite);
    }

    public void SkipTypingEffect()
    {
        if (_isTyping)
        {
            StopCoroutine(_typingCoroutine);
            _dialogueText.text = _fullText;
            _isTyping = false;
            _buttonsContainer?.gameObject.SetActive(true);
            _continueButtonContainer?.gameObject.SetActive(true);
            if (_buttons.Any(x=>x.gameObject.activeInHierarchy)) SwitchCharacterFocus(_barmaidSprite, _character1Sprite);
        }
    }

    private void SetupDialogueButtons(List<NodeLinkData> NodeLinks)
    {
        DialogueNodeData lastDialogueNode = _nodesPathHistory.Last(); // Evite d'appeler à chaque fois Last()
        int ListCount = NodeLinks.Count > 0 
            ? NodeLinks.Count 
            : lastDialogueNode.Ports.Count;
        
        // Active uniquement les boutons nécessaires en fonction du nombre de NodeLinks
        _buttons.ForEach(button => { button.gameObject.SetActive(_buttons.IndexOf(button) < ListCount && !lastDialogueNode.Ports[0].IsPortNull); });
        _continueButton.gameObject.SetActive(NodeLinks.Count == 1 && NodeLinks[0].IsNullNode);
        
        _NodeGUIDFromButton.Clear();

        if (!_continueButton.gameObject.activeSelf)
        {
            if (_randomizeButtonsOrder && ListCount > 1)
            {
                // Crée une liste d'indices de 0 à ListCount et la mélange
                List<int> randomizedIndices = Enumerable.Range(0, ListCount).OrderBy(_ => Random.value).ToList();

                // Assigne les ports aux boutons en suivant l'ordre randomisé
                for (int i = 0; i < ListCount; i++)
                {
                    string PortName = NodeLinks.Count > 0
                        ? NodeLinks[randomizedIndices[i]].PortName
                        : lastDialogueNode.Ports[i].PortName;
                    _dialogueButtonTexts[i].text = PortName;
                
                    string NodeGuid = NodeLinks.Count > 0
                        ? NodeLinks[randomizedIndices[i]].TargetNodeGuid 
                        : lastDialogueNode.NodeGUID;
                    _NodeGUIDFromButton.Add(_buttons[i], NodeGuid);
                }
            }
            else
            {
                string PortName = NodeLinks.Count > 0
                    ? NodeLinks[0].PortName
                    : lastDialogueNode.Ports[0].PortName;
                
                _dialogueButtonTexts[0].text = PortName;
            
                string NodeGuid = NodeLinks.Count > 0 
                    ? NodeLinks[0].TargetNodeGuid 
                    : lastDialogueNode.NodeGUID;
                
                _NodeGUIDFromButton.Add(_buttons[0], NodeGuid);
            }
        }
        else
        {
            _NodeGUIDFromButton.Add(_continueButton, NodeLinks[0].TargetNodeGuid);
        }
        
        if (NodeLinks.Count == 0) Debug.Log("No node next");
    }


    public void OnChoiceButtonClicked(Button clickedButton)
    {
        if (!_containerCache.NodeLinks.Any(x => x.BaseNodeGuid == _nodesPathHistory.Last().NodeGUID)) // Si jamais ce node n'est lié à aucun prochain node alors c'est le node finale.
        {
            Vector2 AffinityRange = _character.AffinityManager.GetRange();
            _character.NextAffinityValue = (int)Mathf.Clamp(_character.NextAffinityValue, AffinityRange.x, AffinityRange.y); // On clamp la proichaine affinité entre la valeur de mood min et la valeur de mood max
            Debug.Log($"Dialogue ended with '{_character.NextAffinityValue}' of affinity");
            EndDialogue();
            //_gameManager.GoToPhase(GamePhase.Hub1);
            return;
        } 
        
        if ((!_buttons.Contains(clickedButton) && clickedButton != _continueButton) || !_NodeGUIDFromButton.ContainsKey(clickedButton))
        {
            Debug.LogError("Button not found or NodeGUID not found");
            EndDialogue();
            return;
        }
        DialogueNodeData nextDialogueNode = GetDialogueNodeData(_NodeGUIDFromButton[clickedButton]);
        if (nextDialogueNode == null)
        {
            Debug.LogError("Dialogue Node Not Found");
            EndDialogue();
            return;
        }
        
        SetDialogueCanvas(nextDialogueNode);
    }

    public void EndDialogue()
    {
        _character.DialogueIndex++;
        _dialogueCanvas.gameObject.SetActive(false);
        _characterBehavior.OnDialogueEnd();
        _onDialogueEnd?.Invoke();
    }
}
