using Data;
using Damage;
using UnityEngine;
using Expressions;

namespace Procedures
{
	public class AddShield : Process, IInitialize, IEnabled, IDisabled, IExpressionElement
	{
		[SerializeField] private IntData count = new IntData ("Count", 1);

		private ShieldHandler shieldHandler;

		public void Initialize ()
		{
			HealthHandler healthHandler = Procedure.GetComponentInParent<HealthHandler> ();

			shieldHandler = healthHandler.GetComponent<ShieldHandler> ();

			if (shieldHandler == null)
			{
				shieldHandler = healthHandler.gameObject.AddComponent<ShieldHandler> ();
				shieldHandler.enabled = false;
			}
		}

		public void Enabled ()
		{
			shieldHandler.UseShield.Flip (this);
			shieldHandler.MaxShieldCount.Addends.Register (this, count);
		}

		public void Disabled ()
		{
			shieldHandler.UseShield.Unflip (this);
			shieldHandler.MaxShieldCount.Addends.Unregister (this);
		}
	}
}