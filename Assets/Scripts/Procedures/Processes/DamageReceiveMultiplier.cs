using Damage;
using Data;
using Expressions;
using UnityEngine;

namespace Procedures
{
	public class DamageReceiveMultiplier : Process, IInitialize, IStart, IStop, IExpressionElement
	{
		[SerializeField] private readonly FloatData multiplier = new FloatData ("Multiplier", 0.1f);

		private HealthHandler healthHandler;

		public void Initialize ()
		{
			healthHandler = Procedure.GetComponentInParent<HealthHandler> ();
		}

		public void Start ()
		{
			healthHandler.Receive += HealthHandlerReceive;
		}

		public void Stop ()
		{
			healthHandler.Receive -= HealthHandlerReceive;
		}

		private void HealthHandlerReceive (object source, DamageInfo e)
		{
			e.Damage.Multipliers.Register (this, multiplier);
		}
	}
}