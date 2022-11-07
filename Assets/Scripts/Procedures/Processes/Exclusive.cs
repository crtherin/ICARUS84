using Data;
using UnityEngine;
using Expressions;

namespace Procedures
{
	public class Exclusive : Process, IInitialize, ICanRun, IExpressionElement
	{
		[SerializeField] private StringData procedureName = new StringData ("Procedure Name", "");

		private ProcedureHandler procedureHandler;

		public void Initialize ()
		{
			procedureHandler = Procedure.GetComponentInParent<ProcedureHandler> ();
		}

		public bool CanRun ()
		{
			return !procedureHandler.IsActive (procedureName.Get ());
		}
	}
}