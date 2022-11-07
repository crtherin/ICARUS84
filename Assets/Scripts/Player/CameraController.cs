using Data;
using UnityEngine;

public class CameraController : DataDrivenBehaviour
{
	[SerializeField] private FloatData distance = new FloatData ("Distance", 10);
	[SerializeField] private bool snapOnStart = new BoolData("Snap on Start", true);
	
	[Header("Position Follow")]
	[SerializeField] private FloatData followMaxDistance = new FloatData("Follow Max Distance", 3f);
	[SerializeField] private FloatData followSmoothTime = new FloatData("Follow Smooth Time", 0.25f);
	[SerializeField] private FloatData followSmoothMaxSpeed = new FloatData("Follow Smooth Max Speed", 100f);

	[Header("Rotation Follow")]
	[SerializeField] private FloatData rotationSmoothTime = new FloatData("Rotation Smooth Time", 1f);
	[SerializeField] private FloatData rotationSmoothMaxSpeed = new FloatData("Rotation Smooth Max Speed", 15f);
	[SerializeField] private FloatData rotationDistance = new FloatData("Rotation Distance", 2f);
	
	[Header("Velocity Prediction")]
	[SerializeField] private FloatData predictionMultiplier = new FloatData("Prediction Multiplier", 1f);
	[SerializeField] private FloatData predictionMaxDistance = new FloatData("Prediction Max Distance", 2f);
	[SerializeField] private FloatData predictionSmoothTime = new FloatData("Prediction Smooth Time", 0.5f);
	[SerializeField] private FloatData predictionSmoothMaxSpeed = new FloatData("Prediction Smooth Max Speed", 50f);

	private Rigidbody2D target;
	
	private Vector3 followPosition;
	private Vector3 followPositionVelocity;

	private Vector3 rotationOffset;
	private Vector3 rotationOffsetVelocity;
	
	private Vector3 predictionOffset;
	private Vector3 predictionOffsetVelocity;

	protected override void Awake ()
	{
		base.Awake ();
		
		target = GameObject.FindWithTag ("Player").GetComponent<Rigidbody2D>();

		if (snapOnStart)
			followPosition = target.position;
		else
			followPosition = transform.position;

		followPosition.z = -distance;
		transform.position = followPosition;
	}

	protected void FixedUpdate ()
	{
		// Velocity Prediction
		Vector3 currentPredictionOffset = target.velocity;
		currentPredictionOffset *= predictionMultiplier;

		if (currentPredictionOffset.magnitude > predictionMaxDistance)
		{
			currentPredictionOffset = currentPredictionOffset.normalized * predictionMaxDistance;
		}
		
		predictionOffset = Vector3.SmoothDamp(
			predictionOffset,
			currentPredictionOffset,
			ref predictionOffsetVelocity,
			predictionSmoothTime,
			predictionSmoothMaxSpeed);
		
		// Rotation Follow
		Vector3 currentRotationOffset = target.transform.up * rotationDistance;
		rotationOffset = Vector3.SmoothDamp(
			rotationOffset,
			currentRotationOffset,
			ref rotationOffsetVelocity,
			rotationSmoothTime,
			rotationSmoothMaxSpeed);
		
		// Position Follow
		Vector3 currentFollowPosition = target.position;
		currentFollowPosition += predictionOffset;
		currentFollowPosition += rotationOffset;
		currentFollowPosition.z = -distance;
		
		followPosition = Vector3.SmoothDamp(
			followPosition,
			currentFollowPosition,
			ref followPositionVelocity,
			followSmoothTime,
			followSmoothMaxSpeed);

		if ((followPosition - currentFollowPosition).magnitude > followMaxDistance)
		{
			followPosition = currentFollowPosition + (followPosition - currentFollowPosition).normalized * followMaxDistance;
		}

		transform.position = followPosition;
	}
	
#if UNITY_EDITOR
	protected void OnDrawGizmos()
	{
		Color color = Color.yellow;
		color.a = 0.5f;
		Gizmos.color = color;
		Gizmos.DrawWireSphere(transform.position + Vector3.forward * distance, 0.1f);
		
		if (target != null)
		{
			color = Color.green;
			color.a = 0.5f;
			Gizmos.color = color;
			Gizmos.DrawWireSphere(target.position, 0.1f);
			
			color = Color.magenta;
			color.a = 0.5f;
			Gizmos.color = color;
			Vector3 firstPosition = target.position;
			firstPosition += rotationOffset;
			Gizmos.DrawLine(target.position, firstPosition);
			
			color = Color.cyan;
			color.a = 0.5f;
			Gizmos.color = color;
			Vector3 secondPosition = firstPosition;
			secondPosition += predictionOffset;
			Gizmos.DrawLine(firstPosition, secondPosition);
			
			color = Color.red;
			color.a = 0.5f;
			Gizmos.color = color;
			Gizmos.DrawWireSphere(secondPosition, followMaxDistance);
		}
	}
#endif
}