using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer (typeof (ShowIfPlayModeAttribute))]
public class ShowIfPlayModeDrawer : PropertyDrawer
{
	public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
	{
		return Application.isPlaying ? EditorGUI.GetPropertyHeight (property, label, true) : 0;
	}

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		if (Application.isPlaying)
			EditorGUI.PropertyField (position, property, label, true);
	}
}