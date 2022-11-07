using Data;
using UnityEngine;

namespace Procedures
{
	public class SelectVariation : Process, IInitialize, IRefresh, IStart, IStop
	{
		[SerializeField] private StringData procedureName = new StringData ("Procedure Name", "");
		[SerializeField] private IntData variation = new IntData ("Variation", 0);
		[SerializeField] private IntData upgrade = new IntData ("Upgrade", 0);

		private ProcedureHandler procedureHandler;
		private Procedure procedure;

		public void Initialize ()
		{
			procedureHandler = Procedure.GetComponentInParent<ProcedureHandler> ();
		}

		public void Refresh ()
		{
			if (procedureHandler == null)
				return;

			procedure = procedureHandler.GetProcedure (procedureName);
		}

		public void Start ()
		{
			if (procedure == null || procedure.GetTree () == null)
				return;

			procedure.GetTree ().SelectVariation (variation);
			procedure.GetTree ().GetVariation (variation).SetUpgrade (upgrade);
		}

		public void Stop ()
		{
			if (procedure == null || procedure.GetTree () == null)
				return;

			procedure.GetTree ().SelectVariation (0);
		}
	}
}