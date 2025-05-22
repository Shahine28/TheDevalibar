using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public class Constraint
{
    public string name; // Nom de la contrainte
    public ConstraintType type; // Type sélectionnable
    public bool IsBlockingConstraint;
    public bool CanTakeStairs;
    
    public bool boolValue = true;
    public int intValue;
    public float floatValue;
    public Vector3 vector3Value;

    // Retourne la valeur selon le type sélectionné
    public object GetValue()
    {
        return type switch
        {
            ConstraintType.Bool => boolValue,
            ConstraintType.Int => intValue,
            ConstraintType.Float => floatValue,
            ConstraintType.Vector3 => vector3Value,
            _ => null
        };
    }

    // ✅ Accesseurs sécurisés selon le type
    public void SetValue(object value)
    {
        switch (type)
        {
            case ConstraintType.Bool:
                boolValue = Convert.ToBoolean(value);
                break;
            case ConstraintType.Int:
                intValue = Convert.ToInt32(value);
                break;
            case ConstraintType.Float:
                floatValue = Convert.ToSingle(value);
                break;
            case ConstraintType.Vector3:
                vector3Value = (Vector3)value;
                break;
        }
    }
}

[Serializable]
public enum ConstraintType
{
    Int,
    Bool,
    Float,
    Vector3
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Constraint))]
public class ConstraintDrawer : PropertyDrawer
{
    // Gère les états de déploiement des contraintes individuellement
    private static Dictionary<string, bool> constraintFoldouts = new Dictionary<string, bool>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Utilise le chemin unique de la propriété pour gérer chaque foldout individuellement
        string propertyPath = property.propertyPath;

        if (!constraintFoldouts.ContainsKey(propertyPath))
            constraintFoldouts[propertyPath] = false;

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = 3f;
        float yOffset = position.y;

        // Foldout principal pour chaque contrainte
        constraintFoldouts[propertyPath] = EditorGUI.Foldout(
            new Rect(position.x, yOffset, position.width, lineHeight),
            constraintFoldouts[propertyPath],
            property.FindPropertyRelative("name").stringValue != "" 
                ? property.FindPropertyRelative("name").stringValue 
                : label.text,
            true);

        yOffset += lineHeight + spacing;

        if (constraintFoldouts[propertyPath])
        {
            EditorGUI.indentLevel++;

            // ✅ Champs de la contrainte
            SerializedProperty nameProperty = property.FindPropertyRelative("name");
            SerializedProperty typeProperty = property.FindPropertyRelative("type");
            SerializedProperty boolProperty = property.FindPropertyRelative("IsBlockingConstraint");
            SerializedProperty bool2Property = property.FindPropertyRelative("CanTakeStairs");
            
            EditorGUI.PropertyField(
                new Rect(position.x, yOffset, position.width, lineHeight),
                nameProperty,
                new GUIContent("Nom")
            );
            yOffset += lineHeight + spacing;

            EditorGUI.PropertyField(
                new Rect(position.x, yOffset, position.width, lineHeight),
                typeProperty,
                new GUIContent("Type")
            );
            yOffset += lineHeight + spacing;
            
            EditorGUI.PropertyField(
                new Rect(position.x, yOffset, position.width, lineHeight),
                boolProperty,
                new GUIContent("Is Blocking Constraint")
            );
            
            yOffset += lineHeight + spacing;
            
            EditorGUI.PropertyField(
                new Rect(position.x, yOffset, position.width, lineHeight),
                bool2Property,
                new GUIContent("Can Take Stairs")
            );
            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight + 4f;
        float totalHeight = lineHeight;

        string propertyPath = property.propertyPath;

        // ✅ Si la contrainte est dépliée, ajoute de la hauteur
        if (constraintFoldouts.ContainsKey(propertyPath) && constraintFoldouts[propertyPath])
        {
            totalHeight += 4 * lineHeight; // Nom + Type
        }

        return totalHeight;
    }
}
#endif

