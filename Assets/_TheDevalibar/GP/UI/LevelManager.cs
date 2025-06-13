using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    [SerializeField] string _mainMenuScene = "Main Menu";
    [SerializeField] string _gameScene = "Shahine";

    
    public UnityEvent<Scene> OnSceneLoaded;
    public UnityEvent<Scene> OnSceneChanged;

    public List<GameObject> GameObjectsSpecificToMainMenuScene = new List<GameObject>();
    public List<GameObject> GameObjectsSpecificToGameScene  = new List<GameObject>();
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
            
            SceneManager.sceneLoaded += OnSceneLoadedCallback;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }
        else
        {
            Destroy(gameObject);
        }
        CheckForGameObjectToShowOrHide();
    }

    private void OnDestroy()
    {
        // Important → on se désabonne pour éviter les leaks
        SceneManager.sceneLoaded -= OnSceneLoadedCallback;
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene(_mainMenuScene);
    }
    public void LoadGameScene()
    {
        SceneManager.LoadScene(_gameScene);
    }
    
    private void OnSceneLoadedCallback(Scene scene, LoadSceneMode mode)
    {
        OnSceneLoaded?.Invoke(scene);
    }
    
    private void OnActiveSceneChanged(Scene previousScene, Scene currentScene)
    {
        CheckForGameObjectToShowOrHide(currentScene);
        OnSceneChanged?.Invoke(currentScene);
    }

    public void CheckForGameObjectToShowOrHide()
    {
        if (SceneManager.GetActiveScene().name == _mainMenuScene)
        {
            GameObjectsSpecificToMainMenuScene.ForEach(obj => obj.SetActive(true));
            GameObjectsSpecificToGameScene.ForEach(obj => obj.SetActive(false));
        }
        else if (SceneManager.GetActiveScene().name == _gameScene)
        {
            GameObjectsSpecificToMainMenuScene.ForEach(obj => obj.SetActive(false));
            GameObjectsSpecificToGameScene.ForEach(obj => obj.SetActive(true));
        }
    }
    
    public void CheckForGameObjectToShowOrHide(Scene scene)
    {
        if (scene.name == _mainMenuScene)
        {
            GameObjectsSpecificToMainMenuScene.ForEach(obj => obj.SetActive(true));
            GameObjectsSpecificToGameScene.ForEach(obj => obj.SetActive(false));
        }
        else if (scene.name == _gameScene)
        {
            GameObjectsSpecificToMainMenuScene.ForEach(obj => obj.SetActive(false));
            GameObjectsSpecificToGameScene.ForEach(obj => obj.SetActive(true));
        }
    }
    
    
}
