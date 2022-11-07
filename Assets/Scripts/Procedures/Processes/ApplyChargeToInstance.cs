using Damage;
using Expressions;

namespace Procedures
{
	public class ApplyChargeToInstance : ChildProcess<Instantiate>, IExpressionElement
	{
		private Charge charge;

		protected override void Register (Instantiate parent)
		{
			parent.Spawn += ParentOnSpawn;
		}

		protected override void Unregister (Instantiate parent)
		{
			parent.Spawn -= ParentOnSpawn;
		}

		public override void Refresh ()
		{
			base.Refresh ();
			charge = Procedure.GetProcessUpwards<Charge> (this);
		}

		private void ParentOnSpawn (object sender, InstantiateEventArgs e)
		{
			Procedure procedure = e.Instance.GetComponent<Procedure> ();
			DamageHandler damageHandler = procedure.GetComponentInParent<DamageHandler> ();
			damageHandler.Deal += DamageHandlerDeal;
		}

		private void DamageHandlerDeal (object source, DamageInfo e)
		{
			e.Damage.Multipliers.Register (this, charge.GetMultiplier ());
		}
	}
}