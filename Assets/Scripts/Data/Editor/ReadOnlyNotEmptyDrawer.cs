using UnityEditor;
using UnityEngine;

namespace Data.Editor
{
	[CustomPropertyDrawer (typeof (ReadOnlyNotEmptyAttribute))]
	public class ReadOnlyNotEmptyDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight (property, label, true);
		}

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			if (!string.IsNullOrEmpty (property.stringValue))
			{
				GUI.enabled = false;
				EditorGUI.PropertyField (position, property, label, true);
			}

			GUI.enabled = true;
		}
	}
}