using System;
using UnityEngine;

public class AbilityTree : MonoBehaviour
{
	public event Action Refresh;

	public void OnRefresh ()
	{
		var e = Refresh;
		if (e != null)
			e.Invoke ();
	}
} 