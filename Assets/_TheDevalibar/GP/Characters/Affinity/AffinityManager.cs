using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;

[System.Serializable]
public class Mood
{
    public string Name;
    public Vector2 Range;
}

[CreateAssetMenu(fileName = "AffinityManager", menuName = "Scriptable Objects/Affinity Manager")]
public class AffinityManager : ScriptableObject
{
    [FormerlySerializedAs("moods")]
    [Header("Moods")]
    [HideInInspector] public List<Mood> Moods = new List<Mood>();
    

    /// Ajoute un nouveau mood.
    public void AddMood(string name, Vector2 range)
    {
        // Vérifie si le mood existe déjà
        if (Moods.Exists(m => m.Name == name))
        {
            Debug.LogWarning($"Mood '{name}' already exists.");
            return;
        }

        // Ajuste la plage pour éviter les chevauchements
        foreach (var existingMood in Moods)
        {
            // Vérifie si la plage se chevauche avec un mood existant
            if (range.x <= existingMood.Range.y && range.y >= existingMood.Range.x)
            {
                Debug.LogWarning(
                    $"Mood '{name}' range ({range.x}, {range.y}) overlaps with mood '{existingMood.Name}' range ({existingMood.Range.x}, {existingMood.Range.y}). Adjusting range."
                );
                
                if (range.x <= existingMood.Range.y)
                {
                    range.x = existingMood.Range.y + 1;
                }


                if (range.y >= existingMood.Range.x)
                {
                    range.y = Mathf.Max(range.x, range.y);
                }
            }
        }

        // Vérifie que la plage ajustée est valide
        if (range.x >= range.y)
        {
            Debug.LogError($"Invalid range for mood '{name}'. Range: ({range.x}, {range.y})");
            return;
        }

        // Ajoute le mood avec la plage ajustée
        Moods.Add(new Mood
        {
            Name = name,
            Range = range
        });
        
        Debug.Log($"Mood '{name}' added with range ({range.x}, {range.y}).");
        UpdateOrder();
    }

    private void UpdateOrder()
    {
        Moods.Sort((m1, m2) => m1.Range.magnitude.CompareTo(m2.Range.magnitude));
    }
    
    
    /// Supprime un mood par son nom.
    public void RemoveMood(string name)
    {
        Mood moodToRemove = Moods.Find(m => m.Name == name);
        if (moodToRemove != null)
        {
            Moods.Remove(moodToRemove);
        }
        else
        {
            Debug.LogWarning($"Mood '{name}' not found.");
        }
    }

    public Mood GetMood(int affinity)
    {
        if (Moods == null || Moods.Count == 0)
        {
            Debug.LogWarning("No moods available.");
            return null;
        }

        foreach (Mood mood in Moods)
        {
            if (affinity >= mood.Range.x && affinity <= mood.Range.y)
            {
                return mood;
            }
        }

        Debug.LogWarning($"No matching mood found for affinity {affinity}.");
        return null; 
    }

    
    public Vector2 GetRange()
    {
        if (Moods.Count == 0)
        {
            Debug.LogWarning("No moods available, returning default range (0,0).");
            return Vector2.zero;
        }

        float minValue = Mathf.Min(Moods.ConvertAll(m => m.Range.x).ToArray());
        float maxValue = Mathf.Max(Moods.ConvertAll(m => m.Range.y).ToArray());

        return new Vector2(minValue, maxValue);
    }
}

