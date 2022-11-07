using Data;
using Damage;
using UnityEngine;
using Expressions;

namespace Procedures
{
	public class DamageStacking : Process, IInitialize, IStart, IStop, IExpressionElement
	{
		[SerializeField] private FloatData PercentageIncrease = new FloatData ("Percentage Increase", 5);

		private DamageHandler damageHandler;
		private Transform character;
		private int stacks;

		public void Initialize ()
		{
			damageHandler = Procedure.GetComponentInParent<DamageHandler> ();
			character = damageHandler.transform;
		}

		public void Start ()
		{
			damageHandler.Kill += DamageHandlerKill;
			damageHandler.Deal += DamageHandlerDeal;
			stacks = 0;
		}

		public void Stop ()
		{
			damageHandler.Kill -= DamageHandlerKill;
			damageHandler.Deal -= DamageHandlerDeal;
			stacks = 0;
			UpdateSize ();
		}

		private void DamageHandlerKill (object source, DamageInfo e)
		{
			stacks++;
			UpdateSize ();
		}

		private void DamageHandlerDeal (object source, DamageInfo e)
		{
			e.Damage.Multipliers.Register (this, GetBonus ());
		}

		private void UpdateSize ()
		{
			character.localScale = Vector3.one * GetBonus ();
		}

		private float GetBonus ()
		{
			return 1 + stacks * (PercentageIncrease / 100);
		}
	}
}