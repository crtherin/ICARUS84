using System;
using System.Collections.Generic;

namespace Expressions
{
	public class Flag : Property<bool>
	{
		public event EventHandler OnChanged;

		private readonly HashSet<IExpressionElement> sources;
		private bool lastValue;

		public Flag (bool value = false) : base (value)
		{
			sources = new HashSet<IExpressionElement> ();
			lastValue = value;
		}

		public override bool Get ()
		{
			return lastValue;
		}

		public void Flip (IExpressionElement source)
		{
			if (!sources.Contains (source))
				sources.Add (source);

			CheckForChanges ();
		}

		public void Unflip (IExpressionElement source)
		{
			if (sources.Contains (source))
				sources.Remove (source);

			CheckForChanges ();
		}

		private void CheckForChanges ()
		{
			bool newValue = sources.Count == 0 ? base.Get () : !base.Get ();

			if (lastValue == newValue)
				return;

			lastValue = newValue;

			OnChanged.SafeInvoke (this, null);
		}

		public static Flag operator + (Flag left, IExpressionElement right)
		{
			left.Flip (right);
			return left;
		}

		public static Flag operator - (Flag left, IExpressionElement right)
		{
			left.Unflip (right);
			return left;
		}
	}
}