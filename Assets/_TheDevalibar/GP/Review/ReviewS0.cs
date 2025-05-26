using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "ReviewSo", menuName = "Scriptable Objects/ReviewSo")]
public class ReviewSO : ScriptableObject
{
    [SerializeField, Range(0,5)] private float _reviewRate;
    public float ReviewRate => _reviewRate;
    [SerializeField] private string _reviewTitle;
    public string ReviewTitle => _reviewTitle;
    [SerializeField, TextArea(3,10)] private string _reviewDescription;
    public string ReviewDescription => _reviewDescription;

    [SerializeField] private ConstraintBoolDictionary _reviewTopic;

    private void OnValidate()
    {
        _reviewRate = Mathf.Round(_reviewRate * 2f) / 2f;
    }

    public string GetReviewTopic()
    {
        return _reviewTopic.keys.FirstOrDefault(key => _reviewTopic[key])?.ToString() ?? string.Empty;
    }

}
