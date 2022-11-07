using System;
using Levels.Tiles;
using UnityEditor;
using UnityEngine;

namespace Levels.Editor
{
    public class LevelEditor : EditorWindow
    {
        private const string DefaultParent = "Level";
        private Transform parent;

        private LevelEditorSettings settings;
        private SerializedObject serializedSettings;
        private Vector2 lastPosition;
        private Tile selectedTile;
        private Tile tempTile;
        private int activeTool;

        protected void OnFocus()
        {
            GameObject parentObject = GameObject.Find(DefaultParent);

            if (parentObject == null)
                parentObject = new GameObject(DefaultParent);

            parent = parentObject.transform;
            settings = LevelEditorSettings.GetOrCreateAsset();
            serializedSettings = LevelEditorSettings.GetSerializedSettings();
            Toggle(serializedSettings.FindProperty("isEnabled").boolValue);
        }

        protected void OnDestroy()
        {
            Toggle(false);
        }

        private void Toggle(bool enabled)
        {
            if (enabled)
            {
                SceneView.duringSceneGui -= OnSceneGUI;
                SceneView.duringSceneGui += OnSceneGUI;
            }
            else
            {
                RemoveTemp();
                SceneView.duringSceneGui -= OnSceneGUI;
            }

            SceneView.RepaintAll();
        }

        private void OnGUI()
        {
            SerializedProperty isEnabled = serializedSettings.FindProperty("isEnabled");
            bool newEnabled = GUILayout.Toggle(isEnabled.boolValue, "Enabled", GUI.skin.button);

            if (isEnabled.boolValue != newEnabled)
            {
                isEnabled.boolValue = newEnabled;

                Toggle(newEnabled);

                serializedSettings.ApplyModifiedProperties();
            }

            if (GUILayout.Button("Reset Persistence"))
                foreach (var persistence in FindObjectsOfType<PersistenceBase>())
                    persistence.Initialize();

            if (GUILayout.Button("Clear Persistence"))
                foreach (var persistence in FindObjectsOfType<PersistenceBase>())
                    persistence.Clear();
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            DrawSelection(sceneView);
            Paint(sceneView);
        }

        private void DrawSelection(SceneView sceneView)
        {
            Handles.BeginGUI();

            GUI.color = new Color(1, 1, 1, 0.5f);
            GUILayout.BeginArea(new Rect(0, 0, SceneView.currentDrawingSceneView.position.width, 57));
            GUILayout.BeginHorizontal(GUI.skin.box);

            Brush[] brushes = settings.GetBrushes();

            foreach (Brush brush in brushes)
            {
                bool isSelected = selectedTile == brush.Prefab.GetComponent<Tile>();
                GUI.backgroundColor = isSelected ? Color.grey : Color.white;

                if (GUILayout.Button(brush.Sprite.texture, GUILayout.Width(50), GUILayout.Height(50)))
                {
                    selectedTile = !isSelected ? brush.Prefab.GetComponent<Tile>() : null;
                    RemoveTemp();
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            GUI.color = Color.white;

            Handles.EndGUI();
        }

        private void Paint(SceneView sceneView)
        {
            if (selectedTile == null)
                return;

            if (Event.current.type == EventType.Layout)
                HandleUtility.AddDefaultControl(Event.current.button);

            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                Event.current.Use();
                RemoveTemp();
                selectedTile = null;
                activeTool = 0;
                return;
            }

            float snap = selectedTile.Snap;

            Vector2 newPosition = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
            newPosition.x = Mathf.Round(newPosition.x / snap) * snap;
            newPosition.y = Mathf.Round(newPosition.y / snap) * snap;

            Tile hit = Tile.OverlapTile(newPosition, selectedTile.Mask);

            switch (activeTool)
            {
                case 0:
                    if (hit == null || hit == tempTile)
                    {
                        Handles.color = Color.green;

                        if (Vector2.Distance(lastPosition, newPosition) > snap / 2)
                        {
                            RemoveTemp();
                            AddTemp(newPosition);
                        }

                        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                        {
                            Event.current.Use();
                            activeTool = 1;
                            StoreTemp();
                        }

                        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.R)
                        {
                            Event.current.Use();
                            activeTool = 3;
                            RemoveTemp();
                        }
                    }
                    else
                    {
                        Handles.color = Color.red;
                        RemoveTemp();

                        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                        {
                            Event.current.Use();
                            activeTool = 2;
                        }

                        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.R)
                        {
                            Event.current.Use();
                            activeTool = 4;
                        }
                    }

                    break;

                case 1:
                    Handles.color = Color.green;

                    if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
                    {
                        Event.current.Use();
                        activeTool = 0;
                    }
                    else if (hit == null)
                    {
                        Add(selectedTile, newPosition);
                    }

                    break;

                case 2:
                    Handles.color = Color.red;

                    if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
                    {
                        Event.current.Use();
                        activeTool = 0;
                    }
                    else if (hit != null)
                    {
                        Remove(hit);
                    }

                    break;

                case 3:
                    Handles.color = Color.green;

                    if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.R)
                    {
                        Event.current.Use();
                        activeTool = 0;
                        break;
                    }

