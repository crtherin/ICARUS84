using Data;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace Damage
{
	public class BoxDamage : MonoBehaviour
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

		protected void Awake ()
		{
			damageHandler = GetComponent<DamageHandler> ();

			enter = new HashSet<HealthHandler> ();
			stay = new HashSet<HealthHandler> ();
			exit = new HashSet<HealthHandler> ();
			other = new HashSet<Collider2D> ();
		}

		protected void OnEnable ()
		{
			timer = 0;
		}

		protected void Update ()
		{
			if (timer < Time.time)
			{
				timer = Time.time + GetCooldown ();

				damageHandler.OverlapBoxContinuousAround (size, angle, enter, stay, exit, other);

				Vector3 Direction(HealthHandler healthHandler)
				{
					Vector3 localDirection = transform.InverseTransformPoint(healthHandler.transform.position).normalized;

					localDirection.x = Mathf.Round(localDirection.x);
					localDirection.y = Mathf.Round(localDirection.y);

					return transform.TransformDirection(localDirection);
				}

				foreach (HealthHandler healthHandler in enter)
				{
					damageHandler.DealDamage(damage, Direction(healthHandler), healthHandler);
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