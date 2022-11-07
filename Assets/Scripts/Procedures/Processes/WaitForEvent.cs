using System;
using UnityEngine;

namespace Procedures
{
	public class WaitForEvent : Process, IEnabled, IDisabled, IStart, IWaitFor
	{
		private int isWaiting;
		
		public void Enabled ()
		{
			Procedure.OnCallback += OnProcedureCallback;
		}

		public void Disabled ()
		{
			Procedure.OnCallback -= OnProcedureCallback;
		}

		public void Start ()
		{
			isWaiting = 1;
		}

		public void Update ()
		{
			if (isWaiting == 1)
				isWaiting = 2;
		}

		public bool CanContinue ()
		{
			return isWaiting == 0;
		}

		private void OnProcedureCallback (object sender, EventArgs e)
		{
			if (isWaiting == 2)
				isWaiting = 0;
		}
	}
}