using Data;
using Damage;
using UnityEngine;
using Expressions;

namespace Procedures
{
	public class AddTicks : Process, IInitialize, IStart, IStop, IExpressionElement
	{
		[SerializeField] private StringData procedureName = new StringData ("Procedure Name", "");
		[SerializeField] private IntData ticks = new IntData ("Ticks", 2);

		private ProcedureHandler procedureHandler;
		private Instantiate instantiate;
		private Enrage enrage;

		public void Initialize ()
		{
			procedureHandler = Procedure.GetComponentInParent<ProcedureHandler> ();
		}

		public void Start ()
		{
			enrage = Procedure.GetProcessUpwards<Enrage> (this);
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
			enrage.AddTicks (ticks);
		}
	}
}