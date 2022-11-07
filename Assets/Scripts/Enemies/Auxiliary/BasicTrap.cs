using Damage;
using UnityEngine;

[RequireComponent (typeof(DamageHandler))]
public class BasicTrap : MonoBehaviour
{
	[SerializeField] private float damage = 5.0f;
	[SerializeField] private float radius = 0.5f;
	[SerializeField] private float initialCooldown = 1;
	[SerializeField] private LayerMask triggerMask = -1;
	[SerializeField] private bool damageOnDeath = true;

	private Animator animator;
	private new Collider2D collider;
	private HealthHandler healthHandler;
	private DamageHandler damageHandler;

	private float timer;
	private bool hasTriggered;

	protected void Awake ()
	{
		animator = GetComponent<Animator> ();
		collider = GetComponent<Collider2D> ();
		damageHandler = GetComponent<DamageHandler> ();
		healthHandler = GetComponent<HealthHandler> ();
	}

	protected void OnEnable ()
	{
		healthHandler.Death += HealthHandlerOnDeath;

		if (hasTriggered)
			Reset ();

		timer = Time.time + initialCooldown;
	}

	protected void OnDisable ()
	{
		healthHandler.Death -= HealthHandlerOnDeath;
	}

	private void HealthHandlerOnDeath (object sender, DamageInfo e)
	{
		Explode (damageOnDeath);
	}

	protected void OnTriggerEnter2D (Collider2D other)
	{
		if (hasTriggered || timer >= Time.time || triggerMask != (triggerMask | (1 << other.gameObject.layer)))
			return;

		Explode ();
	}

	private void Reset ()
	{
		hasTriggered = false;
		collider.enabled = true;
		animator.Play ("Idle");
	}

	private void Explode (bool shouldDealDamage = true)
	{
		hasTriggered = true;
		collider.enabled = false;
		animator.Play ("Explode");

		if (shouldDealDamage)
		{
			Vector3 Direction(HealthHandler target) =>
				(target.transform.position - damageHandler.transform.position).normalized;
			
			damageHandler.CircleDamageAround(damage, Direction, radius);
		}
	}

	public void AnimationEnd ()
	{
		Destroy (gameObject);
	}
}