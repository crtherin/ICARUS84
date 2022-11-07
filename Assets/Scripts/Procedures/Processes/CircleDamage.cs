using Data;
using Damage;
using UnityEngine;
using System.Collections.Generic;

namespace Procedures
{
	public class CircleDamage : Process, IInitialize, IStart, IUpdate
	{
		[SerializeField] private FloatData damage = new FloatData ("Damage", 3);
		[SerializeField] private FloatData radius = new FloatData ("Radius", 3);
		[SerializeField] private IntData ratePerSecond = new IntData ("Rate Per Second", 30);

		private DamageHandler damageHandler;

		private HashSet<HealthHandler> enter;
		private HashSet<HealthHandler> stay;
		private HashSet<HealthHandler> exit;

		private float timer;

		public void Initialize ()
		{
			damageHandler = Procedure.GetComponent<DamageHandler> ();

			enter = new HashSet<HealthHandler> ();
			stay = new HashSet<HealthHandler> ();
			exit = new HashSet<HealthHandler> ();
		}

		public void Start ()
		{
			timer = 0;
		}

		public void Update ()
		{
			if (timer < Time.time)
			{
				timer = Time.time + GetCooldown ();

				damageHandler.OverlapCircleContinuousAround (radius, enter, stay, exit);

				foreach (HealthHandler healthHandler in enter)
				{
					Vector3 direction = (healthHandler.transform.position - damageHandler.transform.position).normalized;
					damageHandler.DealDamage(damage, direction, healthHandler);
				}

				enter.Clear ();
				exit.Clear ();
			}
		}

		private float GetCooldown ()
		{
			return 1f / ratePerSecond;
		}
	}
}