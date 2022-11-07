using Data;
using System;
using UnityEngine;
using System.Reflection;
using System.Text.RegularExpressions;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static partial class ExtensionMethods
{
	
#if UNITY_EDITOR
	
	public static T GetAssetReference<T> (this T target) where T : UnityEngine.Object
	{
		string path = AssetDatabase.GetAssetPath (target);

		if (string.IsNullOrEmpty (path))
			return target;

		return AssetDatabase.LoadAssetAtPath<T> (path);
	}
	
#endif

	public static void LoadFrom<T> (this T target, DataTable table)
	{
		if (!Application.isPlaying || table == null)
			return;

		FieldInfo[] fields = target.GetType ().GetFields<DataField> ();

		for (int i = 0; i < fields.Length; i++)
		{
			DataField dataField = (DataField) fields[i].GetValue (target);

			if (dataField == null)
				dataField = Activator.CreateInstance (
					fields[i].FieldType,
					fields[i].Name
						.AddSpacing ()
						.FirstLetterToUpper ()) as DataField;

			if (dataField == null)
				continue;

			DataField containerField = dataField.GetOrCreateFieldIn (table);

			if (containerField == null)
				continue;

			fields[i].SetValue (target, containerField);
		}
	}
	public static void LoadFrom<T> (this T target, Node node)
	{
		LoadFrom (target, node != null ? node.GetTable () : null);
	}

	public static void ConnectTo (this DataTable target, DataTable source)
	{
		if (target == null || source == null)
			return;

		DataField[] fields = target.GetAllFields ();

		for (int i = 0; i < fields.Length; i++)
		{
			DataField field = fields[i];

			if (field == null)
				continue;

			DataField containerField = field.GetOrCreateFieldIn (source);

			if (containerField == null)
				continue;

			field.ConnectTo (containerField);
		}
	}

	public static T CreateInstance<T> (this Type type)
	{
		return (T) Activator.CreateInstance (type);
	}

	public static string AddSpacing (this string str)
	{
		return Regex.Replace (str, "([a-z])_?([A-Z])", "$1 $2");
	}

	public static string FirstLetterToUpper (this string str)
	{
		if (str == null)
			return null;
		if (str.Length > 1)
			return char.ToUpper (str[0]) + str.Substring (1);

		return str.ToUpper ();
	}
}