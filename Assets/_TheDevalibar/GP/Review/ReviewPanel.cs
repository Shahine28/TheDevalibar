using MyUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ReviewPanel : MonoBehaviour
{
    [SerializeField] private Image _userPictureProfile;
    [SerializeField] private TextMeshProUGUI _userName;
    [SerializeField] private ReviewGrade _reviewGrade;
    [SerializeField] private TextMeshProUGUI _reviewTitle;
    [SerializeField] private TextMeshProUGUI _reviewDescription;
    private ReviewManager _reviewManager;
    void Start()
    {
        _reviewManager = ServiceLocator.Get<ReviewManager>();
    }

    public void SetReview(ReviewSO review, Character reviewCharacter = null)
    {
        _userPictureProfile.sprite = reviewCharacter != null
            ? reviewCharacter.CharacterProfilePicture
            : _reviewManager.GetRandomProfilePicture();
        _userName.text = reviewCharacter != null 
            ? reviewCharacter.CharacterPseudo 
            : _reviewManager.GetRandomUsername();
        _reviewGrade?.SetGrade(review.ReviewRate);
        _reviewTitle.text = review.ReviewTitle;
        _reviewDescription.text = review.ReviewDescription;
    }
}
