using System;
using System.Diagnostics;
using Damage;
using Expressions;

namespace Procedures
{
	public class DestroyOnDepleted : ChildProcess<BoxDamage>, IInitialize, IStart, IExpressionElement
	{
		private DamageHandler damageHandler;
		private BoxDamage boxDamage;

		private bool shouldDestroy;
		private float substracted;

		public void Initialize ()
		{
			Procedure.RemoveProcess<DestroyOnCollision> ();
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

		public void Start ()
		{
			substracted = 0;
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
			damageInfo.Damage.Addends.Register (this, -substracted);
			substracted += damageInfo.Target.GetHealth ();

			if (damageInfo.Target.GetHealth () > damageInfo.Damage.Get ())
				UnityEngine.Object.Destroy (Procedure.gameObject);
		}

		private void BoxDamageContinousOnCollisionEnter (object sender, CollisionInfo collisionInfo)
		{
			UnityEngine.Object.Destroy (Procedure.gameObject);
		}
	}
}