using System;
using System.Collections.Generic;

namespace Expressions
{
	public class Float : SortedProperty<float>
	{
		public event EventHandler OnChanged;

		private static readonly IComparer<KeyValuePair<IExpressionElement, float>> Comparer = new FloatComparer ();

		private static readonly Operation<float> Multiplication = new Operation<float> (
			(a, b) => a * b,
			(a, b) => a / b);

		private static readonly Operation<float> Addition = new Operation<float> (
			(a, b) => a + b,
			(a, b) => a - b);

		public Float (IExpressionElement owner, float value = 0, LookUpType lookUpType = LookUpType.Lowest) :
			base (owner, value, Comparer, lookUpType)
		{
			Multipliers = new Expression<float> (1, Multiplication);
			Addends = new Expression<float> (0, Addition);

			Multipliers.OnChanged += OnChanged;
			Addends.OnChanged += OnChanged;
		}

		public Expression<float> Multipliers { get; private set; }
		public Expression<float> Addends { get; private set; }

		public override float Get ()
		{
			return base.Get () * Multipliers.Get () + Addends.Get ();
		}

		private class FloatComparer : IComparer<KeyValuePair<IExpressionElement, float>>
		{
			public int Compare (KeyValuePair<IExpressionElement, float> x, KeyValuePair<IExpressionElement, float> y)
			{
				return x.Value.CompareTo (y.Value);
			}
		}
	}
}