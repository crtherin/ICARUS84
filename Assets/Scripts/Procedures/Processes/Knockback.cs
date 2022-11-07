using Data;
using Damage;
using UnityEngine;

namespace Procedures
{
	public class Knockback : Process, IInitialize, IEnabled, IDisabled
	{
		[SerializeField] private FloatData forceMultiplier = new FloatData ("Force Multiplier", 100);

		private DamageHandler damageHandler;

		public void Initialize ()
		{
			damageHandler = Procedure.GetComponentInParent<DamageHandler> ();
		}

		public void Enabled ()
		{
			damageHandler.Deal += DamageHandlerOnDeal;
		}

		public void Disabled ()
		{
			damageHandler.Deal -= DamageHandlerOnDeal;
		}

		private void DamageHandlerOnDeal (object sender, DamageInfo damageInfo)
		{
			Rigidbody2D r = damageInfo.Target.GetComponent<Rigidbody2D> ();

			if (r == null)
				return;

			r.AddForce (Procedure.transform.up * damageInfo.Damage.Get () * forceMultiplier);
		}
	}
}