using System;
using System.Collections.Generic;

public static partial class ExtensionMethods
{
	public static void Switch<T> (this List<T> list, int indexA, int indexB)
	{
		T a = list[indexA];
		list[indexA] = list[indexB];
		list[indexB] = a;
	}

	public static void SafeInvoke (this EventHandler eventHandler, object sender, EventArgs e)
	{
		if (eventHandler == null)
			return;

		eventHandler.Invoke (sender, e);
	}

	public static void SafeInvoke<T> (this EventHandler<T> eventHandler, object sender, T e) where T : EventArgs
	{
		EventHandler<T> handler = eventHandler;

		if (handler != null)
			handler.Invoke (sender, e);
	}

	public static void Shuffle<T> (this T[] array)
	{
		new Random ().Shuffle (array);
	}

	public static void Shuffle<T> (this Random rng, T[] array)
	{
		int n = array.Length;
		while (n > 1)
		{
			int k = rng.Next (n--);
			T temp = array[n];
			array[n] = array[k];
			array[k] = temp;
		}
	}
}