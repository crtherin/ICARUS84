using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
	[Serializable]
	public abstract class DataField
	{
		[SerializeField] private string name;
		private HashSet<DataField> connections;

		public DataField (string name)
		{
			this.name = name;
		}

		public abstract DataField GetOrCreateFieldIn (DataTable table);

		public string GetName ()
		{
			return name;
		}

		public void ConnectTo (DataField connection)
		{
			if (connections == null)
			{
				connections = new HashSet<DataField> ();
			}
			else
			{
				foreach (DataField dataField in connections)
				{
					if (dataField.connections.Contains (this))
						dataField.connections.Remove (this);
				}

				connections.Clear ();
			}

			if (connection != null)
			{
				connection.SyncTo (this);
				SyncToConnections ();

				connections.Add (connection);

				if (connection.connections == null)
					connection.connections = new HashSet<DataField> ();

				connection.connections.Add (this);
			}
		}

		public void SyncToConnections ()
		{
			if (connections == null)
				return;

			foreach (DataField connection in connections)
				SyncTo (connection);
		}

		public virtual void SyncTo (DataField connection)
		{
			if (connection == null)
				return;

			connection.name = name;
		}
	}

	[Serializable]
	public abstract class DataField<T> : DataField
	{
		[SerializeField] private T value;

		public DataField (string name) : base (name)
		{
			value = default (T);
		}

		public DataField (string name, T value) : base (name)
		{
			this.value = value;
		}

		public static implicit operator T (DataField<T> dataField)
		{
			return dataField != null ? dataField.Get () : default (T);
		}

		public DataField<T> Copy ()
		{
			return Activator.CreateInstance (GetType (), GetName (), value) as DataField<T>;
		}

		public T Get ()
		{
			return value;
		}

		public void Set (T value)
		{
			this.value = value;
		}

		public override void SyncTo (DataField connection)
		{
			if (connection == null)
				return;

			DataField<T> field = (DataField<T>) connection;

			if (!field.value.Equals (value))
				field.value = value;

			base.SyncTo (connection);
		}

		public override DataField GetOrCreateFieldIn (DataTable table)
		{
			return this;
		}
	}

	#region Generic Data Field Wrappers

	[Serializable]
	public class IntData : DataField<int>
	{
		public IntData (string name) : base (name, default (int))
		{
		}

		public IntData (string name, int value = 0) : base (name, value)
		{
		}

		public override DataField GetOrCreateFieldIn (DataTable table)
		{
			return table.GetIntField (GetName (), this);
		}
	}

	[Serializable]
	public class FloatData : DataField<float>
	{
		public FloatData (string name) : base (name, default (float))
		{
		}

		public FloatData (string name, float value = 0) : base (name, value)
		{
		}

		public override DataField GetOrCreateFieldIn (DataTable table)
		{
			return table.GetFloatField (GetName (), this);
		}
	}

	[Serializable]
	public class BoolData : DataField<bool>
	{
		public BoolData (string name) : base (name, default (bool))
		{
		}

		public BoolData (string name, bool value = false) : base (name, value)
		{
		}

		public override DataField GetOrCreateFieldIn (DataTable table)
		{
			return table.GetBoolField (GetName (), this);
		}
	}

	[Serializable]
	public class StringData : DataField<string>
	{
		public StringData (string name) : base (name, default (string))
		{
		}

		public StringData (string name, string value = default (string)) : base (name, value)
		{
		}

		public override DataField GetOrCreateFieldIn (DataTable table)
		{
			return table.GetStringField (GetName (), this);
		}
	}

	[Serializable]
	public class Vector2Data : DataField<Vector2>
	{
		public Vector2Data (string name) : base (name, default (Vector2))
		{
		}

		public Vector2Data (string name, Vector2 value = default (Vector2)) : base (name, value)
		{
		}

		public override DataField GetOrCreateFieldIn (DataTable table)
		{
			return table.GetVector2Field (GetName (), this);
		}

		public static implicit operator Vector3 (Vector2Data data)
		{
			return data.Get ();
		}
	}

	[Serializable]
	public class Vector3Data : DataField<Vector3>
	{
		public Vector3Data (string name) : base (name, default (Vector3))
		{
		}

		public Vector3Data (string name, Vector3 value = default (Vector3)) : base (name, value)
		{
		}

		public override DataField GetOrCreateFieldIn (DataTable table)
		{
			return table.GetVector3Field (GetName (), this);
		}
	}

	[Serializable]
	public class TransformData : DataField<Transform>
	{
		public TransformData (string name) : base (name, default (Transform))
		{
		}

		public TransformData (string name, Transform value = null) : base (name, value)
		{
		}

		public override DataField GetOrCreateFieldIn (DataTable table)
		{
			return table.GetTransformField (GetName (), this);
		}
	}

	[Serializable]
	public class LayerMaskData : DataField<LayerMask>
	{
		public LayerMaskData (string name) : base (name, default (LayerMask))
		{
		}

		public LayerMaskData (string name, LayerMask value = default (LayerMask)) : base (name, value)
		{
		}

		public override DataField GetOrCreateFieldIn (DataTable table)
		{
			return table.GetLayerMaskField (GetName (), this);
		}

		public static implicit operator int (LayerMaskData data)
		{
			return data.Get ();
		}
	}

	[Serializable]
	public class AudioClipData : DataField<AudioClip>
	{
		public AudioClipData(string name) : base(name, null)
		{
		}

		public AudioClipData(string name, AudioClip value = null) : base(name, value)
		{
		}

		public override DataField GetOrCreateFieldIn(DataTable table)
		{
			return table.GetAudioClipField(GetName(), this);
		}
	}

	#endregion
}