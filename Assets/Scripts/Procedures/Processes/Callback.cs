using Data;
using UnityEngine;

namespace Procedures
{
	public class Callback : Process, IInitialize, IStart, IUpdate
	{
		[SerializeField] private StringData procedureName = new StringData ("Procedure Name", "");

		private ProcedureHandler procedureHandler;
		private bool canTrigger;

		public void Initialize ()
		{
			procedureHandler = Procedure.GetComponentInParent<ProcedureHandler> ();
		}

		public void Start ()
		{
			canTrigger = true;
		}

		public void Update ()
		{
			if (!canTrigger)
				return;

			canTrigger = false;

			Procedure procedure = procedureHandler.GetProcedure (procedureName);

			if (procedure == null)
				return;

			procedure.Callback ();
		}
	}
}