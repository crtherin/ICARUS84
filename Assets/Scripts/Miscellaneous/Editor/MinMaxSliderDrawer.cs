using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer (typeof(MinMaxSliderAttribute))]
class MinMaxSliderDrawer : PropertyDrawer
{
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		if (property.propertyType != SerializedPropertyType.Vector2)
		{
			EditorGUI.LabelField (position, label, "Use MinMaxSlider only with Vector2");
			return;
		}

		Vector2 range = property.vector2Value;
		float min = range.x;
		float max = range.y;

		MinMaxSliderAttribute attr = attribute as MinMaxSliderAttribute;

		EditorGUI.BeginChangeCheck ();
		EditorGUI.MinMaxSlider (position, label, ref min, ref max, attr.Min, attr.Max);
		if (EditorGUI.EndChangeCheck ())
		{
			range.x = min;
			range.y = max;
			property.vector2Value = range;
		}
	}
}