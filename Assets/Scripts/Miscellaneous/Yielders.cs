using UnityEngine;
using System.Collections.Generic;

public static class Yielders
{
	private static readonly WaitForEndOfFrame EndOfFrame = new WaitForEndOfFrame ();
	private static readonly WaitForFixedUpdate FixedUpdate = new WaitForFixedUpdate ();

	private static readonly Dictionary<float, WaitForSeconds> Seconds = new Dictionary<float, WaitForSeconds> (new FloatComparer ());
	private static readonly Dictionary<int, WaitForSeconds> Rate = new Dictionary<int, WaitForSeconds> (new IntComparer ());

	public static WaitForEndOfFrame WaitForEndOfFrame ()
	{
		return EndOfFrame;
	}

	public static WaitForFixedUpdate WaitForFixedUpdate ()
	{
		return FixedUpdate;
	}

	public static WaitForSeconds WaitForSeconds (float seconds)
	{
		WaitForSeconds value;

		if (!Seconds.TryGetValue (seconds, out value))
			Seconds.Add (seconds, value = new WaitForSeconds (seconds));

		return value;
	}

	public static WaitForSeconds RatePerSecond (int rate)
	{
		WaitForSeconds value;

		if (!Rate.TryGetValue (rate, out value))
			Rate.Add (rate, value = new WaitForSeconds (1f / rate));

		return value;
	}

	private class FloatComparer : IEqualityComparer<float>
	{
		public bool Equals (float x, float y)
		{
			return x == y;
		}

		public int GetHashCode (float obj)
		{
			return obj.GetHashCode ();
		}
	}

	private class IntComparer : IEqualityComparer<int>
	{
		public bool Equals (int x, int y)
		{
			return x == y;
		}

		public int GetHashCode (int obj)
		{
			return obj.GetHashCode ();
		}
	}
}