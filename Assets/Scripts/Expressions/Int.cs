using System.Collections.Generic;

namespace Expressions
{
	public class Int : SortedProperty<int>
	{
		private static readonly IComparer<KeyValuePair<IExpressionElement, int>> Comparer = new IntComparer ();

		private static readonly Operation<int> Multiplication = new Operation<int> (
			(a, b) => a * b,
			(a, b) => a / b);

		private static readonly Operation<int> Addition = new Operation<int> (
			(a, b) => a + b,
			(a, b) => a - b);

		public Int (IExpressionElement owner, int value = 0, LookUpType lookUpType = LookUpType.Lowest) :
			base (owner, value, Comparer, lookUpType)
		{
			Multipliers = new Expression<int> (1, Multiplication);
			Addends = new Expression<int> (0, Addition);
		}

		public Expression<int> Multipliers { get; private set; }
		public Expression<int> Addends { get; private set; }

		public override int Get ()
		{
			return base.Get () * Multipliers.Get () + Addends.Get ();
		}

		private class IntComparer : IComparer<KeyValuePair<IExpressionElement, int>>
		{
			public int Compare (KeyValuePair<IExpressionElement, int> x, KeyValuePair<IExpressionElement, int> y)
			{
				return x.Value.CompareTo (y.Value);
			}
		}
	}
}