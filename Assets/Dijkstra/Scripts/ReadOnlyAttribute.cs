using UnityEngine;
using UnityEditor;

public class ReadOnlyAttribute : PropertyAttribute { }
#if UNITY_EDITOR 

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;  // ✅ Rend le champ non modifiable
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;   // ✅ Réactive les autres champs
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
#endif

    

