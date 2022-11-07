using Data;
using System;
using UnityEngine;
using Expressions;

namespace Abilities
{
	public class StaminaInfo : EventArgs, IExpressionElement
	{
		public Float Amount { get; private set; }

		public StaminaInfo (float cost)
		{
			Amount = new Float (this, cost);
		}
	}

	public class StaminaHandler : DataDrivenBehaviour, IExpressionElement, IPersistent<float>
	{
		[SerializeField] [ShowIfPlayMode] private float stamina;
		[SerializeField] private FloatData maxStamina = new FloatData ("Max Stamina", 200);

		public event EventHandler<StaminaInfo> Take;
		public event EventHandler<StaminaInfo> Receive;

		public ModifiableFloatData MaxStamina { get; private set; }

		protected override void Awake ()
		{
			base.Awake ();

			stamina = maxStamina;
			MaxStamina = new ModifiableFloatData (this, maxStamina);
		}

		public bool TakeStamina (float value, object source = null)
		{
			StaminaInfo staminaInfo = new StaminaInfo (value);
			Take.SafeInvoke (source ?? this, staminaInfo);

			if (staminaInfo.Amount > stamina)
				return false;

			stamina -= staminaInfo.Amount;

			if (stamina < 0)
				stamina = 0;
			
			return true;
		}

		public void ReceiveStamina (float value, object source = null)
		{
			StaminaInfo staminaInfo = new StaminaInfo (value);
			Receive.SafeInvoke (source ?? this, staminaInfo);

			stamina += staminaInfo.Amount;

			if (stamina > maxStamina)
				stamina = maxStamina;
		}

		public bool HasEnoughStamina (float value)
		{
			return value <= stamina;
		}

		public float GetStamina ()
		{
			return stamina;
		}

		public float Save()
		{
			return GetStamina();
		}

		public void Load(float data)
		{
			stamina = data;
		}
	}
}