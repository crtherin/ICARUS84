using Data;
using Damage;
using UnityEngine;

namespace Procedures
{
	public class Enrage : Process, IInitialize, ICanRun, IRefresh, IStart, IUpdate, IStop
	{
		[SerializeField] private FloatData healthPerCharge = new FloatData ("Health Per Charge", 10);
		[SerializeField] private IntData chargesPerShield = new IntData ("Charges Per Shield", 5);

		private int ticks;
		private int buffer;
		private float timer;
		private KillCharges killCharges;

		private int additionalTicks;

		private HealthHandler healthHandler;
		private ShieldHandler shieldHandler;

		private bool isUsingShield
		{
			get { return shieldHandler != null && shieldHandler.UseShield; }
		}

		private int cost
		{
			get { return !isUsingShield ? 1 : chargesPerShield; }
		}

		public bool CanRun ()
		{
			return killCharges.GetCharges () >= 1;
		}

		public void Initialize ()
		{
			healthHandler = Procedure.GetComponentInParent<HealthHandler> ();
			shieldHandler = Procedure.GetComponentInParent<ShieldHandler> ();
		}

		public void Refresh ()
		{
			killCharges = Procedure.GetProcessUpwards<KillCharges> (this);
		}
		
		public void Start ()
		{
			timer = Time.time;
			ticks = killCharges.GetCharges ();
			killCharges.TakeAllCharges ();
			additionalTicks = 0;
		}

		public void Update ()
		{
			if (timer > Time.time)
				return;

			if (Remaining () == 0)
			{
				Procedure.Stop ();
				return;
			}

			timer = Time.time + 1;

			if (ticks > 0)
			{
				ticks--;
				buffer++;
			}
			else if (additionalTicks > 0)
				additionalTicks--;

			if (buffer / cost > 0)
			{
				buffer -= cost;

				if (!isUsingShield)
					healthHandler.Heal (healthPerCharge);
				else
					shieldHandler.RegenerateShield ();
			}
		}

		public void Stop ()
		{
			ticks = 0;
			additionalTicks = 0;
		}

		public void AddTicks (int additionalTicks)
		{
			if (Remaining () <= 0)
				return;

			this.additionalTicks += additionalTicks;
		}

		public int Remaining ()
		{
			return ticks + additionalTicks;
		}
	}
}