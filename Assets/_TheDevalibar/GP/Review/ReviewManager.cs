using System;
using System.Collections.Generic;
using MyUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ReviewManager : MonoBehaviour
{
    
    [SerializeField] private List<String> _randomUsernameBank = new List<string>();
    [SerializeField] private List<Sprite> _randomPictureProfileBank = new List<Sprite>();
    
    [Header("Random NPC reviews")]
    [SerializeField] private List<ReviewSO> _randomGoodReviewSO = new List<ReviewSO>();
    [SerializeField] private List<ReviewSO> _randomAverageReviewSO = new List<ReviewSO>();
    [SerializeField] private List<ReviewSO> _randomBadReviewSO = new List<ReviewSO>();
    
    [Header("Panel Prefab & Container")]
    [SerializeField] private GameObject _reviewPanel;
    [SerializeField] private Transform _reviewPanelContainer;
    private List<CharacterReview> _characterReviews = new List<CharacterReview>();

    
    [Header("Close/Open Panel")]
    [SerializeField] private Button _reviewCloseButton;
    [SerializeField] private TextMeshProUGUI _reviewCloseButtonText;
    [SerializeField] private MoveUI _moveUI;
    private bool _isOpen;
    
    
    void Awake()
    {
        ServiceLocator.Register(this);
    }

    void Start()
    {
        _reviewCloseButton?.onClick.AddListener(() => CloseOpenPanel(!_isOpen));
    }
    
    public void CloseOpenPanel(bool ClosePanel)
    {
        _moveUI?.LaunchMoveUI(ClosePanel && _isOpen);
        _isOpen = ClosePanel;
        _reviewCloseButtonText.text = ClosePanel ? "<" : "X";
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

    public ReviewSO GetRandomReview(List<ReviewSO> ReviewSOList)
    {
        if (ReviewSOList == null || ReviewSOList.Count == 0)
        {
            Debug.LogWarning("Review SO bank is empty!");
            return null;
        }

        int index = Random.Range(0, ReviewSOList.Count);
        return ReviewSOList[index];
    }

    public void AddReview(CustomersFeedback feedback, Character character = null)
    {
        CharacterReview characterReview = new CharacterReview(character, feedback);
        _characterReviews.Add(characterReview);
    }

    public void SetReviews()
    {
        for (int i = _reviewPanelContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(_reviewPanelContainer.GetChild(i).gameObject);
        }
       
        if (_characterReviews == null || _characterReviews.Count == 0)
        {
            Debug.LogWarning("Character reviews is null or empty!");
            return;
        }
        List<ReviewSO> AlreadyUsedReviewSOList = new List<ReviewSO>();
        foreach (CharacterReview characterReview in _characterReviews)
        {
            ReviewPanel reviewPanel = Instantiate(_reviewPanel, _reviewPanelContainer).GetComponent<ReviewPanel>();
            if (_reviewPanel != null)
            {
                ReviewSO reviewSO = null;
                switch (characterReview.Feedback)
                {
                    case CustomersFeedback.Good:
                    {
                        if (characterReview.Character == null)
                        {
                            reviewSO = GetRandomReview(_randomGoodReviewSO);
                            int count = 0; 
                            while (AlreadyUsedReviewSOList.Contains(reviewSO) && count < 10)
                            {
                                reviewSO = GetRandomReview(_randomGoodReviewSO);
                                count++;
                            }
                            AlreadyUsedReviewSOList.Add(reviewSO);
                        }
                        else
                        {
                            reviewSO = GetRandomReview(characterReview.Character.CharacterReviews.GoodReviews); 
                        }
                        break;
                    }
                    case CustomersFeedback.Average:
                    {
                        if (characterReview.Character == null)
                        {
                            reviewSO = GetRandomReview(_randomAverageReviewSO);
                            int count = 0; 
                            while (AlreadyUsedReviewSOList.Contains(reviewSO) && count < 10)
                            {
                                reviewSO = GetRandomReview(_randomAverageReviewSO);
                                count++;
                            }
                            AlreadyUsedReviewSOList.Add(reviewSO);
                        }
                        else
                        {
                            reviewSO = GetRandomReview(characterReview.Character.CharacterReviews.AverageReviews); 
                        }
                        break;
                    }
                    case CustomersFeedback.Bad:
                    {
                        if (characterReview.Character == null)
                        {
                            reviewSO = GetRandomReview(_randomBadReviewSO);
                            int count = 0; 
                            while (AlreadyUsedReviewSOList.Contains(reviewSO) && count < 10)
                            {
                                reviewSO = GetRandomReview(_randomBadReviewSO);
                                count++;
                            }
                            AlreadyUsedReviewSOList.Add(reviewSO);
                        }
                        else
                        {
                            reviewSO = GetRandomReview(characterReview.Character.CharacterReviews.BadReviews); 
                        }
                        break;
                    }
                }

                if (reviewSO == null)
                {
                    Debug.LogWarning("Review SO bank is empty!");
                    continue;
                }
                reviewPanel.Initialize(this);
                reviewPanel.SetReview(reviewSO, characterReview.Character);
            }
        }
    }

    private void ClearReviews()
    {
        _characterReviews.Clear();
        for (int i = _reviewPanelContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(_reviewPanelContainer.GetChild(i).gameObject);
        }
    }

}

public struct CharacterReview
{
    public Character Character;
    public CustomersFeedback Feedback;

    public CharacterReview(Character character, CustomersFeedback feedback)
    {
        Character = character;
        Feedback = feedback;
    }
}
