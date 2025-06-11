using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] SceneAsset _GameScene;
    
    public void LoadScene()
    {
        SceneManager.LoadScene(_GameScene.name);
    }
    
    public void LoadSceneAsync()
    {
        SceneManager.LoadSceneAsync(_GameScene.name);
    }
}
