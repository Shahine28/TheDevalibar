using System;
using System.Collections;
using System.Collections.Generic;
using MyUtilities;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CharacterSpawnManager : MonoBehaviour
{
    [SerializeField] private List<DayShift> _dayShifts;
    public int _dayIndex;
    private int _nextCharacterIndex;
    private int _NPCCount = 0;
    [Header("Character & NPC GameObjects")]
    [SerializeField] private GameObject _characterGameObject;

    public bool CanSpawnCharacter;
    [SerializeField] private GameObject _NPCGameObject;
    private bool _haveAllCharactersAndNCPBeenSpawned;
    public bool HaveAllCharactersAndNCPBeenSpawned => _haveAllCharactersAndNCPBeenSpawned;
    [SerializeField] private Transform _characterSpawnPoint;
    public Transform CharacterSpawnPoint => _characterSpawnPoint;

    [Header("Delay between spawning")]
    [SerializeField] private float _characterSpawnDelayInSeconds;
    private Coroutine _delayCoroutine;
    private bool _isPaused;
    
    public event Action OnSpawnFinished;
    [SerializeField] private GameManager _gameManager;
    
    

    void Awake()
    {
        ServiceLocator.Register(this);
        CanSpawnCharacter = true;
    }
    void Start()
    {
        DialogueManager dialogueManager = ServiceLocator.Get<DialogueManager>();
        if (dialogueManager) // Le spawning est en pause lors des phases de visual novel
        {
            dialogueManager._onDialogueStart += PauseSpawning;
            dialogueManager._onDialogueEnd += PlaySpawning;
        }

        if (!_gameManager)
        {
            _gameManager = ServiceLocator.Get<GameManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartSpawning()
    {
        if (_delayCoroutine == null)
        {
            _isPaused = false;
            _dayIndex = _gameManager.GameData.DayIndex;
            _haveAllCharactersAndNCPBeenSpawned = false;
            _delayCoroutine = StartCoroutine(SpawningCharacterAndNPC());
        }
    }

    private IEnumerator SpawningCharacterAndNPC()
    {
        DayShift currentShift = _dayShifts[_dayIndex];
        int customerCount = currentShift.DailyCustomers.Count;
        int npcTargetCount = currentShift.NPCToSpawnThisDay;

        while (_nextCharacterIndex < customerCount || _NPCCount < npcTargetCount)
        {
            while (_isPaused)
                yield return null;

            bool canSpawnCharacter = _nextCharacterIndex < customerCount && CanSpawnCharacter;
            bool canSpawnNPC = _NPCCount < npcTargetCount;

            // Choisir aléatoirement entre character et NPC
            bool spawnCharacter = false;

            if (canSpawnCharacter && canSpawnNPC)
            {
                spawnCharacter = Random.value < 0.5f; // 50/50
            }
            else if (canSpawnCharacter)
            {
                spawnCharacter = true;
            }
            else if (canSpawnNPC)
            {
                spawnCharacter = false;
            }

            if (spawnCharacter)
                SpawnCharacter();
            else if (canSpawnNPC)
                SpawnNPC();

            yield return new WaitForSeconds(_characterSpawnDelayInSeconds);
        }

        _delayCoroutine = null; // Permet de redémarrer plus tard si besoin
        _haveAllCharactersAndNCPBeenSpawned = true;
        Debug.Log("All characters and NPCs spawned for this day.");
    }


    private void PauseSpawning()
    {
        if (_delayCoroutine != null)
        {
            _isPaused = true;
        }
    }
    
    private void PlaySpawning()
    {
        if (_delayCoroutine != null)
        {
            _isPaused = false;
        }
    }

    private void StopSpawning()
    {
        if (_delayCoroutine != null)
        {
            StopCoroutine(_delayCoroutine);
            _delayCoroutine = null;
        }
    }
    
    private void SpawnCharacter()
    {
        if (_characterGameObject == null || _characterSpawnPoint == null)
        {
            Debug.LogWarning("Character prefab or spawn point is not assigned.");
            return;
        }

        GameObject characterInstance = Instantiate(_characterGameObject, _characterSpawnPoint.position, _characterSpawnPoint.rotation,  _characterSpawnPoint);
        CharacterBehavior characterBehavior = characterInstance.GetComponent<CharacterBehavior>();
        if (characterBehavior != null)
        {
            characterBehavior.Initialize(_dayShifts[_dayIndex].DailyCustomers[_nextCharacterIndex]);
            _nextCharacterIndex++;
        }
        Debug.Log("Character spawned.");
    }

    private void SpawnNPC()
    {
        if (_NPCGameObject == null || _characterSpawnPoint == null)
        {
            Debug.LogWarning("NPC prefab or spawn point is not assigned.");
            return;
        }

        Instantiate(_NPCGameObject, _characterSpawnPoint.position, _characterSpawnPoint.rotation,  _characterSpawnPoint);
        _NPCCount++;
        Debug.Log("NPC spawned.");
    }
}

[System.Serializable]
public struct DayShift
{
    public List<Character> DailyCustomers;
    public int NPCToSpawnThisDay;
}
