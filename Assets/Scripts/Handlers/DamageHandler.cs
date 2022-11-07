using System;
using Damage;
using Data;
using Expressions;
using UnityEngine;

namespace Damage
{
	public class DamageInfo : EventArgs, IExpressionElement
	{
		public Float Damage { get; private set; }
		public DamageHandler Source { get; private set; }
		public HealthHandler Target { get; private set; }
		public Vector2 Direction { get; private set; }

		public DamageInfo (float damage, Vector2 direction, DamageHandler source, HealthHandler target)
		{
			Damage = new Float (this, damage);
			Direction = direction;
			Source = source;
			Target = target;
		}
	}

	public class DamageHandler : MonoBehaviour, IExpressionElement
	{
		[SerializeField] private LayerMask mask = 1 << 0;

		public event EventHandler<DamageInfo> Deal;
		public event EventHandler<DamageInfo> Kill;

		public void DealDamage (float damage, Vector3 direction, HealthHandler target, bool applyOnDeal = true)
		{
			DamageInfo damageInfo = new DamageInfo (damage, direction, this, target);
			DealDamage (damageInfo, applyOnDeal);
		}

		private bool DealDamage (DamageInfo e, bool applyOnDeal = true)
		{
			if (applyOnDeal)
				Deal.SafeInvoke (this, e);

			if (e.Target.ReceiveDamage (e))
			{
				Kill.SafeInvoke (this, e);
				return true;
			}

			return false;
		}

		public void AddFrom (DamageHandler damageHandler)
		{
			Deal += damageHandler.Deal;
			Kill += damageHandler.Kill;
		}

		public LayerMask GetMask ()
		{
			return mask;
		}
	}
}