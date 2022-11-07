using Data;
using UnityEngine;

namespace Procedures
{
	public class Cooldown : Process, IRefresh, ICanRun, IStop
	{
		private FloatData cooldown = new FloatData ("Cooldown", 1);
		private float nextTime;

		public void Refresh ()
		{
			Reset ();
		}

		public bool CanRun ()
		{
			return Time.time > nextTime;
		}

		public void Stop ()
		{
			Reset ();
		}

		public float RemainingTime ()
		{
			return nextTime - Time.time;
		}

		public float RemainingPercentage ()
		{
			return (nextTime - Time.time) / cooldown;
		}

		public void Reset ()
		{
			nextTime = Time.time + cooldown;
		}
	}
}