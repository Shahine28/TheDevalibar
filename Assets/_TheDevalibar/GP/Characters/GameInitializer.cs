using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private Vector2Int defaultResolution = new Vector2Int(960, 540);
    [SerializeField] private Vector2Int defaultFullScreenResolution = new Vector2Int(1920, 1080);
    private bool _wasFullscreen;

    void Start()
    {
        _wasFullscreen = Screen.fullScreen;
    }

    void Update()
    {
        if (Screen.fullScreen && !_wasFullscreen)
        {
            Screen.SetResolution(defaultFullScreenResolution.x, defaultFullScreenResolution.y, true);
            _wasFullscreen = true;
        }
        else if (!Screen.fullScreen && _wasFullscreen)
        {
            Screen.SetResolution(defaultResolution.x, defaultResolution.y, false);
            _wasFullscreen = false;
        }
    }

}
