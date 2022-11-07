using Data;
using UnityEngine;
using Expressions;
using Vector2 = UnityEngine.Vector2;

[RequireComponent (typeof (Rigidbody2D))]
public class CharacterMotor : DataDrivenBehaviour, IExpressionElement
{
	[SerializeField] private FloatData speed = new FloatData ("Speed", 5);

	[SerializeField] private FloatData acceleration = new FloatData ("Acceleration", 30);
	[SerializeField] private FloatData deceleration = new FloatData ("Deceleration", 30);

	[SerializeField] private FloatData rotationSmoothTime = new FloatData ("Rotation Smooth Time", 0.2f);
	[SerializeField] private FloatData rotationMaxSpeed = new FloatData ("Rotation Max Speed", 1000);

	public ModifiableFloatData Speed { get; private set; }
	public Flag CanMove { get; private set; } = new Flag(true);
	public Flag CanRotate { get; private set; } = new Flag(true);

	private new Rigidbody2D rigidbody;
	private new CircleCollider2D collider;

	private Vector2 moveDirection;
	private Vector2 lookDirection;

	private float angle;
	private float angleVelocity;

#if UNITY_EDITOR
	protected void OnValidate ()
	{
		if (rigidbody != null)
		{
			rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
			rigidbody.gravityScale = 0;
		}
	}
#endif

	protected override void Awake ()
	{
		base.Awake ();

		rigidbody = GetComponent<Rigidbody2D> ();
		collider = GetComponent<CircleCollider2D> ();

		Speed = new ModifiableFloatData (this, speed);
	}

	protected void Start ()
	{
		angle = transform.localEulerAngles.z;
	}

	/*protected void Update ()
	{
		//HandleRotation ();
	}*/

	protected void FixedUpdate ()
	{
		HandleRotation ();
		HandleMovement ();
	}

	private void HandleMovement ()
	{
		if (!CanMove.Get ())
			return;

		if (moveDirection.magnitude > 0.1f)
		{
			Vector2 accelerationForce = moveDirection * acceleration;
			float frameLimit = Speed / Time.fixedDeltaTime;

			if (accelerationForce.magnitude > frameLimit)
			{
				accelerationForce = accelerationForce.normalized * frameLimit;
			}
			
			rigidbody.AddForce (accelerationForce, ForceMode2D.Force);
		}
		else
		{
			Vector2 decelerationForce = -rigidbody.velocity * deceleration;
			float frameLimit = rigidbody.velocity.magnitude / Time.fixedDeltaTime;

			if (decelerationForce.magnitude > frameLimit)
			{
				decelerationForce = decelerationForce.normalized * frameLimit;
			}
			
			rigidbody.AddForce (decelerationForce, ForceMode2D.Force);
		}

		Vector2 velocity = rigidbody.velocity;

		if (velocity.magnitude > Speed)
		{
			velocity = velocity.normalized * Speed;
			rigidbody.velocity = velocity;
		}
	}

	private void HandleRotation ()
	{
		if (!CanRotate.Get ())
			return;

		float targetAngle = Mathf.Atan2 (lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90;

		angle = Mathf.SmoothDampAngle (angle, targetAngle, ref angleVelocity, rotationSmoothTime, rotationMaxSpeed);

		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	public Vector2 GetMoveDirection ()
	{
		return moveDirection.normalized;
	}

	public Vector2 GetPosition()
	{
		return rigidbody.position;
	}

	public float GetRotation ()
	{
		return rigidbody.rotation;
	}

	public float GetRadius()
	{
		return collider ? collider.radius : 0;
	}

	public void SetMoveDirection (Vector2 dir)
	{
		moveDirection = dir.normalized;
	}

	public void SetMovePosition (Vector2 pos)
	{
		moveDirection = transform.position;
		moveDirection = (pos - moveDirection).normalized;
	}
	
	public void SetVelocity (Vector2 velocity)
	{
		rigidbody.velocity = velocity;
	}

	public void SetLookDirection (Vector2 dir)
	{
		if (dir.magnitude > 1)
			dir.Normalize ();

		lookDirection = dir;
	}

	public void SetLookPosition (Vector2 pos)
	{
		lookDirection = transform.position;
		lookDirection = (pos - lookDirection).normalized;
	}

	public void MovePosition (Vector2 diff)
	{
		rigidbody.MovePosition(rigidbody.position + diff);	
	}
	
	public void ToggleCollider (bool value)
	{
		// collider.enabled = value;
		gameObject.layer = value ? LayerMask.NameToLayer("Player") : LayerMask.NameToLayer("NotPlayer");
	}
}