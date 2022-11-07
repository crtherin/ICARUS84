using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Falloff = System.Func<float, float, float>;
using Random = System.Random;

namespace Damage
{
	public class CollisionInfo : EventArgs
	{
		public Collider2D Collider { get; private set; }
		public Rigidbody2D Rigidbody { get; private set; }

		public CollisionInfo (Collider2D collider, Rigidbody2D rigidbody)
		{
			Collider = collider;
			Rigidbody = Rigidbody;
		}
	}

	public static class ExtensionMethods
	{
		private static void OverlapContinuous (
			Collider2D[] colliders,
			HashSet<HealthHandler> enter,
			HashSet<HealthHandler> stay = null,
			HashSet<HealthHandler> exit = null,
			HashSet<Collider2D> other = null)
		{
			HashSet<HealthHandler> overlap = new HashSet<HealthHandler> ();

			foreach (Collider2D c in colliders)
			{
				Rigidbody2D r = c.attachedRigidbody;

				HealthHandler target = r != null ? r.GetComponent<HealthHandler> () : c.GetComponent<HealthHandler> ();

				if (target == null)
				{
					if (other != null)
						other.Add (c);
					
					continue;
				}

				overlap.Add (target);

				if (stay != null && stay.Contains (target))
					continue;

				enter.Add (target);
			}

			if (stay == null)
				return;

			if (exit == null)
				exit = new HashSet<HealthHandler> ();

			foreach (HealthHandler handler in stay.Where (h => !overlap.Contains (h)))
			{
				exit.Add (handler);
			}

			foreach (HealthHandler handler in exit)
			{
				stay.Remove (handler);
			}
		}

		public static void OverlapBoxContinuousAround (
			this DamageHandler damageHandler,
			Vector2 size,
			float angle,
			HashSet<HealthHandler> enter,
			HashSet<HealthHandler> stay = null,
			HashSet<HealthHandler> exit = null,
			HashSet<Collider2D> other = null
		)
		{
			OverlapBoxContinuousAt (damageHandler, damageHandler.transform.position, size,
				damageHandler.transform.localEulerAngles.z + angle, enter, stay, exit, other);
		}
		
		public static void OverlapBoxContinuousAround (
			this DamageHandler damageHandler,
			Vector2 size,
			float angle,
			Vector3 offset,
			HashSet<HealthHandler> enter,
			HashSet<HealthHandler> stay = null,
			HashSet<HealthHandler> exit = null,
			HashSet<Collider2D> other = null
		)
		{
			OverlapBoxContinuousAt (damageHandler, damageHandler.transform.TransformPoint (offset), size,
				damageHandler.transform.localEulerAngles.z + angle, enter, stay, exit, other);
		}

		public static void OverlapBoxContinuousAt (
			this DamageHandler damageHandler,
			Vector3 position,
			Vector2 size,
			float angle,
			HashSet<HealthHandler> enter,
			HashSet<HealthHandler> stay = null,
			HashSet<HealthHandler> exit = null,
			HashSet<Collider2D> other = null)
		{
			Collider2D[] colliders = Physics2D.OverlapBoxAll (position, size, angle, damageHandler.GetMask ());
			OverlapContinuous (colliders, enter, stay, exit, other);
		}

		public static void OverlapCircleContinuousAt (
			this DamageHandler damageHandler,
			Vector3 position,
			float radius,
			HashSet<HealthHandler> enter,
			HashSet<HealthHandler> stay = null,
			HashSet<HealthHandler> exit = null)
		{
			Collider2D[] colliders = Physics2D.OverlapCircleAll (position, radius, damageHandler.GetMask ());
			OverlapContinuous (colliders, enter, stay, exit);
		}

		public static void OverlapCircleContinuousAround (
			this DamageHandler damageHandler,
			float radius,
			HashSet<HealthHandler> enter,
			HashSet<HealthHandler> stay = null,
			HashSet<HealthHandler> exit = null)
		{
			OverlapCircleContinuousAt (damageHandler, damageHandler.transform.position, radius, enter, stay, exit);
		}

		public static void CircleDamageAt (
			this DamageHandler damageHandler,
			float damage,
			Func<HealthHandler, Vector3> direction,
			float radius,
			Vector3 position,
			Falloff falloff = null,
			bool useHandler = true)
		{
			//falloff = falloff ?? ((d, t) => damage);

			HashSet<HealthHandler> enter = new HashSet<HealthHandler> ();
			OverlapCircleContinuousAt (damageHandler, position, radius, enter);

			foreach (HealthHandler healthHandler in enter)
			{
				Vector3 target = healthHandler.transform.position;
				
				float d = falloff != null ? falloff (damage, (position - target).magnitude / radius) : damage;

				damageHandler.DealDamage (d, direction(healthHandler), healthHandler, useHandler);
			}
		}

		public static void CircleDamageAround (
			this DamageHandler damageHandler,
			float damage,
			Func<HealthHandler, Vector3> direction,
			float radius,
			Vector2 offset = default,
			bool useHandler = true)
		{
			CircleDamageAt (damageHandler, damage, direction, radius, damageHandler.transform.TransformPoint (offset), null, useHandler);
		}

		public static void CircleDamageAround (
			this DamageHandler damageHandler,
			float damage,
			Func<HealthHandler, Vector3> direction,
			float radius,
			Falloff falloff,
			bool useHandler = true)
		{
			CircleDamageAt (damageHandler, damage, direction, radius, damageHandler.transform.position, falloff, useHandler);
		}

		public static void Explode (
			this DamageInfo damageInfo,
			float maxDamage,
			float minDamage,
			float radius,
			bool useHandler = true)
		{
			ExplodeAt (damageInfo.Source, maxDamage, minDamage, damageInfo.Target.transform.position, radius, useHandler);
		}

		public static void ExplodeAround (
			this DamageHandler damageHandler,
			float maxDamage,
			float minDamage,
			float radius,
			bool useHandler = true)
		{
			Vector3 Direction(HealthHandler healthHandler) =>
				(healthHandler.transform.position - damageHandler.transform.position).normalized;

			CircleDamageAt (damageHandler, maxDamage, Direction, radius, damageHandler.transform.position,
				(damage, t) => Mathf.Lerp (damage, minDamage, t), useHandler);
		}

		public static void ExplodeAt (
			this DamageHandler damageHandler,
			float maxDamage,
			float minDamage,
			Vector3 position,
			float radius,
			bool useHandler = true)
		{
			Vector3 Direction(HealthHandler healthHandler) =>
				(healthHandler.transform.position - damageHandler.transform.position).normalized;
			
			CircleDamageAt (damageHandler, maxDamage, Direction, radius, position, (damage, t) => Mathf.Lerp (damage, minDamage, t),
				useHandler);
		}

		public static float RandomRange (this Vector2 minMax)
		{
			return UnityEngine.Random.Range (Mathf.Min (minMax.x, minMax.y), Mathf.Max (minMax.x, minMax.y));
		}
	}
}