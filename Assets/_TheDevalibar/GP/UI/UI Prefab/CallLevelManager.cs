using MyUtilities;
using UnityEngine;


public class CallLevelManager : MonoBehaviour
{
    public void LoadGameScene()
    {
        LevelManager.instance.LoadGameScene();
    }

    public void LoadMenuScene()
    {
        LevelManager.instance.LoadMainMenuScene();
    }
}
