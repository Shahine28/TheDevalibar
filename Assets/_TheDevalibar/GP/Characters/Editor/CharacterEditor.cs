#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Character))]
public class CharacterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Dessine les propriétés par défaut

        Character character = (Character)target;

        // Vérifie si AffinityManager est assigné
        if (!character.AffinityManager)
        {
            EditorGUILayout.HelpBox("Please assign an AffinityManager to display available affinities.", MessageType.Warning);
            return;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Affinities :", EditorStyles.boldLabel);

        // Récupère la liste des affinités avec leurs plages depuis AffinityManager
        var moods = character.AffinityManager.Moods;

        if (moods == null || moods.Count == 0)
        {
            EditorGUILayout.HelpBox("No affinities found in the AffinityManager.", MessageType.Info);
            return;
        }

        // Affiche les affinités avec leur plage sous forme de liste non modifiable
        foreach (var mood in moods)
        {
            EditorGUILayout.LabelField($"- {mood.Name} [{mood.Range.x}, {mood.Range.y}]");
        }
    }
}
#endif