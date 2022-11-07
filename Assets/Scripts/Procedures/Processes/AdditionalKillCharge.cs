using Data;
using Damage;
using UnityEngine;

namespace Procedures
{
	public class AdditionalKillCharge : Process, IInitialize, IRefresh, IStart, IStop
	{
		[SerializeField] private StringData procedureName = new StringData ("Procedure Name", "");

		private ProcedureHandler procedureHandler;
		private KillCharges killCharges;
		private Instantiate instantiate;

		public void Initialize ()
		{
			procedureHandler = Procedure.GetComponentInParent<ProcedureHandler> ();
		}

		public void Refresh ()
		{
			killCharges = Procedure.GetProcessUpwards<KillCharges> (this);
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
			damageHandler.Kill += DamageHandlerOnKill;
		}

		private void DamageHandlerOnKill (object sender, DamageInfo damageInfo)
		{
			killCharges.AddCharges (1);
		}
	}
}