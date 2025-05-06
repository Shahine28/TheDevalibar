using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "GameData", menuName = "Game/GameData")]
public class GameData : ScriptableObject
{
    
    public int DayIndex = 1;
    public CodeLanguage LanguageCode = CodeLanguage.English;
    public int Gold = 750;

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
            
        }
    }

#endif


}
