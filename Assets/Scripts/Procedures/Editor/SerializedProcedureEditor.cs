using System;
using System.Reflection;
using Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Procedures.Editor
{
	[CustomEditor (typeof (SerializedProcedure))]
	public class SerializedProcedureEditor : UnityEditor.Editor
	{
		private SerializedProcedure targetScript;

		public void OnEnable ()
		{
			targetScript = target as SerializedProcedure;
		}

		public override void OnInspectorGUI ()
		{
			DrawProcedure (serializedObject, targetScript);
		}

		public static void DrawProcedure (SerializedObject serializedObject, SerializedProcedure targetScript)
		{
			if (serializedObject == null || targetScript == null)
				return;

			serializedObject.Update ();

			string previousName = serializedObject.FindProperty ("name").stringValue;
			string newName = EditorGUILayout.TextField ("Name", previousName);
			bool shouldApplyTree = targetScript.ApplyTree;
			if (previousName != newName)
				targetScript.UpdateNodeName (newName);

			EditorGUILayout.PropertyField (serializedObject.FindProperty ("container"));

			EditorGUILayout.Space ();

			EditorGUILayout.BeginVertical (EditorStyles.textArea);

			EditorGUILayout.Space ();

			EditorGUILayout.BeginHorizontal ();

			SerializedProperty isTreeFolded = serializedObject.FindProperty ("isTreeFolded");
			if (GUILayout.Button (isTreeFolded.boolValue ? "▶" : "▼", EditorStyles.label))
				isTreeFolded.boolValue = !isTreeFolded.boolValue;

			SerializedProperty applyTree = serializedObject.FindProperty ("applyTree");
			GUILayout.Label ("Variations Tree");

			GUILayout.FlexibleSpace ();

			if (GUILayout.Button (applyTree.boolValue ? "Hide" : "Show", EditorStyles.miniLabel))
				applyTree.boolValue = !applyTree.boolValue;

			EditorGUILayout.EndHorizontal ();

			if (!isTreeFolded.boolValue)
				DrawTree (serializedObject.FindProperty ("tree"), targetScript);

			EditorGUILayout.Space ();

			EditorGUILayout.EndVertical ();

			EditorGUILayout.Space ();

			bool enabled = GUI.enabled;

			SerializedProperty list = serializedObject.FindProperty ("processes");

			for (int i = 0; i < list.arraySize; i++)
			{
				if (!applyTree.boolValue ||
				    targetScript.GetTree () == null ||
				    targetScript.GetTree ().ApplyMask (targetScript.GetHash (i)))
				{
					DrawProcess (list, i, targetScript);
				}
			}

			EditorGUILayout.Space ();
			if (GUILayout.Button ("Add Process"))
				targetScript.AddNewProcess ();

			GUI.enabled = enabled;

			serializedObject.ApplyModifiedProperties ();
		}

		private static void DrawTree (SerializedProperty treeProperty, SerializedProcedure targetScript)
		{
			if (targetScript == null)
				return;

			if (targetScript.GetTree () == null)
			{
				if (GUILayout.Button ("Add Variation Tree"))
					targetScript.SetTree (new VariationTree ());

				return;
			}

			EditorGUILayout.PropertyField (treeProperty);
		}

		private static void DrawProcess (SerializedProperty processes, int index, SerializedProcedure targetScript)
		{
			bool previousEnabled = GUI.enabled;
			bool enabled = GUI.enabled;
			Color previousColor = GUI.color;

			VariationTree tree = targetScript.GetTree ();

			if (tree != null)
			{
				int hash = targetScript.GetHash (index);
				bool isSelected = tree.ApplyMask (hash);
				GUI.color = isSelected ? Color.white : Color.gray;
			}

			EditorGUI.indentLevel = 0;

			EditorGUILayout.BeginVertical (EditorStyles.textArea);

			EditorGUILayout.BeginHorizontal ();

			SerializedProperty process = processes.GetArrayElementAtIndex (index);
			SerializedProperty isFolded = process.FindPropertyRelative ("isFolded");

			if (GUILayout.Button (isFolded.boolValue ? "▶" : "▼", EditorStyles.label))
				isFolded.boolValue = !isFolded.boolValue;

			DrawProcessPopup (process.FindPropertyRelative ("type"), index, targetScript);

			GUILayout.FlexibleSpace ();

			if (tree != null)
			{
				int hash = targetScript.GetHash (index);
				bool isSelected = tree.ApplyMask (hash);
				if (!targetScript.IsTreeFolded ())
				{
					if (GUILayout.Button (isSelected ? "Exclude" : "Include", EditorStyles.miniBoldLabel))
						tree.ToggleInSelected (hash);
				}
			}

			GUI.enabled = enabled && index > 0;
			if (GUILayout.Button ("▲", EditorStyles.miniLabel))
				targetScript.SwitchProcesses (index, index - 1);

			GUI.enabled = enabled && index < processes.FindPropertyRelative ("Array.size").intValue - 1;
			if (GUILayout.Button ("▼", EditorStyles.miniLabel))
				targetScript.SwitchProcesses (index, index + 1);

			GUI.enabled = enabled;
			if (GUILayout.Button ("+", EditorStyles.miniLabel))
				targetScript.InsertProcessAfter (index);

			if (GUILayout.Button ("-", EditorStyles.miniLabel))
			{
				targetScript.RemoveProcessAt (index);
				processes.serializedObject.ApplyModifiedProperties ();
			}

			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			if (!isFolded.boolValue)
				DrawDataFieldTable (process.FindPropertyRelative ("table"), process.FindPropertyRelative ("type").stringValue);

			EditorGUILayout.Space ();

			EditorGUILayout.EndVertical ();

			GUI.enabled = previousEnabled;
			GUI.color = previousColor;
		}

		private static void DrawProcessPopup (SerializedProperty property, int index, SerializedProcedure targetScript)
		{
			string[] behaviourList = typeof (Process).FindAllTypeNames ();

			string propertyString = property.stringValue;

			int popupIndex = -1;

			for (int i = 0; i < behaviourList.Length; i++)
			{
				if (behaviourList[i] == propertyString)
				{
					popupIndex = i;
					break;
				}
			}

			EditorGUILayout.BeginHorizontal ();

			int previousPopupIndex = popupIndex;
			popupIndex = EditorGUILayout.Popup (popupIndex, behaviourList, EditorStyles.boldLabel);

			EditorGUILayout.EndHorizontal ();

			if (popupIndex != previousPopupIndex)
			{
				targetScript.Instantiate (index).SetType (behaviourList[popupIndex]);
				targetScript.UpdateChildNodeName (index);
			}
		}

		public static void DrawDataFieldTable (SerializedProperty table, string typeName)
		{
			Type type = typeName.ToType<Process> ();
			FieldInfo[] fields = type.GetFields<DataField> ();

			for (int i = 0; i < fields.Length; i++)
			{
				Process tempProcess = type.CreateInstance<Process> ();
				DataField dataField = fields[i].GetValue (tempProcess) as DataField;

				DrawDataField (table.FindPropertyRelative ("intFields"), dataField);
				DrawDataField (table.FindPropertyRelative ("floatFields"), dataField);
				DrawDataField (table.FindPropertyRelative ("boolFields"), dataField);
				DrawDataField (table.FindPropertyRelative ("stringFields"), dataField);
				DrawDataField (table.FindPropertyRelative ("vector2Fields"), dataField);
				DrawDataField (table.FindPropertyRelative ("vector3Fields"), dataField);
				DrawDataField (table.FindPropertyRelative ("transformFields"), dataField);
				DrawDataField (table.FindPropertyRelative ("audioClipFields"), dataField);
			}
		}

		private static void DrawDataField (SerializedProperty list, DataField dataField)
		{
			for (int i = 0; i < list.arraySize; i++)
			{
				SerializedProperty dataFieldProperty = list.GetArrayElementAtIndex (i);

				if (dataFieldProperty.type == dataField.GetType ().Name &&
				    dataFieldProperty.FindPropertyRelative ("name").stringValue == dataField.GetName ())
				{
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
				}
			}
		}
	}
}