using System;
using System.Collections.Generic;
using System.Linq;

namespace Procedures
{
	#region Interfaces

	public interface ICallback
	{
		bool IsEnabled ();
	}

	public interface IInitialize : ICallback
	{
		void Initialize ();
	}

	public interface IRefresh : ICallback
	{
		void Refresh ();
	}

	public interface ICanRun : ICallback
	{
		bool CanRun ();
	}

	public interface IStart : ICallback
	{
		void Start ();
	}

	public interface IUpdate : ICallback
	{
		void Update ();
	}

	public interface IWaitFor : IUpdate
	{
		bool CanContinue ();
	}

	public interface IStop : ICallback
	{
		void Stop ();
	}

	public interface IEnabled : ICallback
	{
		void Enabled ();
	}

	public interface IDisabled : ICallback
	{
		void Disabled ();
	}

	public interface IPassiveUpdate : ICallback
	{
		void PassiveUpdate ();
	}

	#endregion

	public enum ExecutionStep
	{
		Initialize,
		Refresh,
		CanRun,
		Start,
		Update,
		WaitFor,
		Stop,
		Enabled,
		Disabled,
		PassiveUpdate
	}
	
	public class ExecutionInfo : EventArgs
	{
		public Type Type { get; private set; }
		public ExecutionStep Step { get; private set; }
		
		public bool HasEnoughStamina { get; private set; }

		public ExecutionInfo(Type type, ExecutionStep step)
		{
			Type = type;
			Step = step;
		}
	}
	
	public class Callbacks
	{
		private List<IInitialize> initializeList;
		private List<IRefresh> refreshList;
		private List<ICanRun> canRunList;
		private List<IStart> startList;
		private List<IUpdate> updateList;
		private List<IWaitFor> waitForList;
		private List<IStop> stopList;
		private List<IEnabled> onEnabledList;
		private List<IDisabled> onDisabledList;
		private List<IPassiveUpdate> passiveUpdateList;

		public EventHandler<ExecutionInfo> Execution;
		
		public void Initialize ()
		{			
			if (initializeList == null)
				return;

			initializeList.ForEach (i => i.Initialize ());
		}

		public void Refresh ()
		{
			if (refreshList == null)
				return;

			foreach (IRefresh i in refreshList)
			{
				if (i.IsEnabled ())
					i.Refresh ();
			}
		}

		public bool CanRun ()
		{
			if (canRunList == null)
				return true;

			return canRunList.All (i => !i.IsEnabled () || i.CanRun ());
		}

		public void Start ()
		{
			if (startList == null)
				return;

			foreach (IStart i in startList)
			{
				if (i.IsEnabled ())
					i.Start ();
			}
		}

		public bool Update ()
		{
			if (updateList == null)
				return false;

			return updateList.Any (i =>
			{
				if (!i.IsEnabled ())
					return false;

				i.Update ();
				return ShouldWait (i);
			});
		}

		private bool ShouldWait (ICallback callback)
		{
			if (!callback.IsEnabled ())
				return false;

			if (waitForList == null)
				return false;

			IWaitFor waitFor = callback as IWaitFor;

			if (waitFor == null || !waitForList.Contains (waitFor))
				return false;

			return !waitFor.CanContinue ();
		}

		public void Stop ()
		{
			if (stopList == null)
				return;

			foreach (IStop i in stopList)
			{
				if (i.IsEnabled ())
					i.Stop ();
			}
		}

		public void PassiveUpdate ()
		{
			if (passiveUpdateList == null)
				return;

			foreach (IPassiveUpdate i in passiveUpdateList)
			{
				if (i.IsEnabled ())
					i.PassiveUpdate ();
			}
		}

		public Callbacks AddAll (ICallback callback)
		{
			Type[] interfaces = callback.GetType ().GetInterfaces ();

			for (int i = 0; i < interfaces.Length; i++)
				Add (interfaces[i], callback);

			return this;
		}

		private Callbacks Add (Type type, ICallback callback)
		{
			switch (type.Name)
			{
				case "IInitialize":
					if (initializeList == null)
						initializeList = new List<IInitialize> ();
					initializeList.Add (callback.Cast<IInitialize> ());
					break;

				case "IRefresh":
					if (refreshList == null)
						refreshList = new List<IRefresh> ();
					refreshList.Add (callback.Cast<IRefresh> ());
					break;

				case "ICanRun":
					if (canRunList == null)
						canRunList = new List<ICanRun> ();
					canRunList.Add (callback.Cast<ICanRun> ());
					break;

				case "IStart":
					if (startList == null)
						startList = new List<IStart> ();
					startList.Add (callback.Cast<IStart> ());
					break;

				case "IUpdate":
					if (updateList == null)
						updateList = new List<IUpdate> ();
					updateList.Add (callback.Cast<IUpdate> ());
					break;

				case "IWaitFor":
					if (waitForList == null)
						waitForList = new List<IWaitFor> ();
					waitForList.Add (callback.Cast<IWaitFor> ());
					break;

				case "IStop":
					if (stopList == null)
						stopList = new List<IStop> ();
					stopList.Add (callback.Cast<IStop> ());
					break;

				case "IOnEnabled":
					if (onEnabledList == null)
						onEnabledList = new List<IEnabled> ();
					onEnabledList.Add (callback.Cast<IEnabled> ());
					break;

				case "IOnDisabled":
					if (onDisabledList == null)
						onDisabledList = new List<IDisabled> ();
					onDisabledList.Add (callback.Cast<IDisabled> ());
					break;

				case "IPassiveUpdate":
					if (passiveUpdateList == null)
						passiveUpdateList = new List<IPassiveUpdate> ();
					passiveUpdateList.Add (callback.Cast<IPassiveUpdate> ());
					break;
			}

			return this;
		}

		public static Callbacks operator + (Callbacks table, ICallback callback)
		{
			table.AddAll (callback);
			return table;
		}

		/*public static Callbacks operator - (Callbacks table, ICallback callback)
		{
			return table;
		}*/
	}
}