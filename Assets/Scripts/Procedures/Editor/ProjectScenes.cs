namespace Procedures.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System.Collections.Generic;
	using UnityEditor.SceneManagement;

	/// <summary>
	/// Class shows all scenes in project and helps switching between them.
	/// </summary>
	public class ProjectScenes : EditorWindow
	{
		internal class ScenesFolder
		{
			public string path;
			public bool unfolded;
			public Dictionary<string, Scene> scenes;

			public void Draw ()
			{
				unfolded = EditorGUILayout.Foldout (unfolded, path);
			}
		}

		internal class Scene
		{
			public string path;
			public string name;

			public void Draw ()
			{
				var opened = EditorSceneManager.GetActiveScene ().path == path;
				var style = opened ? new GUIStyle (EditorStyles.boldLabel) : new GUIStyle (EditorStyles.label);
				style.margin.left = 20;

				if ((GUILayout.Button (name, style)))
				{
					if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo ())
						EditorSceneManager.OpenScene (path);
				}
			}
		}

		private static Dictionary<string, ScenesFolder> sceneFolders;
		private static Vector2 scrollPosition;

		[MenuItem ("Window/Project Scenes")]
		static void Init ()
		{
			var window = GetWindow (typeof (ProjectScenes));
			window.titleContent = new GUIContent ("Project Scenes");
			window.Show ();

			if (sceneFolders == null)
				sceneFolders = new Dictionary<string, ScenesFolder> ();
		}

		void OnGUI ()
		{
			EditorGUILayout.BeginVertical (EditorStyles.inspectorDefaultMargins);
			EditorGUILayout.LabelField ("Scenes in project:");

			scrollPosition = EditorGUILayout.BeginScrollView (scrollPosition);

			string[] scenesGUIDs = AssetDatabase.FindAssets ("t:Scene");
			string previousPath = "";
			for (int i = 0; i < scenesGUIDs.Length; i++)
			{
				var scenePath = AssetDatabase.GUIDToAssetPath (scenesGUIDs[i]);
				var folderPath = scenePath.Substring (0, scenePath.LastIndexOf ("/"));

				ScenesFolder folder = null;
				if (!sceneFolders.TryGetValue (folderPath, out folder))
				{
					folder = new ScenesFolder () {path = folderPath, scenes = new Dictionary<string, Scene> ()};
					sceneFolders.Add (folderPath, folder);
				}

				Scene scene = null;
				if (!folder.scenes.TryGetValue (scenePath, out scene))
				{
					scene = new Scene () {path = scenePath, name = scenePath.Substring (scenePath.LastIndexOf ("/") + 1)};
					folder.scenes.Add (scenePath, scene);
				}

				if (previousPath != folder.path)
					folder.Draw ();

				if (folder.unfolded)
					scene.Draw ();

				previousPath = folder.path;
			}

			EditorGUILayout.EndScrollView ();
			EditorGUILayout.EndVertical ();
		}
	}
}