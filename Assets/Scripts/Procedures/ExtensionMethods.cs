using System;
using Procedures;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

public static partial class ExtensionMethods
{
	public static Type ToType<T> (this string name)
	{
		return typeof (T).Assembly.GetType (typeof (T).Namespace + "." + name);
	}

	public static FieldInfo[] GetFields<T> (this Type type)
	{
		return type.GetFields (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
			.Where (fieldinfo => typeof (T).IsAssignableFrom (fieldinfo.FieldType)).ToArray ();
	}

	public static T GetDefaultValue<T> (this FieldInfo fieldInfo, Type type)
	{
		return (T) fieldInfo.GetValue (Activator.CreateInstance (type));
	}

	public static string[] FindAllTypeNames (this Type type)
	{
		return type.FindAllDerivedTypes ().Select (t => t.Name).ToArray ();
	}

	public static List<Type> FindAllDerivedTypes (this Type type)
	{
		return type.FindAllDerivedTypes (Assembly.GetAssembly (type));
	}

	public static List<Type> FindAllDerivedTypes (this Type type, Assembly assembly)
	{
		return assembly.GetTypes ().Where (t => t != type && !t.IsAbstract && type.IsAssignableFrom (t)).ToList ();
	}

	public static T Cast<T> (this ICallback callback) where T : ICallback
	{
		return (T) callback;
	}

	public static void SafeInvoke<T> (this ICallback callback, Action<T> action) where T : ICallback
	{
		if (!(callback is T))
			return;

		action (callback.Cast<T> ());
	}

	public static bool SafeInvoke<T> (this ICallback callback, Func<T, bool> func) where T : ICallback
	{
		if (!(callback is T))
			return false;

		return func (callback.Cast<T> ());
	}

	public static int RotateLeft (this int value, int count)
	{
		return (value << count) | (value >> (32 - count));
	}

	public static float MinMaxRandom(this UnityEngine.Vector2 minMax)
	{
		return UnityEngine.Random.Range(minMax.x, minMax.y);
	}

	public static bool MinMaxCheck(this UnityEngine.Vector2 minMax, float value)
	{
		if (value < minMax.x)
			return false;
		
		if (value > minMax.y)
			return false;

		return true;
	}
}