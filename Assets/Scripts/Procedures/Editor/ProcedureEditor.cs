using Data;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEditorInternal;
using System.Collections.Generic;

namespace Procedures.Editor
{
	[CustomEditor (typeof (Procedure))]
	public class ProcedureEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			bool enabled = GUI.enabled;

			SerializedProperty serializedProcedureProperty = serializedObject.FindProperty ("serializedProcedure");
			UnityEditor.Editor editor = CreateEditor (serializedProcedureProperty.objectReferenceValue);

			EditorGUILayout.Space ();

			if (Application.isPlaying)
			{
				Procedure procedure = (Procedure) target;

				if (procedure != null)
				{
					EditorGUILayout.Space ();

					EditorGUILayout.BeginVertical (EditorStyles.textArea);
					GUILayout.Label ("Live Procedure");

					EditorGUILayout.Space ();

					DrawTree (procedure.GetTree ());

					EditorGUILayout.Space ();

					List<Process> processes = procedure.GetProcesses ();

					if (processes != null)
					{
						for (int i = 0; i < processes.Count; i++)
						{
							Process process = processes[i];

							if (process.GetHash () != null && !procedure.GetTree ().ApplyMask (process.GetHash ().Value))
								continue;

							EditorGUILayout.BeginVertical (EditorStyles.textArea);

							EditorGUILayout.Space ();

							EditorGUILayout.BeginHorizontal ();

							if (GUILayout.Button (process.IsFolded () ? "▶" : "▼", EditorStyles.label))
								process.IsFolded (!process.IsFolded ());

							GUILayout.Label (process.GetType ().Name, EditorStyles.boldLabel);

							GUILayout.FlexibleSpace ();

							EditorGUILayout.EndHorizontal ();

							EditorGUILayout.Space ();

							FieldInfo[] fields = process.GetType ().GetFields<DataField> ();

							for (int j = 0; j < fields.Length; j++)
								DrawField (fields[j], process);

							EditorGUILayout.EndVertical ();
						}
					}

					EditorGUILayout.EndVertical ();

					EditorGUILayout.Space ();
				}
			}

			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (serializedProcedureProperty);
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();

			if (serializedProcedureProperty.objectReferenceValue == null)
				return;

			SerializedProcedureEditor.DrawProcedure (editor.serializedObject,
				serializedProcedureProperty.objectReferenceValue as SerializedProcedure);

			EditorUtility.SetDirty (serializedProcedureProperty.objectReferenceValue as SerializedProcedure);

			GUI.enabled = enabled;

			serializedObject.ApplyModifiedProperties ();
		}

		public static void DrawTree (VariationTree tree)
		{
			if (tree == null)
				return;

			int variationsCount = tree.GetVariationsCount ();
			int selectedVariation = tree.GetSelection ();

			bool enabled = GUI.enabled;

			EditorGUILayout.BeginVertical (EditorStyles.textArea);
			GUILayout.Label ("Variation Tree");

			EditorGUILayout.BeginHorizontal (EditorStyles.textArea);

			GUI.enabled = enabled && variationsCount > 0 && selectedVariation >= 0;
			if (GUILayout.Button ("◀", EditorStyles.miniLabel))
				tree.SelectVariation (selectedVariation - 1);

			GUILayout.FlexibleSpace ();

			GUILayout.Label (
				(variationsCount > 0
					? "(" + (selectedVariation + 1) + "/" + variationsCount + ") "
					: "") +
				(selectedVariation < 0
					? "Default"
					: "Variation " + (selectedVariation + 1).ToUpperChar ()));

			GUILayout.FlexibleSpace ();

			GUI.enabled = enabled && variationsCount > 0 && selectedVariation < variationsCount - 1;
			if (GUILayout.Button ("▶", EditorStyles.miniLabel))
				tree.SelectVariation (selectedVariation + 1);

			EditorGUILayout.EndHorizontal ();

			if (selectedVariation >= 0)
			{
				Variation variation = tree.GetVariation (selectedVariation);

				int maxUpgrade = variation.GetMaxUpgrade ();
				int selectedUpgrade = variation.GetUpgrade ();

				EditorGUILayout.BeginHorizontal (EditorStyles.textArea);

				GUI.enabled = enabled && maxUpgrade > 0 && selectedUpgrade > 0;
				if (GUILayout.Button ("◀", EditorStyles.miniLabel))
					variation.SetUpgrade (selectedUpgrade - 1);

				GUILayout.FlexibleSpace ();

				GUILayout.Label (
					(maxUpgrade > 0
						? "(" + selectedUpgrade + "/" + maxUpgrade + ") "
						: "") +
					(selectedUpgrade < 1
						? "No Upgrade"
						: "Upgrade " + selectedUpgrade));

				GUILayout.FlexibleSpace ();

				GUI.enabled = enabled && maxUpgrade > 0 && selectedUpgrade < maxUpgrade;
				if (GUILayout.Button ("▶", EditorStyles.miniLabel))
					variation.SetUpgrade (selectedUpgrade + 1);

				EditorGUILayout.EndHorizontal ();
			}

			EditorGUILayout.EndVertical ();

			GUI.enabled = enabled;
		}

		private static void DrawField (FieldInfo field, object target)
		{
			string name = field.Name.AddSpacing ().FirstLetterToUpper ();
			object value = field.GetValue (target);

			switch (field.FieldType.Name)
			{
				case "IntData":
					((IntData) value).Set (EditorGUILayout.IntField (name, ((IntData) value).Get ()));
					break;

				case "FloatData":
					((FloatData) value).Set (EditorGUILayout.FloatField (name, ((FloatData) value).Get ()));
					break;

				case "BoolData":
					((BoolData) value).Set (EditorGUILayout.Toggle (name, ((BoolData) value).Get ()));
					break;

				case "StringData":
					((StringData) value).Set (EditorGUILayout.TextField (name, ((StringData) value).Get ()));
					break;

				case "Vector2Data":
					((Vector2Data) value).Set (EditorGUILayout.Vector2Field (name, ((Vector2Data) value).Get ()));
					break;

				case "Vector3Data":
					((Vector3Data) value).Set (EditorGUILayout.Vector3Field (name, ((Vector3Data) value).Get ()));
					break;

				case "TransformData":
					((TransformData) value).Set (
						(Transform) EditorGUILayout.ObjectField (name, ((TransformData) value).Get (), typeof (Transform), false));
					break;

				case "LayerMaskData":
					((LayerMaskData) value).Set (EditorGUILayout.MaskField (name, ((LayerMaskData) value).Get (),
						InternalEditorUtility.layers));
					break;
				
				case "AudioClipData":
					((AudioClipData) value).Set((AudioClip)EditorGUILayout.ObjectField (name, ((AudioClipData) value).Get(), typeof(AudioClip), false));
					break;
			}
		}

		private static void DrawProcess (SerializedProperty process)
		{
			bool enabled = GUI.enabled;
			GUI.enabled = true;
			EditorGUI.indentLevel = 0;

			EditorGUILayout.BeginVertical (EditorStyles.textArea);

			EditorGUILayout.BeginHorizontal ();

			GUILayout.Label (process.FindPropertyRelative ("type").stringValue, EditorStyles.boldLabel);
			GUILayout.FlexibleSpace ();

			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Space ();
			SerializedProcedureEditor.DrawDataFieldTable (
				process.FindPropertyRelative ("table"),
				process.FindPropertyRelative ("type").stringValue);
			EditorGUILayout.Space ();

			EditorGUILayout.EndVertical ();
			GUI.enabled = enabled;
		}
	}
}