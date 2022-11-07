using Data;
using Damage;
using UnityEngine;
using System.Collections.Generic;

namespace Procedures
{
	public class BoxDamageForHealth : Process, IInitialize, IStart, IUpdate
	{
		[SerializeField] private FloatData damage = new FloatData ("Damage", 1);
		[SerializeField] private Vector2Data size = new Vector2Data ("Size", Vector2.one);
		[SerializeField] private Vector2Data offset = new Vector2Data ("Offset", Vector2.zero);
		[SerializeField] private FloatData angle = new FloatData ("Angle", 0);
		[SerializeField] private IntData maxPenaltyCount = new IntData ("Max Penalty Count", 2);
		[SerializeField] private FloatData healthPenalty = new FloatData ("Penalty Damage", 20);
		[SerializeField] private FloatData healthPerEnemy = new FloatData ("Health Per Enemy", 10);

		private DamageHandler damageHandler;
		private HealthHandler healthHandler;

		private HashSet<HealthHandler> enter;

		private bool canTrigger;

		public void Initialize ()
		{
			damageHandler = Procedure.GetComponentInParent<DamageHandler> ();
			healthHandler = Procedure.GetComponentInParent<HealthHandler> ();

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

			Vector3 direction = damageHandler.transform.up;
			
			foreach (HealthHandler targetHealthHandler in enter)
			{
				damageHandler.DealDamage(damage, direction, targetHealthHandler);
			}

			//Debug.Log (enter.Count);

			if (enter.Count <= maxPenaltyCount)
			{
				healthHandler.ReceiveDamage(healthPenalty, Vector3.zero, damageHandler);
			}
			else
			{
				healthHandler.Heal((enter.Count - maxPenaltyCount) * healthPerEnemy);
			}

			enter.Clear ();
		}
	}
}