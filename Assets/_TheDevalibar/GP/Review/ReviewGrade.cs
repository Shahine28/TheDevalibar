using NaughtyAttributes;
using UnityEngine;

public class ReviewGrade : MonoBehaviour
{
    [SerializeField] private Transform _reviewGroup;
    [SerializeField] private GameObject _fullStar;
    [SerializeField] private GameObject _halfStar;
    [SerializeField] private GameObject _emptyStar;

    
    public void SetGrade(float grade)
    {
        // Arrondir à la demi-étoile la plus proche
        float newGrade = Mathf.Round(grade * 2f) / 2f;

        
        foreach (Transform child in _reviewGroup)
        {
            Destroy(child.gameObject); 
        }
        
        for (int i = 0; i < 5; i++)
        {
            GameObject starToInstantiate;

            if (newGrade >= i + 1)
            {
                starToInstantiate = _fullStar;
            }
            else if (newGrade >= i + 0.5f)
            {
                starToInstantiate = _halfStar;
            }
            else
            {
                starToInstantiate = _emptyStar;
            }
            Instantiate(starToInstantiate, _reviewGroup);
        }
    }
}

