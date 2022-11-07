using Data;
using UnityEngine;

namespace Procedures
{
	public class Duration : Process, IStart, IUpdate
	{
		private FloatData duration = new FloatData ("Duration", 1);
		private float finishTime;

		public void Start ()
		{
			ResetTimer ();
		}

		public void Update ()
		{
			if (Time.time > finishTime)
				Procedure.Stop ();
		}

		public float GetDuration ()
		{
			return duration;
		}

		public float RemainingTime ()
		{
			return finishTime - Time.time;
		}

		public float RemainingPercentage ()
		{
			return (finishTime - Time.time) / duration;
		}

		private void ResetTimer ()
		{
			finishTime = Time.time + duration;
		}
	}
}