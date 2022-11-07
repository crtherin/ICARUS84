using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer (typeof (ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
	public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
	{
		return EditorGUI.GetPropertyHeight (property, label, true);
	}

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		bool e = GUI.enabled;
		GUI.enabled = false;
		EditorGUI.PropertyField (position, property, label, true);
		GUI.enabled = e;
	}
}

[CustomPropertyDrawer (typeof (ReadOnlyPlayModeAttribute))]
public class ReadOnlyPlayModeDrawer : PropertyDrawer
{
	public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
	{
		return EditorGUI.GetPropertyHeight (property, label, true);
	}

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		bool e = GUI.enabled;
		if (Application.isPlaying)
			GUI.enabled = false;
		EditorGUI.PropertyField (position, property, label, true);
		GUI.enabled = e;
	}
}