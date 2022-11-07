using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class ScriptableObjectUtility
{
#if UNITY_EDITOR
	public static void CreateAsset<T> () where T : ScriptableObject
	{
		T asset = ScriptableObject.CreateInstance<T> ();

		string path = AssetDatabase.GetAssetPath (Selection.activeObject);

		if (path == "")
			path = "Assets";
		else if (Path.GetExtension (path) != "")
			path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");

		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/New " + typeof (T) + ".asset");

		AssetDatabase.CreateAsset (asset, assetPathAndName);

		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;
	}

	public static T CreateAsset<T> (string path) where T : ScriptableObject
	{
		T asset = ScriptableObject.CreateInstance<T> ();

		path = "Assets/" + path.Replace (Application.dataPath, "");

		AssetDatabase.CreateAsset (asset, path);
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();
		EditorUtility.FocusProjectWindow ();
		asset = AssetDatabase.LoadAssetAtPath<T> (path);
		Selection.activeObject = asset;
		return asset;
	}
#endif
}