using Data;
using System;
using Damage;
using UnityEngine;
using Expressions;

/*public class CurrencyHandler : DataDrivenBehaviour, IExpressionElement
{
	[SerializeField] private FloatData money;

	protected void Start ()
	{
		GetComponent<DamageHandler> ().Kill += delegate { money += 10; };
	}

	public float GetMoney ()
	{
		return money;
	}

	public bool CanBuy (Func<float> getCost)
	{
		return getCost () <= money;
	}

	public bool TryBuy (Func<float> getCost, Action onSuccess)
	{
		if (getCost == null)
			return false;

		float cost = getCost ();

		if (money < cost)
			return false;

		if (onSuccess == null)
			return false;

		money -= cost;
		onSuccess ();
		return true;
	}
}*/