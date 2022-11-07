using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(StringID))]
public class StringIDPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        EditorGUI.LabelField(position, new GUIContent(property.FindPropertyRelative("value").stringValue),
            GUIContent.none);

        EditorGUI.EndProperty();
    }
}