using System;
using Damage;
using UnityEngine;
using Pathfinding;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Enemies
{
	[RequireComponent (typeof(Seeker))]
	[RequireComponent (typeof(CharacterMotor))]
	public class C1 : MonoBehaviour
	{
		[Header ("Pathfinding")]
		[SerializeField] private float pathRefreshDelay = 0.5f;
		[SerializeField] private float nextWaypointDistance = 2;
		
		[Header ("Attack")]
		[SerializeField] private float attackDamage = 1;
		[SerializeField] private float attackRadius = 1;
		[SerializeField] private float attackCooldown = 2; 
		[SerializeField] private float attackDistance = 1;

		[Header ("Trap")]
		[SerializeField] private Transform trap;
		[SerializeField] private float trapMinCooldown = 5;
		[SerializeField] private float trapMaxCooldown = 10;

		[Header ("Rotation")]
		[SerializeField] private float rotationSmoothTime = 0.25f;

		private Seeker seeker;
		private Transform player;
		private Animator animator;
		private CharacterMotor motor;
		private HealthHandler healthHandler;
		private DamageHandler damageHandler;

		//private Quaternion rotation;

		private Path currentPath;
		private int currentWaypoint;
		private float pathRefreshTimer;
		private LayerMask pathMask;
		
		private float attackTimer;
		private bool isAttacking;

		private float trapTimer;

		private bool isRegistered;
		private static readonly List<C1> Spawned = new List<C1>();

		protected void Awake ()
		{
			seeker = GetComponent<Seeker> ();
			motor = GetComponent<CharacterMotor> ();
			animator = GetComponentInChildren<Animator> ();
			healthHandler = GetComponent<HealthHandler>();
			damageHandler = GetComponent<DamageHandler> ();
		}

		protected void Start ()
		{
			player = GameObject.FindWithTag ("Player").transform;
			pathMask = 1 << LayerMask.NameToLayer("Wall");
			
			AttackCooldownReset ();
			TrapCooldownReset ();
			
			healthHandler.Death += HealthHandlerOnDeath;
		}

		protected void Update ()
		{
			if (!player)
				return;

			if (!isRegistered)
				Register();

			if (isAttacking)
			{
				motor.SetMoveDirection (Vector2.zero);
				return;
			}

			Vector3 transformPosition = transform.position;
			Vector3 playerDirection = player.position - transformPosition;
			float playerDistance = playerDirection.magnitude;
			playerDirection.Normalize ();

			RaycastHit2D sightHit = Physics2D.CircleCast(
				motor.GetPosition(),
				motor.GetRadius() / 2,
				playerDirection,
				playerDistance,
				pathMask);

			bool hasLineOfSight = !sightHit.collider;
			
			if (hasLineOfSight)
			{
				if (playerDistance < attackDistance)
				{
					motor.SetLookDirection (playerDirection);
					motor.SetMoveDirection (Vector2.zero);
					UpdateAttack ();
				}
				else
				{
					motor.SetLookDirection (playerDirection);
					motor.SetMoveDirection(playerDirection);
				}
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

			UpdateTrap ();
		}

		protected void OnEnable()
		{
			Register();
		}

		protected void OnDisable()
		{
			Unregister();
		}

		protected void OnDestroy()
		{
			Unregister();
		}

		private void Register()
		{
			isRegistered = true;
			
			if (Spawned.Contains(this))
				return;

			Spawned.Add(this);
		}

		private void Unregister()
		{
			isRegistered = false;
			
			if (!Spawned.Contains(this))
				return;

			Spawned.Remove(this);
		}

		private void HealthHandlerOnDeath(object sender, DamageInfo e)
		{
			Unregister();
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

		private void UpdateAttack ()
		{
			if (attackTimer >= Time.time)
				return;

			isAttacking = true;
			animator.Play ("Telegraph");
		}

		public void Attack ()
		{
			Vector3 Direction(HealthHandler target) =>
				(target.transform.position - damageHandler.transform.position).normalized;
			
			damageHandler.CircleDamageAround (attackDamage, Direction, attackRadius);
		}

		public void AttackEnd ()
		{
			isAttacking = false;
			AttackCooldownReset ();
		}

		private void AttackCooldownReset ()
		{
			attackTimer = Time.time + attackCooldown;
		}

		private void UpdateTrap ()
		{
			if (trapTimer >= Time.time)
				return;

			TrapCooldownReset ();
			PlaceTrap ();
		}

		private void TrapCooldownReset ()
		{
			trapTimer = Time.time + Random.Range (trapMinCooldown, trapMaxCooldown);
		}

		private void PlaceTrap ()
		{
			Transform trapInstance = Instantiate (trap, transform.position, transform.rotation);
		}

		public static List<C1> GetSpawned()
		{
			return Spawned;
		}
	}
}