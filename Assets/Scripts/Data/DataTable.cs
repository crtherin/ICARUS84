using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
	[Serializable]
	public partial class DataTable
	{
		public DataTable ()
		{
			Initialize ();
		}

		partial void Initialize ();
	}

	public partial class DataTable
	{
		[SerializeField] private List<IntData> intFields;
		[SerializeField] private List<FloatData> floatFields;
		[SerializeField] private List<BoolData> boolFields;
		[SerializeField] private List<StringData> stringFields;
		[SerializeField] private List<Vector2Data> vector2Fields;
		[SerializeField] private List<Vector3Data> vector3Fields;
		[SerializeField] private List<TransformData> transformFields;
		[SerializeField] private List<LayerMaskData> layerMaskFields;
		[SerializeField] private List<AudioClipData> audioClipFields;

		partial void Initialize ()
		{
			intFields = new List<IntData> ();
			floatFields = new List<FloatData> ();
			boolFields = new List<BoolData> ();
			stringFields = new List<StringData> ();
			vector2Fields = new List<Vector2Data> ();
			vector3Fields = new List<Vector3Data> ();
			transformFields = new List<TransformData> ();
			layerMaskFields = new List<LayerMaskData> ();
			audioClipFields = new List<AudioClipData>();
		}

		private T GetField<T, V> (List<T> list, string name, T defaultField) where T : DataField<V>
		{
			for (int i = 0; i < list.Count; i++)
				if (list[i].GetName () == name)
					return list[i];

			T copy = defaultField != null ? defaultField.Copy () as T : typeof (T).CreateInstance<T> ();

			if (copy != null)
				list.Add (copy);

			return copy ?? defaultField;
		}

		#region Getters

		public IntData GetIntField (string name, IntData defaultField)
		{
			return GetField<IntData, int> (intFields, name, defaultField);
		}

		public FloatData GetFloatField (string name, FloatData defaultField)
		{
			return GetField<FloatData, float> (floatFields, name, defaultField);
		}

		public BoolData GetBoolField (string name, BoolData defaultField)
		{
			return GetField<BoolData, bool> (boolFields, name, defaultField);
		}

		public StringData GetStringField (string name, StringData defaultField)
		{
			return GetField<StringData, string> (stringFields, name, defaultField);
		}

		public Vector2Data GetVector2Field (string name, Vector2Data defaultField)
		{
			return GetField<Vector2Data, Vector2> (vector2Fields, name, defaultField);
		}

		public Vector3Data GetVector3Field (string name, Vector3Data defaultField)
		{
			return GetField<Vector3Data, Vector3> (vector3Fields, name, defaultField);
		}

		public TransformData GetTransformField (string name, TransformData defaultField)
		{
			return GetField<TransformData, Transform> (transformFields, name, defaultField);
		}

		public LayerMaskData GetLayerMaskField (string name, LayerMaskData defaultField)
		{
			return GetField<LayerMaskData, LayerMask> (layerMaskFields, name, defaultField);
		}

		public AudioClipData GetAudioClipField (string name, AudioClipData defaultField)
		{
			return GetField<AudioClipData, AudioClip> (audioClipFields, name, defaultField);
		}

		public DataField[] GetAllFields ()
		{
			return intFields.Select (f => (DataField) f)
				.Concat (floatFields.Select (f => (DataField) f))
				.Concat (boolFields.Select (f => (DataField) f))
				.Concat (stringFields.Select (f => (DataField) f))
				.Concat (vector2Fields.Select (f => (DataField) f))
				.Concat (vector3Fields.Select (f => (DataField) f))
				.Concat (transformFields.Select (f => (DataField) f))
				.Concat (layerMaskFields.Select (f => (DataField) f))
				.Concat (audioClipFields.Select (f => (DataField) f))
				.ToArray ();
		}

		#endregion
	}
}