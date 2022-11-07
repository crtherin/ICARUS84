using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Prefab))]
public class PrefabPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        bool enabled = GUI.enabled;
        GUI.enabled = !Application.isPlaying;

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        EditorGUI.BeginChangeCheck();

        GameObject previousValue = property.FindPropertyRelative("value").objectReferenceValue as GameObject;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("value"), GUIContent.none);

        if (EditorGUI.EndChangeCheck())
        {
            GameObject value = property.FindPropertyRelative("value").objectReferenceValue as GameObject;
            MonoBehaviour user = property.serializedObject.targetObject as MonoBehaviour;

            if (previousValue != null && user != null)
            {
                PrefabID prefabID = previousValue.GetComponent<PrefabID>();

                if (prefabID != null)
                    PrefabManager.UnregisterSceneUser(prefabID.Value, user);
            }

            if (value != null)
            {
                if (!PrefabUtility.IsPartOfAnyPrefab(value) ||
                    PrefabUtility.IsPrefabAssetMissing(value))
                {
                    value = null;
                }
                else
                {
                    if (!PrefabUtility.IsPartOfPrefabAsset(value))
                    {
                        value = PrefabUtility.GetCorrespondingObjectFromSource(value);
                    }

                    value = value.transform.root.gameObject;

                    if (user != null)
                    {
                        PrefabID prefabID = value.GetComponent<PrefabID>();

                        if (prefabID == null)
                            prefabID = value.AddComponent<PrefabID>();

                        PrefabManager.RegisterSceneUser(prefabID.Value, value, user);
                    }
                }

                property.FindPropertyRelative("value").objectReferenceValue = value;
            }
        }

        GUI.enabled = enabled;

        EditorGUI.EndProperty();
    }
}