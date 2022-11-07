using Data;
using Damage;
using UnityEngine;
using System.Collections.Generic;

namespace Procedures
{
	public class BoxDamageSpread : Process, IInitialize, IStart, IUpdate
	{
		[SerializeField] private FloatData totalDamage = new FloatData ("Total Damage", 6);
		[SerializeField] private Vector2Data size = new Vector2Data ("Size", Vector2.one);
		[SerializeField] private Vector2Data offset = new Vector2Data ("Offset", Vector2.zero);
		[SerializeField] private FloatData angle = new FloatData ("Angle", 0);

		private DamageHandler damageHandler;
		private HashSet<HealthHandler> enter;

		private bool canTrigger;

		public void Initialize ()
		{
			damageHandler = Procedure.GetComponentInParent<DamageHandler> ();
			enter = new HashSet<HealthHandler> ();
		}

		public void Start ()
		{
			canTrigger = true;
		}

		public void Update ()
		{
			if (!canTrigger)
				return;

			canTrigger = false;

			damageHandler.OverlapBoxContinuousAround (size, angle, offset, enter);

			float damage = totalDamage / enter.Count;

			foreach (HealthHandler healthHandler in enter)
			{
				damageHandler.DealDamage(damage, damageHandler.transform.up, healthHandler);
			}

			enter.Clear ();
		}
	}
}