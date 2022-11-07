using System;
using UnityEngine;
using UnityEditor;

public class AssetNesting : EditorWindow
{
	private ScriptableObject root;
	private ScriptableObject target;

	[MenuItem ("Assets/Nesting")]
	private static void Init ()
	{
		AssetNesting window = (AssetNesting) GetWindow (typeof (AssetNesting));
		window.Show ();
	}

	private void OnGUI ()
	{
		bool enabled = GUI.enabled;

		GUILayout.Label ("Asset Nesting", EditorStyles.boldLabel);

		root = EditorGUILayout.ObjectField (new GUIContent ("Root"), root, typeof (ScriptableObject), false) as ScriptableObject;
		target = EditorGUILayout.ObjectField (new GUIContent ("Target"), target, typeof (ScriptableObject), false) as ScriptableObject;

		GUILayout.BeginHorizontal ();

		bool isChild = AssetDatabase.LoadAssetAtPath<ScriptableObject> (AssetDatabase.GetAssetPath (target)) != target;

		GUI.enabled = enabled && root != target && !isChild;
		if (root != null && target != null && GUILayout.Button ("Child"))
		{
			ScriptableObject copy = GetCopy (target);
			AssetDatabase.AddObjectToAsset (copy, root);
			AssetDatabase.SaveAssets ();
			target = copy;
		}

		GUI.enabled = enabled && root != target && isChild;
		if (root != null && target != null && GUILayout.Button ("Sibling"))
		{
			ScriptableObject copy = GetCopy (target);
			AssetDatabase.CreateAsset (copy, AssetDatabase.GetAssetPath (root).Replace (root.name, copy.name));
			AssetDatabase.SaveAssets ();
			target = copy;
		}

		GUILayout.EndHorizontal ();

		GUI.enabled = enabled;
	}

	private static ScriptableObject GetCopy (ScriptableObject source)
	{
		string json = JsonUtility.ToJson (source);
		string name = source.name;
		Type type = source.GetType ();

		ScriptableObject atPath = AssetDatabase.LoadAssetAtPath<ScriptableObject> (AssetDatabase.GetAssetPath (source));

		// If source isn't a child
		if (atPath == source)
			AssetDatabase.DeleteAsset (AssetDatabase.GetAssetPath (source));
		else
			DestroyImmediate (source, true);

		AssetDatabase.SaveAssets ();
		ScriptableObject copy = CreateInstance (type);

		if (copy == null)
			return null;

		JsonUtility.FromJsonOverwrite (json, copy);
		copy.name = name;
		return copy;
	}
}