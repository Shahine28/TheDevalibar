using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR


[CustomPropertyDrawer(typeof(EdgeDijkstra))]
public class EdgeDrawer : PropertyDrawer
{
    
    // Dictionnaire pour gérer l'état des Foldouts des Contraintes
    private static Dictionary<string, bool> constraintFoldouts = new Dictionary<string, bool>();
    
    // Dictionnaire pour gérer l'état des Foldouts des Edges accéssible depuis l'exterieur
    public static Dictionary<string, bool> edgeFoldouts = new Dictionary<string, bool>();
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        // Utilise le chemin de propriété pour des Foldouts uniques
        string propertyPath = property.propertyPath;

        if (!edgeFoldouts.ContainsKey(propertyPath))
            edgeFoldouts[propertyPath] = false;

        if (!constraintFoldouts.ContainsKey(propertyPath))
            constraintFoldouts[propertyPath] = false;

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = 2f;
        float yOffset = position.y;
        
        // Foldout principal de l'Edge
        edgeFoldouts[propertyPath] = EditorGUI.Foldout(
            new Rect(position.x, yOffset, position.width, lineHeight),
            edgeFoldouts[propertyPath],
            "Edge " + GetIndexFromPropertyPath(propertyPath) ,
            true);

        yOffset += spacing;

        if (edgeFoldouts[propertyPath])
        {
            yOffset += lineHeight + spacing;
            EditorGUI.indentLevel++;
            // Propriétés de base de l'Edge
            SerializedProperty startNodeNumberProperty = property.FindPropertyRelative("StartNodeNumber");
            SerializedProperty endNodeNumberProperty = property.FindPropertyRelative("EndNodeNumber");
            SerializedProperty costProperty = property.FindPropertyRelative("cost");
            SerializedProperty constraintsProperty = property.FindPropertyRelative("constraints");

            EditorGUI.PropertyField(new Rect(position.x, yOffset, position.width, lineHeight), startNodeNumberProperty, new GUIContent("Start Node Number"));
            yOffset += lineHeight + spacing;

            EditorGUI.PropertyField(new Rect(position.x, yOffset, position.width, lineHeight), endNodeNumberProperty, new GUIContent("End Node Number"));
            yOffset += lineHeight + spacing;
            
            EditorGUI.PropertyField(new Rect(position.x, yOffset, position.width, lineHeight), costProperty, new GUIContent("Cost"));
            yOffset += lineHeight + spacing;
            
            // Foldout pour les Contraintes
            constraintFoldouts[propertyPath] = EditorGUI.Foldout(
                new Rect(position.x, yOffset, position.width, lineHeight),
                constraintFoldouts[propertyPath],
                "Contraintes",
                true);

            yOffset += lineHeight + spacing;
            if (constraintFoldouts[propertyPath])
            {
                EditorGUI.indentLevel++;

                for (int i = 0; i < constraintsProperty.arraySize; i++)
                {
                    SerializedProperty constraintProp = constraintsProperty.GetArrayElementAtIndex(i);
                    SerializedProperty nameProp = constraintProp.FindPropertyRelative("name");
                    SerializedProperty typeProp = constraintProp.FindPropertyRelative("type");

                    ConstraintType type = (ConstraintType)typeProp.enumValueIndex;

                    // Affichage dynamique des contraintes
                    switch (type)
                    {
                        case ConstraintType.Int:
                            SerializedProperty intValueProp = constraintProp.FindPropertyRelative("intValue");
                            EditorGUI.PropertyField(new Rect(position.x, yOffset, position.width, lineHeight), intValueProp, new GUIContent(nameProp.stringValue));
                            yOffset += lineHeight + spacing;
                            break;

                        case ConstraintType.Float:
                            SerializedProperty floatValueProp = constraintProp.FindPropertyRelative("floatValue");
                            EditorGUI.PropertyField(new Rect(position.x, yOffset, position.width, lineHeight), floatValueProp, new GUIContent(nameProp.stringValue));
                            yOffset += lineHeight + spacing;
                            break;

                        case ConstraintType.Bool:
                            SerializedProperty boolValueProp = constraintProp.FindPropertyRelative("boolValue");
                            EditorGUI.PropertyField(new Rect(position.x, yOffset, position.width, lineHeight), boolValueProp, new GUIContent(nameProp.stringValue));
                            yOffset += lineHeight + spacing;
                            break;

                        case ConstraintType.Vector3:
                            SerializedProperty vector3ValueProp = constraintProp.FindPropertyRelative("vector3Value");
                            EditorGUI.PropertyField(new Rect(position.x, yOffset, position.width, lineHeight), vector3ValueProp, new GUIContent(nameProp.stringValue));
                            yOffset += lineHeight + spacing;
                            break;
                    }
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }
    
    private int GetIndexFromPropertyPath(string propertyPath)
    {
        string[] pathParts = propertyPath.Split('[');
        if (pathParts.Length > 1)
        {
            string indexPart = pathParts[1].Split(']')[0];
            if (int.TryParse(indexPart, out int index))
            {
                return index;
            }
        }
        return -1;  // Retourne -1 si aucun index n'est trouvé
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight + 3f;
        float totalHeight = lineHeight;

        string propertyPath = property.propertyPath;

        SerializedProperty constraintsProperty = property.FindPropertyRelative("constraints");

        // Si Edge est déplié, ajouter la hauteur de ses propriétés
        if (edgeFoldouts.ContainsKey(propertyPath) && edgeFoldouts[propertyPath])
        {
            totalHeight += 4 * lineHeight;  // StartNode, EndNode, Constraint

            // Si Contraintes est déplié, ajouter la hauteur des contraintes
            if (constraintFoldouts.ContainsKey(propertyPath) && constraintFoldouts[propertyPath])
            {
                totalHeight += (constraintsProperty.arraySize) * lineHeight;
            }
        }

        return totalHeight;
    }
}
#endif

[System.Serializable]
public class EdgeDijkstra
{
    public string label = "";
    public int StartNodeNumber =-1;
    public int EndNodeNumber =-1;
    public float cost = 1f; // Contrainte par défaut non retirable (nécessaire pour que dijkstra puisse fonctionner)
    
    [Header("Constraints")]
    public List<Constraint> constraints = new List<Constraint>();

}


