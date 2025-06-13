using MyUtilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    
    [Header("Menu Elements")]
    [SerializeField] MainMenu _mainMenu;
    [SerializeField] SettingsMenu _settingsMenus;

    
    [SerializeField] private Animator BGAnimator;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            
            ServiceLocator.Register(this);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }
    void Start()
    {
        GoBackToMainMenu();
        RebindPauseInput();
    }
    
#region PauseResume
    public void PauseResume(InputAction.CallbackContext context)
    {
        if (!context.performed || SceneManager.GetActiveScene().name == "Main Menu") return;
        if (_mainMenu.gameObject.activeInHierarchy)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        _mainMenu?.gameObject.SetActive(true);
        SetFocus(_mainMenu?._resumeButton?.gameObject);
        
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        _mainMenu?.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

#endregion


    public void SetFocus(GameObject obj)
    {
        if (obj == null || EventSystem.current.currentSelectedGameObject == obj) return;
        EventSystem.current.SetSelectedGameObject(obj);
    }

    public void ShowSettings()
    {
        _settingsMenus?.gameObject.SetActive(true);
        _mainMenu?.gameObject.SetActive(false);
    }
    
    public void GoBackToMainMenu()
    {
        _settingsMenus?.gameObject.SetActive(false);
        _mainMenu?.gameObject.SetActive(true);
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            SetFocus(_mainMenu?._playButton?.gameObject);
        }
        else
        {
            SetFocus(_mainMenu?._resumeButton?.gameObject);
        }
    }

    public void HideMainMenu()
    {
        _mainMenu?.gameObject.SetActive(false);
    }

    public void ResetMenu()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            GoBackToMainMenu();
        }
        else
        {
            Resume();
            GoBackToMainMenu();
            HideMainMenu();
        }
    }

    public void RebindPauseInput()
    {
        PlayerInput playerInput = ServiceLocator.Get<InputValuesManager>().GetComponent<PlayerInput>();

        if (playerInput == null)
        {
            Debug.LogWarning("PlayerInput not found on selected GameObject.");
            return;
        }

        // Récupérer l'action "Pause"
        InputAction pauseAction = playerInput.actions["Pause"];

        if (pauseAction == null)
        {
            Debug.LogWarning("Pause action not found in PlayerInput.");
            return;
        }

        // Se désabonner d'abord pour éviter les doublons
        pauseAction.performed -= PauseResume;

        // S'abonner
        pauseAction.performed += PauseResume;
    }
}


