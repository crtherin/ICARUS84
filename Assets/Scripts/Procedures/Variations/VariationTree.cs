using System;
using UnityEngine;
using System.Collections.Generic;

namespace Procedures
{
	[Serializable]
	public class VariationTree : Mask
	{
		[SerializeField] private int selected = -1;
		[SerializeField] private List<Variation> variations = new List<Variation> ();

		public int GetVariationsCount ()
		{
			return variations.Count;
		}

		public int GetSelection ()
		{
			return selected;
		}

		public void SelectVariation (int index)
		{
			if (selected == index)
				return;

			if (index < -1)
				index = -1;
			else if (index >= variations.Count)
				index = variations.Count - 1;

			selected = index;
		}

		public Variation GetVariation (int index)
		{
			return variations[index];
		}

		public override bool ApplyMask (int id, bool isIncluded = true)
		{
			isIncluded = base.ApplyMask (id, isIncluded);

			if (selected >= 0)
				isIncluded = variations[selected].ApplyMask (id, isIncluded);

			return isIncluded;
		}

		public void SetEnabledForAll (Dictionary<Process, int> map, List<Process> enable, List<Process> disable)
		{
			foreach (Process process in map.Keys)
				process.SetEnabledFrom (this, enable, disable);
		}

		public int GetHash ()
		{
			return selected < 0
				? selected.GetHashCode ()
				: (selected.GetHashCode () ^ variations[selected].GetHash ()).RotateLeft (16);
		}

		public VariationTree Copy ()
		{
			VariationTree tree = new VariationTree ();
			CopyTo (tree);
			for (int i = 0; i < variations.Count; i++)
				tree.variations.Add (variations[i].Copy ());
			return tree;
		}


#if UNITY_EDITOR

		private bool isFolded;

		public bool IsFolded ()
		{
			return isFolded;
		}

		public void IsFolded (bool isFolded)
		{
			this.isFolded = isFolded;
		}

		public void ToggleInSelected (int id)
		{
			bool isSelected = ApplyMask (id);

			if (selected < 0)
			{
				if (isSelected)
					Exclude (id);
				else
					Include (id);

				return;
			}

			if (isSelected)
				variations[selected].ExcludeFromSelected (id);
			else
				variations[selected].IncludeInSelected (id);
		}

		public void AddVariation ()
		{
			variations.Add (new Variation ());
			selected++;
		}

		public void RemoveSelectedVariation ()
		{
			if (selected < 0 || variations.Count <= selected)
				return;

			variations.RemoveAt (selected);

			while (selected + 1 > variations.Count)
				selected--;
		}

		public void AddUpgradeToSelectedVariation ()
		{
			if (selected < 0)
				return;

			variations[selected].AddUpgrade ();
		}

		public void RemoveSelectedUpgradeFromSelectedVariation ()
		{
			if (selected < 0)
				return;

			variations[selected].RemoveSelectedUpgrade ();
		}
#endif
	}
}