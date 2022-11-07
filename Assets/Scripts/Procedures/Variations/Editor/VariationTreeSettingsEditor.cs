using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor (typeof (VariationTreeSettings))]
internal class VariationTreeSettingsEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		serializedObject.Update ();
		DrawDefaultInspector ();
		serializedObject.ApplyModifiedProperties ();

		foreach (Object o in targets)
		{
			VariationTreeSettings script = o as VariationTreeSettings;
			if (script != null)
				script.Sync ();
		}
	}
}