using Data;
using Damage;
using UnityEngine;

[RequireComponent (typeof (HealthHandler))]
public class Dummy : DataDrivenBehaviour
{
	[SerializeField] private Prefab projectile;
	[SerializeField] private FloatData cooldown = new FloatData ("Cooldown", 3);

	private HealthHandler healthHandler;
	private new SpriteRenderer renderer;
	private new Collider2D collider;

	private float timer;

	protected override void Awake ()
	{
		base.Awake ();
		
		renderer = GetComponent<SpriteRenderer> ();
		collider = GetComponent<Collider2D> ();
		healthHandler = GetComponent<HealthHandler> ();
	}

	protected void Start ()
	{
		healthHandler.Death += HealthHandlerOnDeath;
	}

	private void HealthHandlerOnDeath (object sender, DamageInfo e)
	{
		renderer.enabled = false;
		collider.enabled = false;
		timer = cooldown;
	}

	protected void Update ()
	{
		if (timer > 0)
		{
			if ((timer -= Time.deltaTime) <= 0)
			{
				healthHandler.Heal (10);
				renderer.enabled = true;
				collider.enabled = true;
			}
		}
	}
}