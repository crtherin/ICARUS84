using System.Collections.Generic;

namespace Procedures
{
	public abstract class Process : ICallback
	{
		protected Procedure Procedure { get; private set; }
		private bool? enabled;
		private int? hash;

#if UNITY_EDITOR
		private bool isFolded;

		public bool IsFolded ()
		{
			return isFolded;
		}

		public void IsFolded (bool isFolded)
		{
			this.isFolded = isFolded;
		}
#endif

		public override int GetHashCode ()
		{
			return 0;
		}

		public void SetHash (int hash)
		{
			this.hash = hash;
		}

		public int? GetHash ()
		{
			return hash;
		}

		public void SetProcedure (Procedure procedure)
		{
			Procedure = procedure;
		}

		public bool IsEnabled ()
		{
			return enabled != null && enabled.Value;
		}

		public void SetEnabledFrom (VariationTree tree, List<Process> enable = null, List<Process> disable = null)
		{
			bool newEnabled = hash == null || tree == null || tree.ApplyMask (hash.Value);

			if (enabled == null || enabled != newEnabled)
			{
				if (newEnabled)
				{
					if (enable != null)
						enable.Add (this);
					else if (this is IEnabled)
						this.Cast<IEnabled> ().Enabled ();
				}
				else if (enabled != null)
				{
					if (disable != null)
						disable.Add (this);
					else if (this is IDisabled)
						this.Cast<IDisabled> ().Disabled ();
				}

				enabled = newEnabled;
			}
		}
	}

	public class None : Process
	{
	}
}