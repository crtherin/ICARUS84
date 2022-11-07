using System;
using Damage;
using Data;
using Expressions;
using UnityEngine;

public class ShieldHandler : MonoBehaviour, IExpressionElement
{
	[SerializeField] private Transform shieldEffect;
	
	[SerializeField] [ShowIfPlayMode] private int shieldCount;
	
	public Int MaxShieldCount { get; private set; }
	public Flag UseShield { get; private set; }

	private HealthHandler healthHandler;
	private Animator activeEffect;

	protected void Awake ()
	{
		MaxShieldCount = new Int (this, 0);
		UseShield = new Flag ();
		UseShield.OnChanged += OnUseShieldChanged;

		healthHandler = GetComponentInParent<HealthHandler> ();
	}

	private void OnUseShieldChanged (object source, EventArgs e)
	{
		if (UseShield)
		{
			healthHandler.Receive += HealthHandlerReceive;
		}
		else
		{
			healthHandler.Receive -= HealthHandlerReceive;

			shieldCount = 0;
			
			if (activeEffect != null)
			{
				activeEffect.Play("Out");
				activeEffect = null;
			}
		}
	}

	private void HealthHandlerReceive (object source, DamageInfo e)
	{
		if (shieldCount <= 0)
			return;

		shieldCount--;
		e.Damage.OverrideBy (this, 0);

		if (activeEffect != null)
		{
			activeEffect.Play("Out");
			activeEffect = null;
			
			if (shieldCount > 0)
				TryRegenerateEffect();
		}
	}

	public int ShieldCount ()
	{
		return shieldCount;
	}

	public void RegenerateShield ()
	{
		if (shieldCount >= MaxShieldCount)
			return;
		
		shieldCount++;
		
		TryRegenerateEffect();
	}

	private void TryRegenerateEffect()
	{
		Debug.Log("Regenerating shield 2");
		
		if (shieldEffect == null || activeEffect != null)
			return;

		Debug.Log("Regenerating shield 3");
		
		Transform copy = Instantiate(shieldEffect, shieldEffect.position, shieldEffect.rotation, shieldEffect.parent);
		activeEffect = copy.gameObject.GetComponent<Animator>();
		copy.gameObject.SetActive(true);
	}
}