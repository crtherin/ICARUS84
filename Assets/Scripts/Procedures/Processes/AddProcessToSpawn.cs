using Data;
using System;
using UnityEngine;

namespace Procedures
{
	public class AddProcessToSpawn : ChildProcess<Instantiate>
	{
		[SerializeField] private StringData processName = new StringData ("Process Name", "");

		protected override void Register (Instantiate parent)
		{
			parent.Spawn += ParentOnSpawn;
		}

		protected override void Unregister (Instantiate parent)
		{
			parent.Spawn -= ParentOnSpawn;
		}

		private void ParentOnSpawn (object sender, InstantiateEventArgs e)
		{
			Procedure procedure = e.Instance.GetComponent<Procedure> ();
			Type type = processName.Get ().ToType<Process> ();
			procedure.AddProcess (type);
		}
	}
}