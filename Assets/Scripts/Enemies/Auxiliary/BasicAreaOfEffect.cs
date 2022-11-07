using Damage;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasicAreaOfEffect : MonoBehaviour
{
	[SerializeField] private float damage = 1;
	[SerializeField] private float duration = 5;
	[SerializeField] private int ratePerSecond = 4;

	private Animator animator;
	private new Collider2D collider;
	private DamageHandler damageHandler;

	private Dictionary<HealthHandler, Coroutine> effects;

	protected void Awake ()
	{
		animator = GetComponent<Animator> ();
		collider = GetComponent<Collider2D> ();
		damageHandler = GetComponent<DamageHandler> ();
		effects = new Dictionary<HealthHandler, Coroutine> ();
	}

	protected void OnEnable ()
	{
		collider.enabled = false;
		animator.Play ("Appear");
	}

	protected void OnDisable ()
	{
		if (collider.enabled)
			StopCoroutine (Effect ());
	}

	public void AppearEnd ()
	{
		StartCoroutine (Effect ());
	}

	private IEnumerator Effect ()
	{
		collider.enabled = true;

		yield return Yielders.WaitForSeconds (duration);

		collider.enabled = false;
		animator.Play ("Disappear");
	}

	public void DisappearEnd ()
	{
		Destroy (gameObject);
	}

	private void OnTriggerEnter2D (Collider2D other)
	{
		TryStartEffect (GetHealthHandler (other));
	}

	private void OnTriggerExit2D (Collider2D other)
	{
		TryStopEffect (GetHealthHandler (other));
	}

	private HealthHandler GetHealthHandler (Collider2D other)
	{
		return other.transform.GetComponentInParent<HealthHandler> ();
	}

	private bool TryStartEffect (HealthHandler target)
	{
		if (target == null || effects.ContainsKey (target))
			return false;

		effects[target] = StartCoroutine (EffectLoop (target));
		return true;
	}

	private bool TryStopEffect (HealthHandler target)
	{
		if (target == null || !effects.ContainsKey (target))
			return false;

		StopCoroutine (effects[target]);
		effects.Remove (target);
		return true;
	}

	private void StopAll ()
	{
		foreach (HealthHandler target in effects.Keys)
			TryStopEffect (target);
	}

	private IEnumerator EffectLoop (HealthHandler target)
	{
		float maxKillTime = Time.time + duration;

		while (maxKillTime > Time.time)
		{
			yield return Yielders.RatePerSecond (ratePerSecond);
			damageHandler.DealDamage (damage, Vector3.zero, target);
		}
	}
}