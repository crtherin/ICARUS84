using Data;
using Expressions;
using UnityEngine;

namespace Procedures
{
	public class SpeedMultiplier : Process, IInitialize, IEnabled, IDisabled, IExpressionElement
	{
		[SerializeField] private readonly FloatData multiplier = new FloatData ("Multiplier", 2);

		private CharacterMotor characterMotor;

		public void Initialize ()
		{
			characterMotor = Procedure.GetComponentInParent<CharacterMotor> ();
		}
		
		public void Enabled ()
		{
			characterMotor.Speed.Multipliers.Register (this, multiplier);
		}

		public void Disabled ()
		{
			characterMotor.Speed.Multipliers.Unregister (this);
		}
	}
}