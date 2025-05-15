using System;
using System.Collections.Generic;
using MyUtilities;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using Random = UnityEngine.Random;

public class ReviewManager : MonoBehaviour
{
    [SerializeField] private List<String> _randomUsernameBank = new List<string>();
    [SerializeField] private List<Sprite> _randomPictureProfileBank = new List<Sprite>();

    void Awake()
    {
        ServiceLocator.Register(this);
    }
    
    public string GetRandomUsername()
    {
        if (_randomUsernameBank == null || _randomUsernameBank.Count == 0)
        {
            Debug.LogWarning("Username bank is empty!");
            return "User_" + Random.Range(1000, 9999); // fallback
        }

        int index = Random.Range(0, _randomUsernameBank.Count);
        return _randomUsernameBank[index];
    }

    public Sprite GetRandomProfilePicture()
    {
        if (_randomPictureProfileBank == null || _randomPictureProfileBank.Count == 0)
        {
            Debug.LogWarning("Profile picture bank is empty!");
            return null;
        }

        int index = Random.Range(0, _randomPictureProfileBank.Count);
        return _randomPictureProfileBank[index];
    }
}
