using Data;
using Damage;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace Procedures
{
	public class BoxDamage : Process, IInitialize, IStart, IUpdate
	{
		public event EventHandler<CollisionInfo> CollisionEnter;

		[SerializeField] private FloatData damage = new FloatData ("Damage", 1);
		[SerializeField] private Vector2Data size = new Vector2Data ("Size", Vector2.one);
		[SerializeField] private FloatData angle = new FloatData ("Angle", 0);
		[SerializeField] private IntData ratePerSecond = new IntData ("Rate Per Second", 30);

		private DamageHandler damageHandler;

		private HashSet<HealthHandler> enter;
		private HashSet<HealthHandler> stay;
		private HashSet<HealthHandler> exit;
		private HashSet<Collider2D> other;

		private float timer;

		public void Initialize ()
		{
			damageHandler = Procedure.GetComponent<DamageHandler> ();

			enter = new HashSet<HealthHandler> ();
			stay = new HashSet<HealthHandler> ();
			exit = new HashSet<HealthHandler> ();
			other = new HashSet<Collider2D> ();
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

				damageHandler.OverlapBoxContinuousAround (size, angle, enter, stay, exit, other);

				foreach (HealthHandler healthHandler in enter)
				{
					damageHandler.DealDamage(damage, damageHandler.transform.up, healthHandler);
				}

				foreach (Collider2D c in other)
				{
					CollisionInfo collisionInfo = new CollisionInfo (c, c.attachedRigidbody);
					CollisionEnter.SafeInvoke (this, collisionInfo);
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