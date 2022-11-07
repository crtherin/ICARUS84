using Damage;
using Miscellaneous;
using UnityEngine;
using Pathfinding;

namespace Enemies
{
	[RequireComponent (typeof (Seeker))]
	[RequireComponent (typeof (CharacterMotor))]
	public class C4 : MonoBehaviour
	{
		[Header ("Pathfinding")]
		[SerializeField] private float pathRefreshDelay = 0.5f;
		[SerializeField] private float nextWaypointDistance = 2;
		
		[Header ("Movement")]
		[SerializeField] private Vector2 movementDistance = new Vector2 (3, 6);
		[SerializeField] private Vector2 movementCooldown = new Vector2 (2, 5);

		[Header("Laser")]
		[SerializeField] private float attackSight = 0.2f;
		[SerializeField] private float attackDelay = 1;
		[SerializeField] private float attackCircle = 1;
		[SerializeField] private float attackCooldown = 4;
		[SerializeField] private MinMax attackCooldownModifier = new MinMax(-0.4f, 0.4f);
		[SerializeField] private bool attackAggroCooldown = true;
		[SerializeField] private BasicOneShot laserTelegraph;
		[SerializeField] private Transform laserPivot;
		[SerializeField] private Transform laserPrefab;

		/*[Header ("Rotation")]
		[SerializeField] private float rotationSmoothTime = 0.25f;*/

		private Seeker seeker;
		private Transform player;
		private Camera playerCamera;
		//private Animator animator;
		private CharacterMotor motor;
		
		private Path currentPath;
		private int currentWaypoint;
		private float pathRefreshTimer;
		private LayerMask pathMask;

		private Vector3 relativeTargetPosition;
		private Vector3 randomAttackOffset;

		private float movementTimer;
		private float attackTimer;
		private bool canAttack;

		protected void Awake ()
		{
			seeker = GetComponent<Seeker> ();
			motor = GetComponent<CharacterMotor> ();
			//animator = GetComponentInChildren<Animator> ();
		}

		protected void Start ()
		{
			player = GameObject.FindWithTag ("Player").transform;
			playerCamera = FindObjectOfType<CameraController>().GetComponent<Camera>();
			pathMask = 1 << LayerMask.NameToLayer("Wall");
			
			ResetMovement ();
			//ResetAttackTimer ();
			canAttack = false;
		}

		protected void Update ()
		{
			Vector3 transformPosition = transform.position;
			Vector2 playerDirection = player.position - transformPosition;
			float playerDistance = playerDirection.magnitude;
			playerDirection.Normalize ();
			
			RaycastHit2D attackHit = Physics2D.CircleCast(
				motor.GetPosition(),
				attackSight,
				playerDirection,
				playerDistance,
				pathMask);

			bool hasAttackSight = !attackHit.collider;

			if (!hasAttackSight)
			{
				Vector3 predictedPosition =
					motor.GetPosition() + motor.GetMoveDirection() * (motor.Speed.Get() * attackDelay);
				
				RaycastHit2D predictedHit = Physics2D.CircleCast(
					predictedPosition,
					attackSight,
					playerDirection,
					playerDistance,
					pathMask);

				hasAttackSight = !predictedHit.collider;
			}

			if (hasAttackSight)
			{
				Vector3 attackDirection = (player.position + randomAttackOffset) - transform.position;
				attackDirection.Normalize();
				motor.SetLookDirection(attackDirection);

				if (!canAttack)
				{
					Vector2 viewportPosition = playerCamera.WorldToViewportPoint(transform.position);

					if (viewportPosition.x > 0 &&
					    viewportPosition.x < 1 &&
					    viewportPosition.y > 0 &&
					    viewportPosition.y < 1)
					{
						canAttack = true;
						ResetAttackTimer(false);
					}
				}

				if (canAttack && attackTimer < Time.time)
				{
					ResetAttackTimer();
					//animator.Play ("Telegraph");
					laserTelegraph.Trigger(Attack);
				}
			}
			
			RaycastHit2D pathHit = Physics2D.CircleCast(
				motor.GetPosition(),
				motor.GetRadius() / 2,
				playerDirection,
				playerDistance,
				pathMask);

			bool hasPathSight = !pathHit.collider;

			if (hasPathSight)
			{
				if (movementTimer < Time.time)
					ResetMovement();

				Vector2 targetPosition = player.position + relativeTargetPosition;
				motor.SetMovePosition(targetPosition);
			}
			else
			{
				if (Time.time > pathRefreshTimer && seeker.IsDone())
				{
					pathRefreshTimer = Time.time + pathRefreshDelay;
					seeker.StartPath(transform.position, player.position, OnPathComplete);
				}

				if (currentPath != null)
				{
					while (true)
					{
						float distanceToWaypoint = Vector3.Distance(
							transform.position,
							currentPath.vectorPath[currentWaypoint]);

						if (distanceToWaypoint < nextWaypointDistance)
						{
							if (currentWaypoint + 1 < currentPath.vectorPath.Count)
							{
								currentWaypoint++;
							}
							else
							{
								// reached end of path
								break;
							}
						}
						else
						{
							// waypoint is still too far away
							break;
						}
					}

					Vector3 pathDirection = (currentPath.vectorPath[currentWaypoint] - transform.position).normalized;

					motor.SetLookDirection(pathDirection);
					motor.SetMoveDirection(pathDirection);
				}	
			}
		}
		
		private void OnPathComplete(Path path)
		{
			path.Claim(this);

			if (!path.error)
			{
				currentPath?.Release(this);
				currentPath = path;
				currentWaypoint = 0;
			}
			else
			{
				path.Release(this);
				// Debug.LogError("Path-finding failed: " + path.error, this);
			}
		}

		private void ResetMovement ()
		{
			movementTimer = Time.time + movementCooldown.RandomRange ();
			relativeTargetPosition = Random.insideUnitCircle.normalized * movementDistance.RandomRange ();
		}

		private void ResetAttackTimer (bool useBaseCooldown = true)
		{
			attackTimer = Time.time + (useBaseCooldown ? attackCooldown : 0) + attackCooldownModifier.Random;
			randomAttackOffset = Random.insideUnitCircle * attackCircle;
		}

		private void Attack ()
		{
			Instantiate (laserPrefab, laserPivot.position, laserPivot.rotation);
		}
	}
}