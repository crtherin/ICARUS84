using Data;
using Damage;
using Abilities;
using UnityEngine;

namespace Procedures
{
	public class RegenerateStamina : Process, IInitialize, IStart, IStop
	{
		[SerializeField] private StringData procedureName = new StringData ("Procedure Name", "");
		[SerializeField] private FloatData amount = new FloatData ("Amount", 20);

		private ProcedureHandler procedureHandler;
		private StaminaHandler staminaHandler;
		private Instantiate instantiate;

		public void Initialize ()
		{
			procedureHandler = Procedure.GetComponentInParent<ProcedureHandler> ();
			staminaHandler = Procedure.GetComponentInParent<StaminaHandler> ();
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

		private void DamageHandlerOnDeal (object sender, DamageInfo damageInfo)
		{
			staminaHandler.ReceiveStamina (amount, Procedure);
		}
	}
}