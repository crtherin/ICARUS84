using Data;
using Damage;
using UnityEngine;
using Expressions;

namespace Procedures
{
	public class DamageDealMultiplier : ChildProcess<Instantiate>, IExpressionElement
	{
		[SerializeField] private FloatData multiplier = new FloatData ("Multiplier", 0.5f);

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
			e.Damage.Multipliers.Register (this, multiplier);
		}
	}
}