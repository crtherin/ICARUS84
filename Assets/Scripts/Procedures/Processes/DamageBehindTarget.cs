using Data;
using Damage;
using UnityEngine;
using Expressions;

namespace Procedures
{
	public class DamageBehindTarget : ChildProcess<Instantiate>, IExpressionElement
	{
		[SerializeField] private FloatData damage = new FloatData ("Damage", 1);
		[SerializeField] private FloatData radius = new FloatData ("Radius", 1);
		[SerializeField] private FloatData distance = new FloatData ("Distance", 1);

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
		}

		private void DamageHandlerDeal (object source, DamageInfo e)
		{
			Vector3 Direction(HealthHandler target) =>
				(target.transform.position - e.Source.transform.position).normalized;
			
			e.Source.CircleDamageAt (damage,
				Direction,
				radius,
				e.Target.transform.position + e.Source.transform.right * distance,
				null,
				false);
		}
	}
}