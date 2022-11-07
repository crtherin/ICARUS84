using Data;
using System;
using UnityEngine;
using Expressions;

namespace Damage
{
	public class HealInfo : EventArgs, IExpressionElement
	{
		public Float Heal { get; private set; }
		public HealthHandler Target { get; private set; }

		public HealInfo (float heal, HealthHandler target)
		{
			Heal = new Float (this, heal);
			Target = target;
		}
	}

	public class HealthHandler : DataDrivenBehaviour, IExpressionElement, IPersistent<float>
	{
		[SerializeField] [ShowIfPlayMode] private float health;
		[SerializeField] private FloatData maxHealth = new FloatData ("Max Health", 100);
		[SerializeField] private GameObject deathEffect;

		public ModifiableFloatData MaxHealth { get; private set; }

		public event EventHandler<DamageInfo> Receive;
		public event EventHandler<DamageInfo> Death;
		public event EventHandler<HealInfo> Healed;

		protected override void Awake ()
		{
			base.Awake ();
			
			health = maxHealth;
			MaxHealth = new ModifiableFloatData (this, maxHealth);
			MaxHealth.OnChanged += MaxHealthOnOnChanged;
		}

		private void MaxHealthOnOnChanged (object sender, EventArgs eventArgs)
		{
			if (MaxHealth > 0 && health > MaxHealth)
				health = MaxHealth;
		}

		public bool ReceiveDamage (float damage, Vector2 direction, DamageHandler source = null)
		{
			DamageInfo damageInfo = new DamageInfo (damage, direction, source, this);
			return ReceiveDamage (damageInfo);
		}

		public bool ReceiveDamage (DamageInfo e)
		{
			if (MaxHealth > 0 && health > MaxHealth)
				health = MaxHealth;

			if (health <= 0)
				return false;

			Receive.SafeInvoke (this, e);

			health -= e.Damage;

			if (health <= 0)
				health = 0;
			else if (MaxHealth > 0 && health > MaxHealth)
				health = MaxHealth;

			if (health > 0)
				return false;

			Death.SafeInvoke (this, e);
			
			if (deathEffect != null)
			{
				deathEffect.transform.SetParent(null);
				deathEffect.gameObject.SetActive(true);
				Destroy(transform.gameObject);
			}
			return true;
		}

		public void Heal (float heal)
		{
			HealInfo healInfo = new HealInfo (heal, this);
			Heal (healInfo);
		}

		public void Heal (HealInfo e)
		{
			Healed.SafeInvoke (this, e);

			if (health <= 0)
				health = 0;

			health += e.Heal;

			if (MaxHealth > 0 && health >= MaxHealth)
				health = MaxHealth;
		}

		public float GetHealth ()
		{
			if (health < 0)
				health = 0;
			else if (MaxHealth > 0 && health > MaxHealth)
				health = MaxHealth;

			return health;
		}

		public float Save()
		{
			return GetHealth();
		}

		public void Load(float data)
		{
			health = data;
		}
	}
}