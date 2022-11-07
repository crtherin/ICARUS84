using Expressions;

namespace Data
{
	public class ModifiableFloatData : Float
	{
		private FloatData field;

		public ModifiableFloatData (
			IExpressionElement owner = null,
			FloatData field = null,
			LookUpType lookUpType = LookUpType.Lowest) : base (owner, field != null ? field.Get () : 0, lookUpType)
		{
			if (owner != null && field != null)
				SyncWith (field);
		}

		public ModifiableFloatData SyncWith (FloatData field)
		{
			this.field = field;

			if (field != null)
				Set (field);

			return this;
		}

		public override float Get ()
		{
			if (field != null)
				Set (field);

			return base.Get ();
		}

		public static implicit operator float (ModifiableFloatData data)
		{
			return data?.Get () ?? 0;
		}
	}
}