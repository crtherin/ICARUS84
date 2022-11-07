using Damage;
using Object = UnityEngine.Object;

namespace Procedures
{
	public class DestroyOnCollision : ChildProcess<BoxDamage>, IInitialize
	{
		private DamageHandler damageHandler;
		private BoxDamage boxDamage;

		public void Initialize ()
		{
			damageHandler = Procedure.GetComponentInParent<DamageHandler> ();
		}

		public override void Enabled ()
		{
			base.Enabled ();
			damageHandler.Deal += DamageHandlerOnDeal;
		}

		public override void Disabled ()
		{
			base.Disabled ();
			damageHandler.Deal -= DamageHandlerOnDeal;
		}

		protected override void Register (BoxDamage parent)
		{
			parent.CollisionEnter += BoxDamageContinousOnCollisionEnter;
		}

		protected override void Unregister (BoxDamage parent)
		{
			parent.CollisionEnter -= BoxDamageContinousOnCollisionEnter;
		}

		private void DamageHandlerOnDeal (object sender, DamageInfo damageInfo)
		{
			Object.Destroy (Procedure.gameObject);
		}

		private void BoxDamageContinousOnCollisionEnter (object sender, CollisionInfo collisionInfo)
		{
			Object.Destroy (Procedure.gameObject);
		}
	}
}