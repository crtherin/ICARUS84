using Damage;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Procedures
{
	public class ChainOnKill : ChildProcess<Instantiate>
	{
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
			Procedure procedure = e.Instance.GetComponent<Procedure> ();
			DamageHandler damageHandler = procedure.GetComponentInParent<DamageHandler> ();
			damageHandler.Kill += DamageHandlerOnKill;
		}

		private void DamageHandlerOnKill (object sender, DamageInfo e)
		{
			DamageHandler damageHandler = (DamageHandler) sender;
			Transform instance = damageHandler.transform;
			Parent.SpawnPrefab (instance.position, instance.rotation * Quaternion.Euler (0, 0, Random.value * 360));
		}
	}
}