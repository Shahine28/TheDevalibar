using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    [Header("Data")] public GameData gameData;

    [Header("SceneLoader")] private AsyncOperation _sceneLoadingOperation;
    private string _nextSceneName;
    private bool _isSceneReady;
    private bool _allowSceneActivation;
    private bool _isNextSceneRequested;
    [SerializeField] private bool _loadSceneDirectly;
    public event Action<string> OnSceneReady;
    public event Action<string> OnStartLoadingScene;
    public event Action<string> OnLoadingScene;


    [Header("Localization")] private bool active;

    public bool isConnectedToGPS;
    public event Action OnLanguageChanged;

    
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
   
}



