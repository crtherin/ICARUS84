using Data;
using Expressions;
using UnityEngine;

namespace Procedures
{
	public class Dash : Process, IInitialize, IRefresh, ICanRun, IStart, IUpdate, IStop, IExpressionElement
	{
		private FloatData distance = new FloatData ("Distance", 1);
		private FloatData phaseStartDistance = new FloatData ("Phase Start Distance", 1);
		private FloatData phaseEndDistance = new FloatData ("Phase End Distance", 3);
		private CharacterMotor motor;
		private Duration duration;
		private Vector2 velocity;
		private float percentage;
		private float currentDuration;
		private float currentTime;

		public void Initialize ()
		{
			motor = Procedure.GetComponentInParent<CharacterMotor> ();
		}

		public void Refresh ()
		{
			duration = Procedure.GetProcessUpwards<Duration> (this);
		}

		public bool CanRun ()
		{
			return motor.GetMoveDirection ().sqrMagnitude > .1f;
		}

		public void Start ()
		{
			motor.CanMove.Flip (this);
			motor.CanRotate.Flip (this);
			motor.ToggleCollider (false);

			velocity = motor.GetMoveDirection () * distance / duration.GetDuration () * 2;

			RaycastHit2D startHit = Physics2D.CircleCast(motor.GetPosition(),
				motor.GetRadius(),
				motor.GetMoveDirection(),
				distance,
				1 << LayerMask.NameToLayer("Wall"));

			if (!startHit.collider)
			{
				percentage = 1;
			}
			else if (startHit.distance > phaseStartDistance.Get())
			{
				percentage = (startHit.distance + motor.GetRadius()) / distance;
			}
			else
			{
				Vector2 destination = motor.GetPosition() + motor.GetMoveDirection() * distance;
				RaycastHit2D endHit = Physics2D.CircleCast(destination,
					motor.GetRadius(),
					-motor.GetMoveDirection(),
					distance,
					1 << LayerMask.NameToLayer("Wall"));

				if (!endHit.collider)
				{
					percentage = 1;
				}
				else
				{
					float phaseDistance = distance - endHit.distance;

					if (phaseDistance > phaseEndDistance.Get())
					{
						percentage = startHit.distance / distance;
					}
					else
					{
						percentage = 1;
					}
				}
			}
			
			// Debug.Log(percentage);
			
			currentDuration = duration.GetDuration() * percentage;
			currentTime = 0;
		}

		public void Update ()
		{
			motor.SetVelocity (Vector2.zero);
			motor.MovePosition (velocity * Time.deltaTime);
			
			if ((currentTime += Time.deltaTime) > currentDuration)
				Procedure.Stop ();
		}

		public void Stop ()
		{
			motor.CanMove.Unflip (this);
			motor.CanRotate.Unflip (this);
			motor.ToggleCollider (true);

			motor.SetVelocity (Vector2.zero);
		}
	}
}