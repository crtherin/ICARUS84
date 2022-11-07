using Data;
using Damage;
using UnityEngine;

namespace Procedures
{
	public class DamageOnStart : Process, IInitialize, IStart
	{
		[SerializeField] private FloatData damage = new FloatData ("Damage", 3);
		[SerializeField] private FloatData radius = new FloatData ("Radius", 3);

		private DamageHandler damageHandler;

		public void Initialize ()
		{
			damageHandler = Procedure.GetComponent<DamageHandler> ();
		}

		public void Start ()
		{
			Vector3 Direction(HealthHandler target) =>
				(target.transform.position - damageHandler.transform.position).normalized;
			
			damageHandler.CircleDamageAround (damage, Direction, radius);
		}
	}
}