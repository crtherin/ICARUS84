using System;
using System.Collections.Generic;
using UnityEngine;

namespace Procedures
{
	[Serializable]
	public class Mask
	{
		[SerializeField] private List<int> exclude = new List<int> ();
		[SerializeField] private List<int> include = new List<int> ();

		public void CopyTo (Mask mask)
		{
			for (int i = 0; i < exclude.Count; i++)
				if (!mask.exclude.Contains (exclude[i]))
					mask.exclude.Add (exclude[i]);

			for (int i = 0; i < include.Count; i++)
				if (!mask.include.Contains (include[i]))
					mask.include.Add (include[i]);
		}

		public bool HasIncluded (int id)
		{
			return include.Contains (id);
		}

		public bool HasExcluded (int id)
		{
			return exclude.Contains (id);
		}

		public virtual bool ApplyMask (int id, bool isIncluded = true)
		{
			if (isIncluded && exclude.Contains (id))
				return false;

			if (!isIncluded && include.Contains (id))
				return true;

			return isIncluded;
		}

#if UNITY_EDITOR

		public void Include (int id)
		{
			if (!include.Contains (id))
				include.Add (id);

			if (exclude.Contains (id))
				exclude.Remove (id);
		}

		public void Exclude (int id)
		{
			if (!exclude.Contains (id))
				exclude.Add (id);

			if (include.Contains (id))
				include.Remove (id);
		}

#endif
	}
}