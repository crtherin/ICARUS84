using Data;
using Damage;
using UnityEngine;
using Expressions;

namespace Procedures
{
	public class MissingHealthDamageMultiplier : Process, IInitialize, IStart, IStop, IExpressionElement
	{
		[SerializeField] private StringData procedureName = new StringData ("Procedure Name", "");
		[SerializeField] private FloatData multiplierRatio = new FloatData ("Multiplier Ratio", 20);

		private ProcedureHandler procedureHandler;
		private HealthHandler healthHandler;
		private Instantiate instantiate;

		public void Initialize ()
		{
			procedureHandler = Procedure.GetComponentInParent<ProcedureHandler> ();
			healthHandler = Procedure.GetComponentInParent<HealthHandler> ();
		}

		public void Start ()
		{
			Procedure procedure = procedureHandler.GetProcedure (procedureName);
			instantiate = procedure.GetProcess<Instantiate> ();
			instantiate.Spawn += InstantiateOnSpawn;
		}

		public void Stop ()
		{
			instantiate.Spawn -= InstantiateOnSpawn;
		}

		private void InstantiateOnSpawn (object sender, InstantiateEventArgs e)
		{
			DamageHandler damageHandler = e.Instance.GetComponent<DamageHandler> ();
			damageHandler.Deal += DamageHandlerOnDeal;
		}

		private void DamageHandlerOnDeal (object sender, DamageInfo e)
		{
			float multiplier = 1 - (healthHandler.GetHealth () / healthHandler.MaxHealth) * multiplierRatio;
			e.Damage.Multipliers.Register (this, multiplier);
		}
	}
}