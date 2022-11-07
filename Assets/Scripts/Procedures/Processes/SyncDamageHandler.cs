using Damage;

namespace Procedures
{
	public class SyncDamageHandler : ChildProcess<Instantiate>, IInitialize
	{
		private DamageHandler damageHandler;

		public void Initialize ()
		{
			damageHandler = Procedure.GetComponentInParent<DamageHandler> ();
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
			e.Instance.GetComponent<DamageHandler> ().AddFrom (damageHandler);
		}
	}
}