using Data;
using UnityEditor;
using UnityEngine;

namespace Procedures.Editor
{
	[CustomPropertyDrawer (typeof (VariationTree))]
	public class VariationTreeDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{
			return base.GetPropertyHeight (property, label) - 10;
		}

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			object variationTreeObject = fieldInfo.GetValue (property.GetParent ());
			VariationTree tree = variationTreeObject as VariationTree;

			SerializedProperty variationsListProperty = property.FindPropertyRelative ("variations");
			int variationsCount = variationsListProperty.FindPropertyRelative ("Array.size").intValue;

			SerializedProperty selectedProperty = property.FindPropertyRelative ("selected");
			int selectedVariation = selectedProperty.intValue;

			bool enabled = GUI.enabled;
			EditorGUILayout.BeginHorizontal (EditorStyles.textArea);

			GUI.enabled = enabled && variationsCount > 0 && selectedVariation >= 0;
			if (GUILayout.Button ("◀", EditorStyles.miniLabel))
			{
				selectedProperty.intValue--;
				EditorUtility.SetDirty (property.serializedObject.targetObject);
			}

			GUI.enabled = enabled && selectedVariation >= 0;
			if (GUILayout.Button ("-", EditorStyles.miniLabel))
			{
				tree.RemoveSelectedVariation ();
				EditorUtility.SetDirty (property.serializedObject.targetObject);
			}

			GUILayout.FlexibleSpace ();

			GUILayout.Label (
				(variationsCount > 0
					? "(" + (selectedVariation + 1) + "/" + variationsCount + ") "
					: "") +
				(selectedVariation < 0
					? "Default"
					: "Variation " + (selectedVariation + 1).ToUpperChar ()));

			GUILayout.FlexibleSpace ();

			GUI.enabled = enabled && selectedVariation + 1 >= variationsCount;
			if (GUILayout.Button ("+", EditorStyles.miniLabel))
			{
				tree.AddVariation ();
				EditorUtility.SetDirty (property.serializedObject.targetObject);
			}

			GUI.enabled = enabled && variationsCount > 0 && selectedVariation < variationsCount - 1;
			if (GUILayout.Button ("▶", EditorStyles.miniLabel))
			{
				selectedProperty.intValue++;
				EditorUtility.SetDirty (property.serializedObject.targetObject);
			}

			EditorGUILayout.EndHorizontal ();

			if (selectedVariation >= 0)
			{
				SerializedProperty variationProperty = variationsListProperty.GetArrayElementAtIndex (selectedVariation);
				SerializedProperty upgradesListProperty = variationProperty.FindPropertyRelative ("upgrades");
				int upgradesCount = upgradesListProperty.FindPropertyRelative ("Array.size").intValue;

				SerializedProperty upgradeProperty = variationProperty.FindPropertyRelative ("upgrade");
				int selectedUpgrade = upgradeProperty.intValue;

				EditorGUILayout.BeginHorizontal (EditorStyles.textArea);

				GUI.enabled = enabled && upgradesCount > 0 && selectedUpgrade > 0;
				if (GUILayout.Button ("◀", EditorStyles.miniLabel))
				{
					upgradeProperty.intValue--;
					EditorUtility.SetDirty (property.serializedObject.targetObject);
				}

				GUI.enabled = enabled && selectedUpgrade > 0;
				if (GUILayout.Button ("-", EditorStyles.miniLabel))
				{
					tree.RemoveSelectedUpgradeFromSelectedVariation ();
					EditorUtility.SetDirty (property.serializedObject.targetObject);
				}

				GUILayout.FlexibleSpace ();

				GUILayout.Label (
					(upgradesCount > 0
						? "(" + selectedUpgrade + "/" + upgradesCount + ") "
						: "") +
					(selectedUpgrade < 1
						? "No Upgrade"
						: "Upgrade " + selectedUpgrade));

				GUILayout.FlexibleSpace ();

				GUI.enabled = enabled && selectedUpgrade >= upgradesCount;
				if (GUILayout.Button ("+", EditorStyles.miniLabel))
				{
					tree.AddUpgradeToSelectedVariation ();
					EditorUtility.SetDirty (property.serializedObject.targetObject);
				}

				GUI.enabled = enabled && upgradesCount > 0 && selectedUpgrade < upgradesCount;
				if (GUILayout.Button ("▶", EditorStyles.miniLabel))
				{
					upgradeProperty.intValue++;
					EditorUtility.SetDirty (property.serializedObject.targetObject);
				}

				EditorGUILayout.EndHorizontal ();
			}

			GUI.enabled = enabled;
		}
	}
}