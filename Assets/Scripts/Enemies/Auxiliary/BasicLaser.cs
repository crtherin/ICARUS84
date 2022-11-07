using Damage;
using UnityEngine;

public class BasicLaser : MonoBehaviour
{
	[Header ("Ray")]
	[SerializeField] private float damage = 10;
	[SerializeField] private float distance = 15;
	[SerializeField] private LayerMask mask = -1;

	[Header ("Area of Effect")]
	[SerializeField] private bool effectEnabled;
	[SerializeField] private Transform effectPrefab;

	// private Animator animator; not needed before pooling
	private DamageHandler damageHandler;

	protected void Awake ()
	{
		// animator = GetComponent<Animator> ();
		damageHandler = GetComponent<DamageHandler> ();
	}

	protected void OnEnable ()
	{
		Trigger ();
	}

	private void Trigger ()
	{
		Ray2D ray = new Ray2D (transform.position, transform.right);
		RaycastHit2D hit = Physics2D.Raycast (ray.origin, ray.direction, distance, mask);

		float dist = hit.collider ? hit.distance : distance;
		Vector3 point = ray.GetPoint (dist);

		transform.position = (transform.position + point) / 2f;

		Vector3 scale = transform.localScale;
		scale.x = dist;
		transform.localScale = scale;

		if (hit.collider)
		{
			HealthHandler targetHealth = hit.transform.GetComponentInParent<HealthHandler> ();

			if (targetHealth != null)
			{
				Vector3 Direction(HealthHandler healthHandler)
				{
					Vector3 localDirection = transform.InverseTransformPoint(healthHandler.transform.position).normalized;

					localDirection.x = Mathf.Round(localDirection.x);
					localDirection.y = Mathf.Round(localDirection.y);

					return transform.TransformDirection(localDirection);
				}
				
				damageHandler.DealDamage(damage, Direction (targetHealth), targetHealth);
			}
		}

		if (effectEnabled)
		{
			Instantiate(effectPrefab, point, Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
		}
	}

	public void AnimationEnd ()
	{
		Destroy (gameObject);
	}
}