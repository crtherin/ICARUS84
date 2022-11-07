using Data;
using Damage;
using UnityEngine;
using Expressions;

namespace Procedures
{
	public class SetMaxHealth : Process, IInitialize, IEnabled, IDisabled, IExpressionElement
	{
		[SerializeField] private FloatData maxHealth = new FloatData ("Max Health", 1);
		
		private HealthHandler healthHandler;

		public void Initialize ()
		{
			healthHandler = Procedure.GetComponentInParent<HealthHandler> ();
		}

		public void Enabled ()
		{
			healthHandler.MaxHealth.OverrideBy (this, maxHealth);
		}

		public void Disabled ()
		{
			healthHandler.MaxHealth.Revoke (this);
		}
	}
}