using Damage;

namespace Procedures
{
	public class ResetCooldownOnKill : Process, IInitialize, IRefresh, IEnabled, IDisabled
	{
		private DamageHandler damageHandler;
		private Cooldown cooldown;

		public void Initialize ()
		{
			damageHandler = Procedure.GetComponentInParent<DamageHandler> ();
		}

		public void Refresh ()
		{
			cooldown = Procedure.GetProcess<Cooldown> ();
		}

		public void Enabled ()
		{
			damageHandler.Kill += DamageHandlerKill;
		}

		public void Disabled ()
		{
			damageHandler.Kill -= DamageHandlerKill;
		}

		private void DamageHandlerKill (object source, DamageInfo e)
		{
			cooldown.Reset ();
		}
	}
}