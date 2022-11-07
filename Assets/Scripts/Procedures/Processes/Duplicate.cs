using Data;
using UnityEngine;

namespace Procedures
{
	public class Duplicate : Process, IRefresh, IStart, IUpdate
	{
		[SerializeField] private FloatData rotationOffset = new FloatData ("Rotation OFfset", 0);

		private Instantiate instantiate;
		private bool canTrigger;

		public void Refresh ()
		{
			instantiate = Procedure.GetProcessUpwards<Instantiate> (this);
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

		private void Trigger ()
		{
			instantiate.Trigger ().rotation *= Quaternion.Euler (0, 0, rotationOffset);
		}
	}
}