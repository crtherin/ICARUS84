using Damage;
using Expressions;
using UnityEngine;

public class ImmunityHandler : MonoBehaviour, IExpressionElement
{
	[SerializeField] private bool isImmune;
	private HealthHandler healthHandler;

	protected void Awake ()
	{
		healthHandler = GetComponent<HealthHandler> ();
	}

	protected void OnEnable ()
	{
		healthHandler.Receive += HealthHandlerOnReceive;
	}

	private void OnDisable ()
	{
		healthHandler.Receive -= HealthHandlerOnReceive;
	}

	private void HealthHandlerOnReceive (object sender, DamageInfo e)
	{
		if (!isImmune)
			return;

		e.Damage.OverrideBy (this, 0);
	}

	public void Toggle (bool isImmune)
	{
		this.isImmune = isImmune;
	}
}