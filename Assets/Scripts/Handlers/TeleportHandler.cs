using UnityEngine;

public class TeleportHandler : MonoBehaviour
{
	private const float DistanceCutoffTolerance = 0.1f;

	[SerializeField] private LayerMask mask = -1;
	[SerializeField] private Transform target;
	[SerializeField] private int resolution = 5;
	[SerializeField] private float distance = 5;
	[SerializeField] private float collisionRadius = 1;
	[SerializeField] private bool isRandomized = true;

	public float Distance
	{
		get { return distance; }
	}

	private Candidate[] positions;

	private struct Candidate
	{
		public readonly Vector2 Position;
		public readonly float Distance;

		public Candidate (Vector2 position, float distance)
		{
			Position = position;
			Distance = distance;
		}
	}

#if UNITY_EDITOR
	protected void OnValidate ()
	{
		if (Application.isPlaying)
		{
			if (positions != null && positions.Length != resolution)
				positions = new Candidate[resolution];
		}
	}

	protected void Reset ()
	{
		CircleCollider2D c = GetComponent<CircleCollider2D> ();

		if (c != null)
			collisionRadius = c.radius;
	}
#endif

	protected void Start ()
	{
		positions = new Candidate[resolution];
	}

	public void SetTarget (Transform target)
	{
		this.target = target;
	}

	public Vector2 Find ()
	{
		FindPossibilities (positions, target.position, collisionRadius, distance);
		positions.Shuffle ();

		Vector2 fromPosition = transform.position;

		Candidate best = new Candidate (fromPosition, 0);

		for (int i = 0; i < positions.Length; i++)
		{
			Candidate candidate = positions[i];

			if (CircleCast (fromPosition, positions[i].Position, collisionRadius))
				continue;

			if (candidate.Distance < best.Distance)
				continue;

			best = candidate;

			if (best.Distance > distance - DistanceCutoffTolerance)
				break;
		}

		return best.Position;
	}

	private void FindPossibilities (Candidate[] output, Vector3 center, float collisionRadius, float distance)
	{
		int count = output.Length;
		float angleStep = Mathf.PI * 2f / count;
		float angleOffset = isRandomized ? Random.Range (0f, angleStep) : 0;

		for (int i = 0; i < count; i++)
		{
			float angle = angleStep * i + angleOffset;
			output[i] = CircleCast (center, collisionRadius, angle, distance);
		}
	}

	private Candidate CircleCast (Vector2 center, float collisionRadius, float angle, float distance)
	{
		Vector2 direction = GetDirection (angle);
		RaycastHit2D hit = Physics2D.CircleCast (center, collisionRadius, direction, distance, mask);

		if (hit.collider == null)
			return new Candidate (center + direction * distance, distance);

		return new Candidate (hit.centroid, hit.distance);
	}

	private bool CircleCast (Vector2 from, Vector2 to, float collisionRadius)
	{
		Vector2 direction = to - from;
		return Physics2D.CircleCast (from, collisionRadius, direction, direction.magnitude, mask).collider != null;
	}

	private Vector2 GetDirection (float angle)
	{
		return new Vector2 (Mathf.Cos (angle), Mathf.Sin (angle));
	}
}