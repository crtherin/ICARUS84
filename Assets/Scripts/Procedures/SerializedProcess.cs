using UnityEngine;
using Data;
using System;
using System.Reflection;

namespace Procedures
{
	[Serializable]
	public class SerializedProcess
	{
		[SerializeField] private int hash;
		[SerializeField] private DataTable table = new DataTable ();
		[SerializeField] [ProcessSelector] private string type = typeof (None).Name;

		#if UNITY_EDITOR
		[SerializeField] private bool isFolded;
		#endif
		
		public SerializedProcess ()
		{
			hash = Guid.NewGuid ().GetHashCode ();
		}

		public int GetHash ()
		{
			return hash;
		}
		
		public new string GetType ()
		{
			return type;
		}

		public DataTable GetTable ()
		{
			return table;
		}

		public Process GetProcess ()
		{
			return type.ToType<Process> ().CreateInstance<Process> ();
		}

#if UNITY_EDITOR
		public void SetType (string type)
		{
			this.type = type;
			RebuildTable ();
		}

		public void RebuildTable ()
		{
			table = new DataTable ();

			Type processType = GetType ().ToType<Process> ();
			FieldInfo[] fields = processType.GetFields<DataField> ();

			for (int j = 0; j < fields.Length; j++)
			{
				if (fields[j] != null)
				{
					DataField dataField = fields[j].GetDefaultValue<DataField> (processType);

					if (dataField == null)
						dataField = Activator.CreateInstance (fields[j].FieldType, fields[j].Name.AddSpacing ().FirstLetterToUpper ()) as DataField;

					//if (dataField != null)
					dataField.GetOrCreateFieldIn (table);
				}
			}
		}
#endif
	}
}