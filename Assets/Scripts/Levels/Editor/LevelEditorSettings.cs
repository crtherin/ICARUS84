using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Levels.Editor
{
    [System.Serializable]
    class Brush
    {
        [SerializeField] private Sprite sprite;
        [SerializeField] private Transform prefab;

        public Sprite Sprite => sprite;

        public Transform Prefab => prefab;
    }

    class LevelEditorSettings : ScriptableObject
    {
        private const string Path = "Assets/Data/Editor/LevelEditor.asset";

        [SerializeField] private Brush[] brushes;
        [SerializeField] private bool isEnabled;

        public Brush[] GetBrushes()
        {
            return brushes;
        }

        internal static LevelEditorSettings GetOrCreateAsset()
        {
            var settings = AssetDatabase.LoadAssetAtPath<LevelEditorSettings>(Path);
            if (settings == null)
            {
                settings = CreateInstance<LevelEditorSettings>();
                settings.brushes = new Brush[0];
                AssetDatabase.CreateAsset(settings, Path);
                AssetDatabase.SaveAssets();
            }

            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateAsset());
        }
    }

    static class LevelEditorSettingsRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreateLevelEditorSettingsProvider()
        {
            var provider = new SettingsProvider("Project/Level Editor", SettingsScope.Project)
            {
                label = "Level Editor",
                guiHandler = (searchContext) =>
                {
                    var settings = LevelEditorSettings.GetSerializedSettings();
                    EditorGUILayout.PropertyField(settings.FindProperty("brushes"), true);
                    settings.ApplyModifiedProperties();
                },

                keywords = new HashSet<string>(new[] {"Level Editor", "Tiles"})
            };

            return provider;
        }
    }
}