using Data;
using Damage;
using UnityEngine;

namespace Procedures
{
	public class DamageOnUpdate : Process, IInitialize, IStart, IUpdate
	{
		[SerializeField] private FloatData damage = new FloatData ("Damage", 1);
		[SerializeField] private FloatData radius = new FloatData ("Radius", 1);
		[SerializeField] private Vector2Data offset = new Vector2Data ("Offset", default(Vector2));

		private DamageHandler damageHandler;
		private bool canTrigger;

		public void Initialize ()
		{
			damageHandler = Procedure.GetComponentInParent<DamageHandler> ();
		}
		
		public void Start ()
		{
			canTrigger = true;
		}

		public void Update ()
		{
			if (!canTrigger)
				return;

			canTrigger = false;
			damageHandler.CircleDamageAround (damage, healthHandler => damageHandler.transform.up, radius, offset);
		}
	}
}