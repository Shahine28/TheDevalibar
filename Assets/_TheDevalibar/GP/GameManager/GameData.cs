using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "GameData", menuName = "Game/GameData")]
public class GameData : ScriptableObject
{
    public int Gold;
    [HideInInspector] public int NextGoldValue;
    public int Customers;
    [HideInInspector] public int NextCustomersValue;

    public int DayIndex = 1;

    public bool GameHasStarted;
    public CodeLanguage LanguageCode = CodeLanguage.English;
    public bool IsIntroDone;
    

    // Stockage des valeurs initiales
    private int initialGold;
    private int initialNextGoldValue;
    private int initialCustomers;
    private int initialNextCustomersValue;

    private int initialDayIndex;

    private bool initialGameHasStarted;
    private bool initialIsIntroDone;

#if UNITY_EDITOR  
    private void OnEnable()
    {
        // SaveInitialValues();
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode) // Quand on quitte le mode Play
        {
            ResetToInitialValues();
        }
    }

#endif
    public void SaveInitialValues()
    {
        Debug.Log("Saving Initial Values for GameData");
        initialGold = Gold;
        initialNextGoldValue = NextGoldValue;
        initialCustomers = Customers;
        initialNextCustomersValue = NextCustomersValue;

        initialDayIndex = DayIndex;

        initialGameHasStarted = GameHasStarted;
        initialIsIntroDone = IsIntroDone;
    }

    public void ResetToInitialValues()
    {
        Debug.Log("Resetting Initial Values for GameData");
        Gold = initialGold;
        NextGoldValue = initialNextGoldValue;
        Customers = initialCustomers;
        NextCustomersValue = initialNextCustomersValue;

        DayIndex = initialDayIndex;

        GameHasStarted = initialGameHasStarted;
        IsIntroDone = initialIsIntroDone;

        // Marque l'objet comme modifié pour que Unity détecte les changements
        #if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        #endif
    }

}
