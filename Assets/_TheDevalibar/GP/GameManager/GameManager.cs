using System;
using UnityEngine;
using MyUtilities;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    [Header("Data")] 
    public GameData GameData;

    [Header("SceneLoader")] private AsyncOperation _sceneLoadingOperation;
    private string _nextSceneName;
    private bool _isSceneReady;
    private bool _allowSceneActivation;
    private bool _isNextSceneRequested;

    [SerializeField] private TextMeshProUGUI _goldText;
    [SerializeField] private bool _loadSceneDirectly;
    public event Action<string> OnSceneReady;
    public event Action<string> OnStartLoadingScene;
    public event Action<string> OnLoadingScene;


    [Header("Localization")] 
    private bool active;
    
    public event Action OnLanguageChanged;

    [Header("On New Day")]
    public UnityEvent OnNewDay;
    

    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persiste entre les scènes
        }
        else
        {
            Destroy(gameObject); // Évite les doublons
        }

        ServiceLocator.Register(this);
    }

    public void UpdateGoldValue()
    {
        UpdateGoldValue(GameData.Gold);
    }
    public void UpdateGoldValue(int gold)
    {
        _goldText.text = gold.ToString();
    }
    private void Start()
    {
        if (GameData && _goldText)
        {
            UpdateGoldValue(GameData.Gold);
        }
    }

    public void StartNewDay()
    {
        GameData.DayIndex++;
        OnNewDay?.Invoke();
    }
   
}



