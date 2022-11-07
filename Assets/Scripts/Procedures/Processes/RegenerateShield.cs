using Damage;
using Data;
using UnityEngine;

namespace Procedures
{
	public class RegenerateShield : Process, IInitialize, IEnabled, IPassiveUpdate
	{
		[SerializeField] private FloatData shieldCooldown = new FloatData ("Shield Cooldown", 20);
		
		private ShieldHandler shieldHandler;
		private float shieldTimer;

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
			shieldTimer = Time.time + shieldCooldown;
		}

		public void PassiveUpdate ()
		{
			if (shieldTimer < Time.time)
			{
				shieldTimer = Time.time + shieldCooldown;
				Debug.Log("Regenerating shield 1");
				shieldHandler.RegenerateShield ();
			}
		}
	}
}