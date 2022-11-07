using Data;
using Damage;
using UnityEngine;

namespace Procedures
{
	public class KillCharges : Process, IInitialize, IEnabled, IDisabled
	{
		[SerializeField] private IntData maxCharges = new IntData ("Max Charges", 10);
		private int charges;

		private DamageHandler damageHandler;

		public void Initialize ()
		{
			damageHandler = Procedure.GetComponentInParent<DamageHandler> ();
		}

		public void Enabled ()
		{
			damageHandler.Kill += DamageHandlerOnKill;
		}

		public void Disabled ()
		{
			damageHandler.Kill -= DamageHandlerOnKill;
		}

		private void DamageHandlerOnKill (object sender, DamageInfo damageInfo)
		{
			AddCharges (1);
		}

		public int GetCharges ()
		{
			return charges;
		}

		public void AddCharges (int charges)
		{
			this.charges += charges;

			if (maxCharges > 0 && this.charges > maxCharges)
				this.charges = maxCharges;
		}

		public int TakeCharges (int charges)
		{
			charges = Mathf.Min (this.charges, charges);
			this.charges -= charges;
			return charges;
		}

		public void TakeCharge ()
		{
			TakeCharges (1);
		}

		public void TakeAllCharges ()
		{
			charges = 0;
		}
	}
}