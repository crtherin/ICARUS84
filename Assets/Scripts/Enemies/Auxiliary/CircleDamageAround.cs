using Damage;
using UnityEngine;

public class CircleDamageAround : MonoBehaviour
{
	[SerializeField] private float damage = 1.0f;
	[SerializeField] private float radius = 0.5f;

	private DamageHandler damageHandler;

	protected void Awake ()
	{
		damageHandler = GetComponent<DamageHandler> ();
	}

	public void CircleDamageAroundTrigger ()
	{
		Vector3 Direction(HealthHandler target) =>
			(target.transform.position - damageHandler.transform.position).normalized;
	
		damageHandler.CircleDamageAround (damage, Direction, radius);
	}
}