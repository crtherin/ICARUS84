using Data;
using Damage;
using Abilities;
using UnityEngine;
using Expressions;

namespace Procedures
{
	public class UseHealthAsStamina : Process, IInitialize, IEnabled, IDisabled, IExpressionElement
	{
		[SerializeField] private FloatData ratio = new FloatData ("Ratio", 2);

		private HealthHandler healthHandler;
		private StaminaHandler staminaHandler;

		public void Initialize ()
		{
			healthHandler = Procedure.GetComponentInParent<HealthHandler> ();
			staminaHandler = Procedure.GetComponentInParent<StaminaHandler> ();
		}

		public void Enabled ()
		{
			staminaHandler.Take += StaminaHandlerOnTake;
		}

		public void Disabled ()
		{
			staminaHandler.Take -= StaminaHandlerOnTake;
		}

		private void StaminaHandlerOnTake (object sender, StaminaInfo staminaInfo)
		{
			Procedure procedure = sender as Procedure;

			if (procedure == null || procedure != Procedure)
				return;

			float difference = staminaHandler.GetStamina () - staminaInfo.Amount;

			if (difference >= 0)
				return;

			healthHandler.ReceiveDamage (-difference * ratio, Vector3.zero);
			staminaInfo.Amount.Addends.Register (this, difference);
		}
	}
}