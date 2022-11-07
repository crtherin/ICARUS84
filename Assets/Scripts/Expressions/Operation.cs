using System;

namespace Expressions
{
	public class Operation<T>
	{
		public Operation (Func<T, T, T> forwards, Func<T, T, T> backward)
		{
			this.forwards = forwards;
			this.backward = backward;
		}

		private readonly Func<T, T, T> forwards;
		private readonly Func<T, T, T> backward;

		public T Do (T a, T b)
		{
			return forwards (a, b);
		}

		public T Undo (T a, T b)
		{
			return backward (a, b);
		}
	}
}