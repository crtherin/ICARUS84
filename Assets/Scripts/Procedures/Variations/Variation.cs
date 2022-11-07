using System;
using System.Collections.Generic;
using UnityEngine;

namespace Procedures
{
	[Serializable]
	public class Variation : Mask
	{
		[SerializeField] private int upgrade = 0;
		[SerializeField] private List<Mask> upgrades = new List<Mask> ();

		public int GetUpgrade ()
		{
			return upgrade;
		}

		public int GetMaxUpgrade ()
		{
			return upgrades.Count;
		}

		public void SetUpgrade (int upgrade)
		{
			this.upgrade = upgrade;
		}

		public Variation Copy ()
		{
			Variation variation = new Variation ();
			CopyTo (variation);

			for (int i = 0; i < upgrades.Count; i++)
			{
				Mask newUpgrade = new Mask ();
				upgrades[i].CopyTo (newUpgrade);
				variation.upgrades.Add (newUpgrade);
			}

			return variation;
		}

		public int GetHash ()
		{
			return upgrade.GetHashCode ();
		}

		public override bool ApplyMask (int id, bool isIncluded = true)
		{
			isIncluded = base.ApplyMask (id, isIncluded);

			for (int i = 0; i < upgrade; i++)
				isIncluded = upgrades[i].ApplyMask (id, isIncluded);

			return isIncluded;
		}

#if UNITY_EDITOR

		public void IncludeInSelected (int id)
		{
			if (upgrade < 1)
			{
				Include (id);
				return;
			}

			upgrades[upgrade - 1].Include (id);
		}

		public void ExcludeFromSelected (int id)
		{
			if (upgrade < 1)
			{
				Exclude (id);
				return;
			}

			upgrades[upgrade - 1].Exclude (id);
		}

		public void AddUpgrade ()
		{
			upgrades.Add (new Mask ());
			upgrade++;
		}

		public void RemoveSelectedUpgrade ()
		{
			if (upgrade < 1 || upgrades.Count < upgrade)
				return;

			upgrades.RemoveAt (upgrade - 1);

			while (upgrade > upgrades.Count)
				upgrade--;
		}

#endif
	}
}