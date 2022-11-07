using Data;
using Damage;
using Abilities;
using UnityEngine;

namespace Procedures
{
	public class EnrageImmediate : Process, IInitialize, ICanRun, IRefresh, IStart, IUpdate
	{
		[SerializeField] private FloatData healthCostPerSecond = new FloatData ("Health Cost Per Second", 5);
		[SerializeField] private FloatData staminaToHealthRatio = new FloatData ("Stamina To Health Ratio", 4);
		[SerializeField] private FloatData useStaminaTreshold = new FloatData ("Use Stamina Treshold", 10);
		[SerializeField] private FloatData healthPerCharge = new FloatData ("Health Per Charge", 15);
		[SerializeField] private FloatData staminaPerCharge = new FloatData ("Stamina Per Charge", 20);
		[SerializeField] private IntData chargesPerShield = new IntData ("Charges Per Shield", 3);

		private int ticks;
		private int buffer;
		private float timer;
		private KillCharges killCharges;

		private HealthHandler healthHandler;
		private ShieldHandler shieldHandler;
		private StaminaHandler staminaHandler;

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
			staminaHandler = Procedure.GetComponentInParent<StaminaHandler> ();
		}

		public void Refresh ()
		{
			killCharges = Procedure.GetProcessUpwards<KillCharges> (this);
		}

		public void Start ()
		{
			ticks = killCharges.GetCharges ();
			killCharges.TakeAllCharges ();
			timer = Time.time;
		}

		public void Update ()
		{
			if (killCharges.GetCharges () > 1)
			{
				killCharges.TakeCharges (1);
				buffer++;

				if (healthPerCharge > 0 && buffer >= cost)
				{
					if (!isUsingShield)
						healthHandler.Heal (healthPerCharge);
					else
						shieldHandler.RegenerateShield ();

					buffer -= cost;
				}

				if (staminaPerCharge > 0)
					staminaHandler.ReceiveStamina (staminaPerCharge, Procedure);
			}

			if (timer > Time.time)
				return;

			timer = Time.time + 1;

			if (ticks == 0)
			{
				Procedure.Stop ();
				return;
			}

			ticks--;

			float totalCost = healthCostPerSecond;

			if (healthHandler.GetHealth () > useStaminaTreshold)
			{
				float difference = healthHandler.GetHealth () - useStaminaTreshold;

				if (difference > 0)
				{
					float healthCost = Mathf.Min (totalCost, difference);

					if (healthCost > 0)
					{
						healthHandler.ReceiveDamage (healthCost, Vector3.zero);
						totalCost -= healthCost;
					}
				}
			}

			if (totalCost > 0)
			{
				if (staminaToHealthRatio > 0)
				{
					if (!staminaHandler.HasEnoughStamina (totalCost * staminaToHealthRatio))
					{
						Procedure.Stop ();
						return;
					}

					staminaHandler.TakeStamina (totalCost * staminaToHealthRatio);
				}
				else
				{
					Procedure.Stop ();
					return;
				}
			}
		}

		public float Remaining ()
		{
			return ticks + timer - Time.time;
		}
	}
}