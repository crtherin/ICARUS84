using UnityEditor;
using UnityEngine;

namespace Data
{
	[CustomEditor (typeof (Container))]
	public class ContainerEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();
			DrawNode (serializedObject.FindProperty ("root"));
			serializedObject.ApplyModifiedProperties ();
		}

		private void DrawNode (SerializedProperty node)
		{
			SerializedProperty fields = node.FindPropertyRelative ("dataTable");
			DrawFieldLists (fields);

			SerializedProperty nodes = node.FindPropertyRelative ("children");
			for (int i = 0; i < nodes.arraySize; i++)
			{
				SerializedProperty n = nodes.GetArrayElementAtIndex (i);

				EditorGUILayout.BeginVertical (EditorStyles.textArea);

				GUILayout.Label (n.FindPropertyRelative ("name").stringValue, EditorStyles.boldLabel);
				DrawNode (nodes.GetArrayElementAtIndex (i));

				EditorGUILayout.EndVertical ();
			}
		}

		private void DrawFieldLists (SerializedProperty dataFields)
		{
			DrawList (dataFields.FindPropertyRelative ("intFields"));
			DrawList (dataFields.FindPropertyRelative ("floatFields"));
			DrawList (dataFields.FindPropertyRelative ("boolFields"));
			DrawList (dataFields.FindPropertyRelative ("stringFields"));
			DrawList (dataFields.FindPropertyRelative ("vector2Fields"));
			DrawList (dataFields.FindPropertyRelative ("vector3Fields"));
		}

		private void DrawList (SerializedProperty list)
		{
			SerializedProperty size = list.FindPropertyRelative ("Array.size");

			if (size.hasMultipleDifferentValues)
			{
				EditorGUILayout.HelpBox ("Not showing lists with different sizes.", MessageType.Info);
			}
			else
			{
				for (int i = 0; i < list.arraySize; i++)
				{
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
				}
			}
		}
	}
}