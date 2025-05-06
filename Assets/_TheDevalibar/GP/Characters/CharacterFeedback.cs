using UnityEngine;

namespace _TheDevalibar.GP.Characters
{
    [CreateAssetMenu(fileName = "CharacterFeedback", menuName = "Scriptable Objects/CharacterFeedback", order = 0)]
    public class CharacterFeedback : ScriptableObject
    {
        public bool useColorFeedback;
        
        public Color GoodFeedbackColor;
        public Color AverageFeedbackColor;
        public Color BadFeedbackColor;
        
        public Texture GoodFeedbackSprite;
        public Texture AverageFeedbackSprite;
        public Texture BadFeedbackSprite;

        public int GoodTipsValue = 20;
        public int AverageTipsValue = 10;
        public int BadTipsValue = 0;
    }
}