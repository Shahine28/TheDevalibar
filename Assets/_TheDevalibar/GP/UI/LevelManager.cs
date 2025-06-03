using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] SceneAsset _GameScene;
    //[SerializeField] SceneManager _GameScene;


    public void Play()
    {
        SceneManager.LoadScene(_GameScene.name);
    }

    public void QuitToMain()
    {
        SceneManager.LoadScene(_GameScene.name);
    }
}
