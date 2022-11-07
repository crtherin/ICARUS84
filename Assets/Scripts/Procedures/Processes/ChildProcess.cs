namespace Procedures
{
	public abstract class ChildProcess<T> : Process, IRefresh, IEnabled, IDisabled
		where T : Process
	{
		protected T Parent { get; private set; }

		protected abstract void Register (T parent);
		protected abstract void Unregister (T parent);

		public virtual void Refresh ()
		{
			T newParent = Procedure.GetProcessUpwards<T> (this);

			if (Parent == null)
			{
				Parent = newParent;
			}
			else if (Parent != newParent)
			{
				Unregister (Parent);
				Parent = newParent;
			}
		}

		public virtual void Enabled ()
		{
			Register (Parent);
		}

		public virtual void Disabled ()
		{
			Unregister (Parent);
		}
	}
}