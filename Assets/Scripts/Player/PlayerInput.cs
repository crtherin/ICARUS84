using Rewired;
using UnityEngine;

[RequireComponent (typeof (CharacterMotor))]
public class PlayerInput : MonoBehaviour
{
	private CharacterMotor motor;
	private Player player;

	protected void Awake ()
	{
		motor = GetComponent<CharacterMotor> ();
		player = ReInput.players.GetPlayer (0);
	}

	protected void Update ()
	{
		motor.SetMoveDirection (GetMoveInput ());
		motor.SetLookDirection (GetLookInput ());
	}

	private Vector2 GetMoveInput ()
	{
		return new Vector2 (player.GetAxisRaw ("Move Horizontal"), player.GetAxisRaw ("Move Vertical"));
	}

	private Vector2 GetLookInput ()
	{
		Vector2 r;

		if (IsJoystickConnected ())
		{
			r.x = player.GetAxis ("Aim Horizontal");
			r.y = player.GetAxis ("Aim Vertical");
		}
		else
		{
			Vector3 p = Camera.main.WorldToScreenPoint (transform.position);
			r = (Input.mousePosition - p).normalized;
		}

		return r;
	}

	private bool IsJoystickConnected ()
	{
		return false; //return player.controllers.ContainsController (ControllerType.Joystick, 0);
	}

	public Player GetRewiredPlayer ()
	{
		return player ?? ReInput.players.GetPlayer (0);
;	}
}