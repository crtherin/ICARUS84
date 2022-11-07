using Miscellaneous;
using UnityEngine;
using Pathfinding;

namespace Enemies
{
	[RequireComponent (typeof(Seeker))]
	[RequireComponent (typeof(CharacterMotor))]
	public class C2 : MonoBehaviour
	{
		[Header ("Pathfinding")]
		[SerializeField] private float pathRefreshDelay = 0.5f;
		[SerializeField] private float nextWaypointDistance = 2;

		[Header ("Safe Distance")]
		[SerializeField] private float distance = 10;
		[SerializeField] private float buffer = 2;

		[Header ("Wandering")]
		[SerializeField] [Range (0, 1)] private float wanderToFleeingRatio = 0.5f;
		[SerializeField] private MinMax wanderDelay = new MinMax (0, 3);
		[SerializeField] private MinMax wanderTime = new MinMax (0, 3);

		[Header ("Teleport")]
		[SerializeField] private float teleportTriggerDistance = 5;

		[Header ("Attack")]
		[SerializeField] private float attackCooldown = 2;
		[SerializeField] private float attackDistance = 12;
		[SerializeField] private float attackSight = 0.2f;
		[SerializeField] private Transform projectile;
		[SerializeField] private Transform shootPivot;

		[Header ("Rotation")]
		[SerializeField] private float rotationSmoothTime = 0.25f;

		private Seeker seeker;
		private Animator animator;
		private Transform player;
		private CharacterMotor motor;
		private ImmunityHandler immunityHandler;
		private TeleportHandler teleportHandler;

		private Quaternion rotation;
		
		private Path currentPath;
		private int currentWaypoint;
		private float pathRefreshTimer;
		private LayerMask pathMask;

		private Vector2 reactionaryDirection;
		private Vector2 wanderDirection;
		private float wanderTimer;
		private bool isWandering;

		private float attackTimer;

		private bool isTeleporting;

		protected void Awake ()
		{
			seeker = GetComponent<Seeker> ();
			motor = GetComponent<CharacterMotor> ();
			animator = GetComponentInChildren<Animator> ();
			immunityHandler = GetComponent<ImmunityHandler> ();
			teleportHandler = GetComponent<TeleportHandler> ();
		}

		protected void Start ()
		{
			player = GameObject.FindWithTag ("Player").transform;
			pathMask = 1 << LayerMask.NameToLayer("Wall");
			
			teleportHandler.SetTarget (player);
		}

		protected void Update ()
		{
			if (!player)
				return;
			
			if (isTeleporting)
			{
				motor.SetMoveDirection (Vector2.zero);
				return;
			}

			Vector3 transformPosition = transform.position;
			Vector3 playerDirection = player.position - transformPosition;
			float playerDistance = playerDirection.magnitude;
			playerDirection.Normalize ();
			
			RaycastHit2D attackHit = Physics2D.CircleCast(
				motor.GetPosition(),
				attackSight,
				playerDirection,
				playerDistance,
				pathMask);
			
			bool hasAttackSight = !attackHit.collider;

			if (hasAttackSight)
			{
				if (playerDistance < teleportTriggerDistance)
				{
					Teleport();
					return;
				}
				
				UpdateReaction(playerDirection, playerDistance);
				UpdateWandering();

				if (playerDistance < attackDistance)
				{
					UpdateAttack();
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
				Vector3 moveDirection = Vector3.Lerp(reactionaryDirection, wanderDirection, wanderToFleeingRatio);

				motor.SetLookDirection(playerDirection);
				motor.SetMoveDirection(moveDirection);
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

		private void UpdateReaction (Vector3 dir, float dist)
		{
			if (dist < distance - buffer)
			{
				reactionaryDirection = -dir;
				return;
			}

			if (dist > distance + buffer)
			{
				reactionaryDirection = dir;
				return;
			}

			reactionaryDirection = Vector2.zero;
		}

		private void UpdateWandering ()
		{
			if (Time.time < wanderTimer)
				return;

			isWandering = !isWandering;

			if (isWandering)
			{
				wanderTimer = Time.time + wanderTime.Random;
				wanderDirection = Random.insideUnitCircle;
			}
			else
			{
				wanderTimer = Time.time + wanderDelay.Random;
				wanderDirection = Vector3.zero;
			}
		}

		private void UpdateAttack ()
		{
			if (attackTimer >= Time.time)
				return;
			
			AttackCooldownReset ();
			
			ShootProjectile ();
		}

		private void AttackCooldownReset ()
		{
			attackTimer = Time.time + attackCooldown;
		}

		private void ShootProjectile ()
		{
			Transform projectileInstance = Instantiate (projectile, shootPivot.position, shootPivot.rotation);
		}

		private void Teleport ()
		{
			isTeleporting = true;
			animator.Play ("Teleport");
			immunityHandler.Toggle (true);
		}

		public void TeleportAnimationEnd ()
		{
			isTeleporting = false;

			Vector3 targetPosition = teleportHandler.Find ();
			transform.position = targetPosition;

			immunityHandler.Toggle (false);

			AttackCooldownReset ();
		}
	}
}