                    Vector2 avgSize = Vector3.one;
                    avgSize.x = Mathf.Abs(newPosition.x - lastPosition.x) + snap;
                    avgSize.y = Mathf.Abs(newPosition.y - lastPosition.y) + snap;
                    Handles.DrawWireCube((lastPosition + newPosition) / 2, avgSize);

                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    {
                        Event.current.Use();
                        Fill(lastPosition, newPosition, true);
                        activeTool = 0;
                        break;
                    }

                    sceneView.Repaint();
                    return;

                case 4:
                    Handles.color = Color.red;

                    if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.R)
                    {
                        Event.current.Use();
                        activeTool = 0;
                        break;
                    }

                    avgSize = Vector3.one;
                    avgSize.x = Mathf.Abs(newPosition.x - lastPosition.x) + snap;
                    avgSize.y = Mathf.Abs(newPosition.y - lastPosition.y) + snap;
                    Handles.DrawWireCube((lastPosition + newPosition) / 2, avgSize);

                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    {
                        Event.current.Use();
                        activeTool = 0;
                        Fill(lastPosition, newPosition, false);
                        break;
                    }

                    sceneView.Repaint();
                    return;
            }

            lastPosition = newPosition;

            Handles.DrawWireCube(lastPosition, Vector3.one * snap);
            sceneView.Repaint();
        }

        private Transform GetParent(Transform spawned)
        {
            String layerName = LayerMask.LayerToName(spawned.gameObject.layer);

            if (string.IsNullOrEmpty(layerName))
                return parent;

            Transform layerParent = parent.Find(layerName);

            if (layerParent == null)
            {
                layerParent = new GameObject(layerName).transform;
                layerParent.parent = parent;
                layerParent.localPosition = Vector3.zero;
                layerParent.localScale = Vector3.one;
            }

            return layerParent;
        }

        private Tile Add(Tile tile, Vector2 newPosition = default, Quaternion newRotation = default)
        {
            Transform spawned = Tile.SpawnPrefab(tile.transform, newPosition, newRotation);
            spawned.transform.parent = GetParent(spawned);

            tile = spawned.GetComponent<Tile>();
            if (tile == null)
                tile = spawned.gameObject.AddComponent<Tile>();

            tile.Refresh();
            return tile;
        }

        private void Remove(Tile tile)
        {
            tile.gameObject.SetActive(false);
            tile.Refresh();
            DestroyImmediate(tile.gameObject);
        }

        private void Fill(Vector2 a, Vector2 b, bool isAdding)
        {
            Vector2 bottomLeft = new Vector2(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y));
            Vector2 topRight = new Vector2(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y));

            for (float x = bottomLeft.x; x <= topRight.x; x += selectedTile.Snap)
            {
                for (float y = bottomLeft.y; y <= topRight.y; y += selectedTile.Snap)
                {
                    Vector2 newPosition = new Vector2(x, y);

                    Tile hit = Tile.OverlapTile(newPosition, selectedTile.Mask);
                    if (isAdding)
                    {
                        if (!hit)
                            Add(selectedTile, newPosition);
                    }
                    else
                    {
                        if (hit)
                            Remove(hit);
                    }
                }
            }
        }

        private void AddTemp(Vector2 tempPosition)
        {
            tempTile = Add(selectedTile, tempPosition);
            tempTile.gameObject.hideFlags = HideFlags.HideAndDontSave;
        }

        private void RemoveTemp()
        {
            if (tempTile == null)
                return;

            Remove(tempTile);
            tempTile = null;
        }

        private void StoreTemp()
        {
            if (tempTile == null)
                return;

            tempTile.gameObject.hideFlags = HideFlags.None;
            tempTile = null;
        }

        [MenuItem("Window/Level Editor")]
        private static void ShowWindow()
        {
            LevelEditor levelEditor = (LevelEditor) GetWindow(typeof(LevelEditor));
            levelEditor.titleContent = new GUIContent("Level Editor");
            levelEditor.Show();
        }
    }
}