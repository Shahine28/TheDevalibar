using _TheDevalibar.GP.Characters;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CharacterFeedback : CharacterComponent
{
    [Header("Character Feedback")]
    [SerializeField, ReadOnly] private CustomersFeedback _customerFeedback = CustomersFeedback.Good;
    public CustomersFeedback CustomerFeedback => _customerFeedback;
    
    [SerializeField] private CharacterFeedbackSO characterFeedbackSo;
    [SerializeField] private RawImage _feedBackImage;
    public RawImage FeedBackImage => _feedBackImage;

    
    public override void Init(CharacterBehavior characterBehavior)
    {
        base.Init(characterBehavior);
        FeedBackImage.gameObject.SetActive(false);
        
        UpdateFeedBackImage();

    }
    
    
    public void SetCustomerFeedback(Table table)
    {
        if (table == null)
        {
            _customerFeedback = CustomersFeedback.Bad;
            UpdateFeedBackImage();
            return;
        }

        // Aucun handicap → satisfait
        if (string.IsNullOrEmpty(_characterDisabilityHandler.CharacterDisability))
        {
            _customerFeedback = CustomersFeedback.Good;
            return;
        }

        // Handicap présent → vérifie si table adaptée
        bool isTableAdapted = table.constraintDict.ContainsKey(_characterDisabilityHandler.CharacterDisability) && table.constraintDict[_characterDisabilityHandler.CharacterDisability];

        _customerFeedback = isTableAdapted ? CustomersFeedback.Good : CustomersFeedback.Average;
        UpdateFeedBackImage();
    }

    private void UpdateFeedBackImage()
    {
        if (!FeedBackImage || !characterFeedbackSo)
        {
            Debug.LogError("No feedback image found.");
            return;
        }

        Color color = Color.white;
        Texture texture = null;

        switch (_customerFeedback)
        {
            case CustomersFeedback.Good:
                color = characterFeedbackSo.GoodFeedbackColor;
                texture = characterFeedbackSo.GoodFeedbackSprite;
                break;

            case CustomersFeedback.Average:
                color = characterFeedbackSo.AverageFeedbackColor;
                texture = characterFeedbackSo.AverageFeedbackSprite;
                break;

            case CustomersFeedback.Bad:
                color = characterFeedbackSo.BadFeedbackColor;
                texture = characterFeedbackSo.BadFeedbackSprite;
                break;
        }

        if (characterFeedbackSo.useColorFeedback)
        {
            FeedBackImage.color = color;
        }
        else
        {
            FeedBackImage.texture = texture;
        }
    }

    public int GetTipValue()
    {
        switch (_customerFeedback)
        {
            case CustomersFeedback.Good:
                return characterFeedbackSo.GoodTipsValue;
            case CustomersFeedback.Average:
                return characterFeedbackSo.AverageTipsValue;
            case CustomersFeedback.Bad:
                return characterFeedbackSo.BadTipsValue;
            default:
                break;
        }

        return 0;
    }
}
