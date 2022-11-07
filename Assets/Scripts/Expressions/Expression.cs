using System;
using System.Collections.Generic;

namespace Expressions
{
	public class Expression<T>
	{
		public event EventHandler OnChanged;

		private T value;
		private readonly Operation<T> operation;
		private readonly Dictionary<IExpressionElement, T> sources;

		public Expression (T value, Operation<T> operation)
		{
			this.value = value;
			this.operation = operation;
			sources = new Dictionary<IExpressionElement, T> ();
		}

		public T Get ()
		{
			return value;
		}

		public void Register (IExpressionElement source, T value)
		{
			if (sources.ContainsKey (source))
				this.value = operation.Undo (this.value, sources[source]);

			this.value = operation.Do (this.value, value);
			sources.Add (source, value);
			OnChanged.SafeInvoke (this, null);
		}

		public void Unregister (IExpressionElement source)
		{
			if (!sources.ContainsKey (source))
				return;

			value = operation.Undo (value, sources[source]);
			sources.Remove (source);
			OnChanged.SafeInvoke (this, null);
		}

		public void Clear ()
		{
			foreach (IExpressionElement source in sources.Keys)
				value = operation.Undo (value, sources[source]);

			sources.Clear ();
		}
	}
}