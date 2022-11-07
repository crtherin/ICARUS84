using System;
using Data;
using Damage;
using UnityEngine;
using Expressions;

namespace Procedures
{
	public class ConsecutiveHitsDamageStacking : ChildProcess<Instantiate>, IExpressionElement
	{
		[SerializeField] private FloatData damagePerStack = new FloatData ("Damage Per Stack", 1);

		private int stacks;

		public override void Enabled ()
		{
			base.Enabled ();
			stacks = 0;
		}

		protected override void Register (Instantiate parent)
		{
			parent.Spawn += ParentOnSpawn;
		}

		protected override void Unregister (Instantiate parent)
		{
			parent.Spawn -= ParentOnSpawn;
		}

		private void ParentOnSpawn (object sender, InstantiateEventArgs e)
		{
			Procedure procedure = e.Instance.GetComponent<Procedure> ();
			DamageHandler damageHandler = procedure.GetComponentInParent<DamageHandler> ();
			damageHandler.Deal += DamageHandlerDeal;

			BoxDamage boxDamage = procedure.GetProcess<BoxDamage> ();
			boxDamage.CollisionEnter += BoxDamageOnCollisionEnter;
		}

		private void DamageHandlerDeal (object source, DamageInfo e)
		{
			e.Damage.Addends.Register (this, stacks * damagePerStack);
			stacks++;
		}

		private void BoxDamageOnCollisionEnter (object sender, CollisionInfo collisionInfo)
		{
			if (!collisionInfo.Rigidbody || !collisionInfo.Rigidbody.GetComponent<HealthHandler> ())
				stacks = 0;
		}
	}
}