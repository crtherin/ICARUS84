using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace Data
{
	public abstract class DataFieldDrawer<T> : PropertyDrawer
	{
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty (position, label, property);

			//int indent = EditorGUI.indentLevel;
			//EditorGUI.indentLevel = 0;

			object targetObject = property.serializedObject.targetObject;
			DataField field = property.GetReference<DataField<T>> (fieldInfo);

			if (targetObject is IDataDriven && !(targetObject is Container))
				SyncFromNode (property, (IDataDriven) targetObject, field);

			EditorGUI.BeginChangeCheck ();

			/*if (!(field is LayerMaskData))
			{*/
				EditorGUI.PropertyField (position,
					property.FindPropertyRelative ("value"), new GUIContent (
						property.FindPropertyRelative ("name").stringValue));
			/*}
			else
			{
				((LayerMaskData) field).Set (EditorGUI.MaskField (position,
					property.FindPropertyRelative ("name").stringValue,
					((LayerMaskData) field).Get (),
					InternalEditorUtility.layers));
			}*/

			if (EditorGUI.EndChangeCheck ())
			{
				property.serializedObject.ApplyModifiedProperties ();

				field.SyncToConnections ();

				if (targetObject is IDataDriven)
				{
					if (!Application.isPlaying)
						SyncToNode (property, (IDataDriven) targetObject, field);

					SetContainerDirty (property);
				}

				if (Application.isPlaying)
				{
					AssetDatabase.SaveAssets ();
					AssetDatabase.Refresh ();
				}
			}

			//EditorGUI.indentLevel = indent;
			EditorGUI.EndProperty ();
		}

		private static void SyncFromNode (SerializedProperty property, IDataDriven targetObject, DataField field)
		{
			if (field == null)
				return;

			Node node = targetObject.FindNode (field);

			if (node == null)
				return;

			DataField containerField = field.GetOrCreateFieldIn (node.GetTable ());

			if (containerField == null)
				return;

			containerField.SyncTo (field);
			property.serializedObject.ApplyModifiedProperties ();
		}

		private static void SyncToNode (SerializedProperty property, IDataDriven targetObject, DataField field)
		{
			if (field == null)
				return;

			Node node = targetObject.FindNode (field);

			if (node == null)
				return;

			DataField containerField = field.GetOrCreateFieldIn (node.GetTable ());

			if (containerField == null)
				return;

			field.SyncTo (containerField);

			SerializedProperty containerProperty = property.serializedObject.FindProperty ("container");
			if (containerProperty != null && containerProperty.objectReferenceValue != null)
				EditorUtility.SetDirty (containerProperty.objectReferenceValue as Container);
		}

		private static void SetContainerDirty (SerializedProperty property)
		{
			SerializedProperty containerProperty = property.serializedObject.FindProperty ("container");

			if (containerProperty == null)
				return;

			Container container = containerProperty.objectReferenceValue as Container;

			if (container != null)
				EditorUtility.SetDirty (container);
		}
	}

	[CustomPropertyDrawer (typeof (IntData))]
	public class IntDrawer : DataFieldDrawer<int>
	{
	}

	[CustomPropertyDrawer (typeof (FloatData))]
	public class FloatDrawer : DataFieldDrawer<float>
	{
	}

	[CustomPropertyDrawer (typeof (BoolData))]
	public class BoolDrawer : DataFieldDrawer<bool>
	{
	}

	[CustomPropertyDrawer (typeof (StringData))]
	public class StringDrawer : DataFieldDrawer<string>
	{
	}

	[CustomPropertyDrawer (typeof (Vector2Data))]
	public class Vector2Drawer : DataFieldDrawer<Vector2>
	{
	}

	[CustomPropertyDrawer (typeof (Vector3Data))]
	public class Vector3Drawer : DataFieldDrawer<Vector3>
	{
	}

	[CustomPropertyDrawer (typeof (TransformData))]
	public class TransformDrawer : DataFieldDrawer<Transform>
	{
	}

	[CustomPropertyDrawer (typeof (LayerMaskData))]
	public class LayerMaskDrawer : DataFieldDrawer<LayerMask>
	{
	}
}