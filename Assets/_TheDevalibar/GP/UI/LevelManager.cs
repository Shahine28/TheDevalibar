using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    [SerializeField] SceneAsset _GameScene;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void LoadScene()
    {
        SceneManager.LoadScene(_GameScene.name);
    }
    
    public void LoadSceneAsync()
    {
        SceneManager.LoadSceneAsync(_GameScene.name);
    }
}
