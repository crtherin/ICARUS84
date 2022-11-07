using Damage;
using UnityEngine;

[RequireComponent (typeof(DamageHandler), typeof(BoxDamage))]
public class BasicProjectile : MonoBehaviour
{
	[SerializeField] private float speed = 1;
	[SerializeField] private float lifetime = 5;

	[Header ("References")]
	[SerializeField] private DamageHandler damageHandler;
	[SerializeField] private BoxDamage boxDamage;
	[SerializeField] private Animator animator;

	private float lifeTimer;
	private bool isDestroyed;

#if UNITY_EDITOR
	protected void Reset ()
	{
		Awake ();
	}
#endif

	protected void Awake ()
	{
		if (damageHandler == null)
			damageHandler = GetComponent<DamageHandler> ();

		if (boxDamage == null)
			boxDamage = GetComponent<BoxDamage> ();

		if (animator == null)
			animator = GetComponent<Animator> ();
	}

	protected void OnEnable ()
	{
		isDestroyed = false;
		boxDamage.enabled = true;
		boxDamage.CollisionEnter += BoxDamageOnCollisionEnter;
		damageHandler.Deal += DamageHandlerOnDeal;

		animator.Play ("Move");

		lifeTimer = Time.time + lifetime;
	}

	protected void OnDisable ()
	{
		boxDamage.CollisionEnter -= BoxDamageOnCollisionEnter;
		damageHandler.Deal -= DamageHandlerOnDeal;
	}

	private void BoxDamageOnCollisionEnter (object sender, CollisionInfo e)
	{
		Hit ();
	}

	private void DamageHandlerOnDeal (object sender, DamageInfo e)
	{
		Hit ();
	}

	private void Hit ()
	{
		if (isDestroyed)
			return;

		isDestroyed = true;
		boxDamage.enabled = false;
		animator.Play ("Destroy");
	}

	protected void Update ()
	{
		if (isDestroyed)
			return;

		if (Time.time > lifeTimer)
			Hit ();

		transform.position += transform.up * speed * Time.deltaTime;
	}

	public void DestroyEnd ()
	{
		Destroy (gameObject);
	}
}