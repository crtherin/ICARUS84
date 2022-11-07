using Data;
using Damage;
using UnityEngine;
using System.Collections.Generic;

namespace Procedures
{
	public class ExplodeOnKill : ChildProcess<Instantiate>
	{
		[SerializeField] private FloatData maxDamage = new FloatData ("MaxDamage", 3);
		[SerializeField] private FloatData minDamage = new FloatData ("MinDamage", 1);
		[SerializeField] private FloatData radius = new FloatData ("Radius", 3);

		protected override void Register (Instantiate parent)
		{
			parent.Spawn += ParentOnSpawn;
		}

		protected override void Unregister (Instantiate parent)
		{
			parent.Spawn -= ParentOnSpawn;
		}

		private void ParentOnSpawn (object sender, InstantiateEventArgs e)
		{
			DamageHandler damageHandler = e.Instance.GetComponent<DamageHandler> ();
			damageHandler.Kill += (senderObject, damageInfo) => { damageInfo.Explode (maxDamage, minDamage, radius, false); };
		}
	}
}