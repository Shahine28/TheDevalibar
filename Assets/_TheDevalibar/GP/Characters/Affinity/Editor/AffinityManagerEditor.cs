#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AffinityManager))]
public class AffinityManagerEditor : Editor
{
    private string newMoodName = "";
    private Vector2 newMoodRange = Vector2.zero;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AffinityManager affinityManager = (AffinityManager)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Mood Manager", EditorStyles.boldLabel);

        // Champs pour ajouter un nouveau mood
        newMoodName = EditorGUILayout.TextField("Mood Name", newMoodName);
        newMoodRange = EditorGUILayout.Vector2Field("Mood Range", newMoodRange);

        if (GUILayout.Button("Add Mood"))
        {
            if (!string.IsNullOrEmpty(newMoodName))
            {
                affinityManager.AddMood(newMoodName, newMoodRange);
                newMoodName = ""; // Réinitialise le champ
                newMoodRange = Vector2.zero;
                EditorUtility.SetDirty(affinityManager); // Marque l'objet comme modifié
            }
            else
            {
                Debug.LogWarning("Mood name cannot be empty.");
            }
        }

        EditorGUILayout.Space();

        // Affiche la liste des moods
        EditorGUILayout.LabelField("Existing Moods (Read-Only)", EditorStyles.boldLabel);
        foreach (Mood mood in affinityManager.Moods)
        {
            EditorGUILayout.BeginHorizontal();

            // Nom du mood en lecture seule
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LabelField(mood.Name, GUILayout.Width(100));
            EditorGUILayout.Vector2Field("", mood.Range); // Range en lecture seule
            EditorGUI.EndDisabledGroup();

            // Bouton pour supprimer le mood
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                affinityManager.RemoveMood(mood.Name);
                EditorUtility.SetDirty(affinityManager); // Marque l'objet comme modifié
                break;
            }

            EditorGUILayout.EndHorizontal();
        }

        // Enregistre les modifications si l'utilisateur modifie les valeurs des moods
        if (GUI.changed)
        {
            EditorUtility.SetDirty(affinityManager);
        }
    }
}
#endif
