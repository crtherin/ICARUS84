using Data;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Procedures
{
	public class InstantiateEventArgs : EventArgs
	{
		public Transform Prefab { get; set; }
		public Transform Instance { get; set; }
	}

	public class Instantiate : Process, IInitialize, IStart, IUpdate
	{
		public event EventHandler<InstantiateEventArgs> Spawn;

		[SerializeField] private TransformData prefab = new TransformData ("Prefab", null);
		[SerializeField] private StringData pivotName = new StringData ("Pivot Name", "Pivot");

		private bool canTrigger;
		private Transform pivot;
		public void Initialize ()
		{
			pivot = Procedure.transform.Find (pivotName);
		}

		public void Start ()
		{
			canTrigger = true;
		}

		public void Update ()
		{
			if (!canTrigger)
				return;

			canTrigger = false;
			Trigger ();
		}

		// use inheritance
		public Transform Trigger ()
		{
			return SpawnPrefab (
				(pivot ?? Procedure.transform).position,
				(pivot ?? Procedure.transform).rotation);
		}

		public Transform SpawnPrefab (Vector3 position, Quaternion rotation)
		{
			InstantiateEventArgs e = new InstantiateEventArgs
			{
				Prefab = prefab,
				Instance = Object.Instantiate (prefab.Get (), position, rotation)
			};

			Spawn.SafeInvoke (this, e);
			return e.Instance;
		}
	}
}