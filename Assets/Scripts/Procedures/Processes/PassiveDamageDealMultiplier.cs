using Data;
using Damage;
using UnityEngine;
using Expressions;

namespace Procedures
{
	public class PassiveDamageDealMultiplier : Process, IInitialize, IEnabled, IDisabled, IExpressionElement
	{
		[SerializeField] private readonly FloatData multiplier = new FloatData ("Multiplier", 2);

		private DamageHandler damageHandler;
		
		public void Initialize ()
		{
			damageHandler = Procedure.GetComponentInParent<DamageHandler> ();
		}

		public void Enabled ()
		{
			damageHandler.Deal += DamageHandlerDeal;
		}

		public void Disabled ()
		{
			damageHandler.Deal -= DamageHandlerDeal;
		}

		private void DamageHandlerDeal (object source, DamageInfo e)
		{
			e.Damage.Multipliers.Register (this, multiplier);
		}
	}
}