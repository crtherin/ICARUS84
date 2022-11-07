using UnityEngine;
using System.Collections.Generic;

namespace Procedures
{
	public class ProcedureHandler : MonoBehaviour
	{
		private HashSet<string> active;
		private Dictionary<string, Procedure> procedureMap;

		protected void Awake ()
		{
			active = new HashSet<string> ();
			procedureMap = procedureMap ?? new Dictionary<string, Procedure> ();
		}

		public void AddProcedure (string name, Procedure procedure)
		{
			if (procedureMap == null)
				procedureMap = new Dictionary<string, Procedure> ();

			procedureMap[name] = procedure;
		}

		public void RemoveProcedure (string name)
		{
			if (procedureMap == null || !procedureMap.ContainsKey (name))
				return;

			procedureMap.Remove (name);
		}

		public Procedure GetProcedure (string name)
		{
			if (procedureMap == null || !procedureMap.ContainsKey (name))
				return null;

			return procedureMap[name];
		}

		public Procedure GetProcedure (SerializedProcedure procedure)
		{
			return GetProcedure (procedure.GetName ());
		}

		public void AddActive (string name)
		{
			if (active.Contains (name))
				return;

			active.Add (name);
		}

		public void RemoveActive (string name)
		{
			if (!active.Contains (name))
				return;

			active.Remove (name);
		}

		public bool IsActive (string name)
		{
			return active.Contains (name);
		}
	}
}