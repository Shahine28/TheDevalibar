using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(ConstraintBoolDictionary))]
public class ConstraintBoolDictionaryDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty keysProp = property.FindPropertyRelative("keys");
        SerializedProperty valuesProp = property.FindPropertyRelative("values");

        EditorGUI.BeginProperty(position, label, property);
        EditorGUILayout.BeginVertical("box");

        // Titre
        EditorGUILayout.LabelField("Contraintes à prendre en compte pour Dijkstra", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Les contraintes sont synchronisées avec le NodeManager. Activez/désactivez leur prise en compte dans le calcul du chemin.", MessageType.Info);

        // Parcours des contraintes
        for (int i = 0; i < keysProp.arraySize; i++)
        {
            SerializedProperty keyProp = keysProp.GetArrayElementAtIndex(i);
            SerializedProperty valueProp = valuesProp.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginHorizontal();

            // ✅ Affiche le nom de la contrainte suivi du toggle
            EditorGUILayout.LabelField($"{keyProp.stringValue} :", GUILayout.MaxWidth(200));
            valueProp.boolValue = EditorGUILayout.Toggle(valueProp.boolValue, GUILayout.MaxWidth(20));

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty keysProp = property.FindPropertyRelative("keys");
        return (keysProp.arraySize + 2) * (EditorGUIUtility.singleLineHeight + 6);
    }
}
#endif

public static class ConstraintSyncEvent
{
    public static event Action<List<Constraint>> OnConstraintSync;

    public static void Raise(List<Constraint> constraints)
    {
        OnConstraintSync?.Invoke(constraints);
    }
}

[Serializable]
public class ConstraintBoolDictionary
{
    public List<string> keys = new List<string>();
    public List<bool> values = new List<bool>();
    
    [NonSerialized]
    private bool isListening = false;

    public ConstraintBoolDictionary()
    {
        RegisterToGlobalSync();
    }

    // Appelé à l’instanciation ou manuellement dans OnEnable / Start
    public void RegisterToGlobalSync()
    {
        if (!isListening)
        {
            ConstraintSyncEvent.OnConstraintSync += SyncWithConstraints;
            isListening = true;
        }
    }

    public void UnregisterFromGlobalSync()
    {
        if (isListening)
        {
            ConstraintSyncEvent.OnConstraintSync -= SyncWithConstraints;
            isListening = false;
        }
    }
    public void SyncWithConstraints(List<Constraint> constraints)
    {
        // Synchronise le dictionnaire avec les contraintes
        foreach (var constraint in constraints)
        {
            if (!keys.Contains(constraint.name))
            {
                keys.Add(constraint.name);
                values.Add(false);  // Par défaut, les contraintes sont actives
            }
        }

        // Supprime les contraintes qui n'existent plus
        for (int i = keys.Count - 1; i >= 0; i--)
        {
            if (!constraints.Exists(c => c.name == keys[i]))
            {
                keys.RemoveAt(i);
                values.RemoveAt(i);
            }
        }
    }

    public bool ContainsKey(string key)
    {
        return keys.Contains(key);
    }
    public bool this[string key]
    {
        get
        {
            int index = keys.IndexOf(key);
            if (index != -1)
            {
                return values[index];
            }
            else
            {
                Debug.LogWarning($"Clé '{key}' introuvable dans ConstraintBoolDictionary.");
                return false;  // ✅ Valeur par défaut si non trouvée
            }
        }
        set
        {
            int index = keys.IndexOf(key);
            if (index != -1)
            {
                values[index] = value;
            }
            else
            {
                Debug.LogWarning($"Clé '{key}' introuvable. Ajout de la clé avec la valeur {value}.");
                keys.Add(key);
                values.Add(value);
            }
        }
    }
}



