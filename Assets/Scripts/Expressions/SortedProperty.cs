using System.Collections.Generic;

namespace Expressions
{
	public class SortedProperty<T> : Property<T>
	{
		public enum LookUpType
		{
			Lowest,
			Highest
		}

		private readonly IExpressionElement owner;
		private readonly SortedSet<T> values;
		private LookUpType lookUpType;

		public SortedProperty (
			IExpressionElement owner,
			T value = default (T),
			IComparer<KeyValuePair<IExpressionElement, T>> comparer = null,
			LookUpType lookUpType = LookUpType.Lowest)
		{
			this.owner = owner;

			if (comparer == null)
				comparer = Comparer<KeyValuePair<IExpressionElement, T>>.Default;

			values = new SortedSet<T> (comparer);

			if (owner != null)
				values.Add (owner, value);

			this.lookUpType = lookUpType;
		}

		public void OverrideBy (IExpressionElement source, T value)
		{
			values.Add (source, value);
		}

		public void Revoke (IExpressionElement source)
		{
			if (owner == source)
				return;

			values.Remove (source);
		}

		public override void Set (T value)
		{
			if (owner != null)
				OverrideBy (owner, value);
		}

		public override T Get ()
		{
			switch (lookUpType)
			{
				default:
					return values[0];
				case LookUpType.Highest:
					return values[values.Count - 1];
			}
		}

		public void SetLookUpType (LookUpType lookUpType)
		{
			this.lookUpType = lookUpType;
		}

		public void Clear ()
		{
			values.Clear ();
		}
	}
}