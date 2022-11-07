using Data;
using Rewired;
using Abilities;
using UnityEngine;
using Expressions;

namespace Procedures
{
	public class Charge : Process, IInitialize, IRefresh, IStart, IWaitFor, IExpressionElement
	{
		[SerializeField] private FloatData maxDuration = new FloatData ("Max Duration", 1);
		[SerializeField] private FloatData maxCost = new FloatData ("Max Cost", 40);
		[SerializeField] private FloatData maxMultiplier = new FloatData ("Max Multiplier", 3);

		private string button;
		private Player player;
		private float timer;
		private bool isCharging;
		private StaminaHandler staminaHandler;
		private SpearChargeAudio spearChargeAudio;

		public void Initialize ()
		{
			staminaHandler = Procedure.GetComponentInParent<StaminaHandler> ();
			spearChargeAudio = Procedure.GetComponentInChildren<SpearChargeAudio>();
		}

		public void Refresh ()
		{
			GetButtonDown getButtonDown = Procedure.GetProcessUpwards<GetButtonDown> (this);
			button = getButtonDown.GetButton ();
			player = getButtonDown.GetPlayer ();
		}

		public void Start ()
		{
			isCharging = true;
			timer = 0;
			spearChargeAudio.SetUsingHealthAsStamina(false);
			spearChargeAudio.StartLoop();
		}

		public void Update ()
		{
			if (!isCharging)
				return;

			if (timer < maxDuration)
			{
				float deltaTime = Time.deltaTime;
				deltaTime = Mathf.Min (deltaTime, maxDuration - timer);
				timer += deltaTime;

				float cost = deltaTime / maxDuration * maxCost;

				if (staminaHandler.TakeStamina (cost, Procedure))
					timer += deltaTime;
			}

			if (player.GetButton (button))
				return;

			isCharging = false;
			spearChargeAudio.StopLoop();
		}

		public bool CanContinue ()
		{
			return !isCharging;
		}

		public float GetMultiplier ()
		{
			return Mathf.Lerp (1, maxMultiplier, timer / maxDuration);
		}
	}
}