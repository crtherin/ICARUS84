using System;
using System.Linq;
using System.Reflection;

namespace Procedures.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System.Collections.Generic;

	[CustomPropertyDrawer (typeof (ProcessSelectorAttribute))]
	public class ProcessSelectorDrawer : PropertyDrawer
	{
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType == SerializedPropertyType.String)
			{
				EditorGUI.BeginProperty (position, label, property);

				string[] abilitiesList = typeof (Behaviour).FindAllTypeNames ();
				string propertyString = property.stringValue;

				int index = -1;

				for (int i = 0; i < abilitiesList.Length; i++)
				{
					if (abilitiesList[i] == propertyString)
					{
						index = i;
						break;
					}
				}

				index = EditorGUI.Popup (position, label.text, index, abilitiesList.ToArray ());

				if (index >= 0)
					property.stringValue = abilitiesList[index];

				EditorGUI.EndProperty ();
			}
			else
			{
				EditorGUI.PropertyField (position, property, label);
			}
		}
	}
}