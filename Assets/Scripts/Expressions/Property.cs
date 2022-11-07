namespace Expressions
{
	public abstract class Property<T>
	{
		private T value;

		protected Property (T value = default (T))
		{
			this.value = value;
		}

		public virtual void Set (T value)
		{
			this.value = value;
		}

		public virtual T Get ()
		{
			return value;
		}

		public static implicit operator T (Property<T> instance)
		{
			return instance.Get ();
		}
	}
}