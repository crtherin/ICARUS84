using System;
using Procedures;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu (menuName = "Data/Variation Tree Setting")]
public class VariationTreeSettings : ScriptableObject
{
	[SerializeField] private SerializedProcedure treeTemplate;
	[SerializeField] private List<VariationSetting> variationSettings = new List<VariationSetting> ();
	[SerializeField] private int selectedVariation;

#if UNITY_EDITOR
	public void Sync ()
	{
		if (treeTemplate == null)
		{
			variationSettings.Clear ();
			selectedVariation = -1;
			return;
		}

		VariationTree tree = treeTemplate.GetTree ();

		if (tree == null)
			return;

		for (int i = 0; i < tree.GetVariationsCount (); i++)
		{
			if (variationSettings.Count <= i)
				variationSettings.Add (new VariationSetting ());
		}
	}
#endif

	public int GetVariationCount ()
	{
		return variationSettings.Count;
	}

	public void SelectVariation (int variation)
	{
		if (variation >= GetVariationCount ())
			return;

		selectedVariation = variation;
	}

	public int SelectedVariation ()
	{
		return selectedVariation;
	}

	public VariationSetting GetVariationSetting (int variation)
	{
		if (variation < 0 || variation >= GetVariationCount ())
			return null;

		return variationSettings[variation];
	}

	public void ApplyTo (VariationTree tree)
	{
		if (tree == null)
			return;

		tree.SelectVariation (selectedVariation);

		VariationSetting setting = GetVariationSetting (selectedVariation);

		if (setting == null)
			return;

		setting.ApplyTo (tree.GetVariation (selectedVariation));
	}

	[Serializable]
	public class VariationSetting
	{
		[SerializeField] private int selectedUpgrade;

		public int SelectedUpgrade ()
		{
			return selectedUpgrade;
		}

		public void SelectUpgrade (int upgrade)
		{
			selectedUpgrade = upgrade;
		}

		public void ApplyTo (Variation variation)
		{
			if (variation == null)
				return;

			variation.SetUpgrade (selectedUpgrade);
		}
	}
}