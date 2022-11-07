using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Data
{
	public static class ExtensionMethods
	{
		public static T GetReference<T> (this SerializedProperty property, FieldInfo fieldInfo) where T : DataField
		{
			string name = property.FindPropertyRelative ("name").stringValue;
			object obj = fieldInfo.GetValue (property.GetParent ());

			if (obj == null)
				return null;

			T actualObject = null;

			if (obj.GetType ().IsGenericType)
			{
				IEnumerable list = obj as IEnumerable;

				if (list != null)
				{
					foreach (object o in list)
					{
						T t = o as T;
						
						if (t != null && t.GetName () == name)
							actualObject = t;
					}
				}
			}
			else
			{
				actualObject = obj as T;
			}

			return actualObject;
		}

		public static object GetParent (this SerializedProperty prop)
		{
			string path = prop.propertyPath.Replace (".Array.data[", "[");
			object obj = prop.serializedObject.targetObject;
			var elements = path.Split ('.');

			foreach (string element in elements.Take (elements.Length - 1))
				if (element.Contains ("["))
				{
					string elementName = element.Substring (0, element.IndexOf ("["));
					int index = Convert.ToInt32 (element.Substring (element.IndexOf ("[")).Replace ("[", "").Replace ("]", ""));
					obj = GetValue (obj, elementName, index);
				}
				else
				{
					obj = GetValue (obj, element);
				}

			return obj;
		}

		public static object GetValue (this object source, string name)
		{
			if (source == null)
				return null;

			Type type = source.GetType ();
			FieldInfo f = type.GetField (name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			if (f == null)
			{
				PropertyInfo p = type.GetProperty (name,
					BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
				if (p == null)
					return null;

				return p.GetValue (source, null);
			}

			return f.GetValue (source);
		}

		public static object GetValue (this object source, string name, int index)
		{
			IEnumerable enumerable = GetValue (source, name) as IEnumerable;
			IEnumerator enm = enumerable.GetEnumerator ();
			try
			{
				while (index-- >= 0)
					enm.MoveNext ();
				
				return enm.Current;
			}
			catch (Exception e)
			{
				Debug.Log (e);
				return null;
			}	
		}

		public static char ToUpperChar (this int index)
		{
			if (index < 1 || index > 26)
				throw new ArgumentOutOfRangeException ("index");

			return "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[index - 1];
		}
	}
}