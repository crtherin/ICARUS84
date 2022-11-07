using Damage;
using Pathfinding;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Enemies
{
    public class C3 : MonoBehaviour
    {
        private enum MoveState
        {
	        _0m_01 = 0, // left
            _01_0m = 1, // forward
            _0m_02 = 2, // right
            _02_0m = 3, // forward
        }

        [Header("Movement")]
        [SerializeField] private float forwardDistance = 3;
        [SerializeField] private float sideDistance = 1;
        [SerializeField] private float forwardTime = 1;
        [SerializeField] private float sideTime = 1;
        [SerializeField] private AnimationCurve easeCurve;
        
        [Header ("Wandering")]
        //[SerializeField] private MinMax wanderDelay = new MinMax (0, 3);
        [SerializeField] private Vector2 wanderTime = new Vector2(4, 8);
        [SerializeField] private Vector2 desiredDistance = new Vector2(2, 10);
        [SerializeField] private float minDistance = 1;
        [SerializeField] private float rotationThreshold = 10;
        [SerializeField] private float rotationDuration = 1;

        [Header ("Pathfinding")]
        [SerializeField] private float pathRefreshDelay = 0.5f;
        [SerializeField] private float nextWaypointDistance = 2;

        [Header("Spawning")]
        [SerializeField] private Vector2 spawnCooldown = new Vector2(5, 7);
        [SerializeField] private Transform spawnOrigin;
        [SerializeField] private Transform spawnPrefab;
        [SerializeField] private Vector2 spawnOffset;
        [SerializeField] private Transform teleporter;

        [Header("Animation")]
        [SerializeField] private C3AnimationHandler animationHandler;
        
        private Seeker seeker;
        private Rigidbody2D body;
        private Transform player;
        private HealthHandler healthHandler;

        private Path currentPath;
        private int currentWaypoint;
        private float pathRefreshTimer;
        private LayerMask pathMask;

        private float stateTimer;
        private float spawnTimer;
        private float wanderTimer;
        private MoveState moveState;
        private Vector2 currentTarget;
        private bool isPathfinding;
        private bool canSpawn;
        
        private bool isRegistered;
        private static readonly HashSet<C3> Spawned = new HashSet<C3>();

        protected void Awake ()
        {
	        body = GetComponent<Rigidbody2D>();
            seeker = GetComponent<Seeker> ();
            healthHandler = GetComponent<HealthHandler>();
        }

        protected void Start ()
        {
            player = GameObject.FindWithTag ("Player").transform;
            pathMask = 1 << LayerMask.NameToLayer("Wall");
            
            healthHandler.Death += HealthHandlerOnDeath;

            if (!C3Teleporter.HasInstance())
	            Instantiate(teleporter);

            StartCoroutine(Wait());
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
        
        private IEnumerator Wait()
        {
	        while (!player)
	        {
		        Freeze();
		        yield return null;
	        }

	        SpawnCooldownReset();

	        moveState = MoveState._0m_01;
	        BranchState();
        }

        private void Freeze()
        {
	        body.velocity = Vector2.zero;
	        body.angularVelocity = 0;
        }

        private bool HasSight()
        {
	        Vector3 transformPosition = transform.position;
	        Vector3 playerDirection = player.position - transformPosition;
	        float playerDistance = playerDirection.magnitude;
	        playerDirection.Normalize ();
			
	        RaycastHit2D attackHit = Physics2D.CircleCast(
		        body.position,
		        0.5f,
		        playerDirection,
		        playerDistance,
		        pathMask);

	        return !attackHit.collider;
        }
        
        private void CheckWander()
        {
	        float distance = (currentTarget - body.position).magnitude;

	        if (isPathfinding)
		        isPathfinding = false;
	        else if (wanderTimer > Time.time && distance > minDistance)
		        return;
	        
	        wanderTimer = Time.time + wanderTime.MinMaxRandom();
	        
	        Vector2 randomDir2D = Random.insideUnitCircle;
	        Vector3 randomDir = new Vector3(randomDir2D.x, randomDir2D.y);
	        float randomDist = desiredDistance.MinMaxRandom();

	        currentTarget = player.position + randomDir.normalized * randomDist;
        }

        private void BranchState()
        {
	        // Freeze();
	        
	        if (!isRegistered)
		        Register();

	        if (canSpawn && spawnTimer < Time.time)
	        {
		        SpawnCooldownReset();
		        StartCoroutine(Spawn());
		        return;
	        }
	        
	        if (HasSight())
	        {
		        CheckWander();
	        }
	        else
	        {
		        if (!isPathfinding)
			        isPathfinding = true;
		        
		        UpdatePathfinding();
	        }

	        if (Vector3.Angle(transform.up, currentTarget - body.position) > rotationThreshold)
	        {
		        StartCoroutine(RotateToTarget());
		        return;
	        }
	        
	        switch (moveState)
	        {
		        case MoveState._0m_01: // left
		        case MoveState._0m_02: // right
			        canSpawn = true;
			        
			        Vector3 sDir = moveState == MoveState._0m_01 
				        ? Vector3.Cross(Vector3.forward, transform.up) 
				        : Vector3.Cross(Vector3.back, transform.up);
			        Vector2 sDir2D = new Vector2(sDir.x, sDir.y);
			        
			        StartCoroutine(MoveToPosition(body.position + sDir2D * sideDistance, sideTime, 
				        moveState == MoveState._0m_01 
					        ? "0m-01" 
					        : "0m-02"));
			        break;

		        case MoveState._01_0m: // forward
		        case MoveState._02_0m: // forward
			        canSpawn = false;
			        
			        Vector3 fDir = transform.up;
			        Vector2 fDir2D = new Vector2(fDir.x, fDir.y);
			        
			        StartCoroutine(MoveToPosition(body.position + fDir2D * forwardDistance, forwardTime, 
				        moveState == MoveState._01_0m 
					        ? "01-0m" 
					        : "02-0m"));
			        break;
	        }

	        moveState = (MoveState) (((int) moveState + 1) % 4);
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
        
        private void UpdatePathfinding()
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
			        
		        currentTarget = currentPath.vectorPath[currentWaypoint];
	        }   
        }

        private IEnumerator Spawn()
        {
	        Freeze();
	        
	        Transform spawned = Instantiate(spawnPrefab, spawnOrigin.TransformPoint(spawnOffset), transform.rotation);
	        spawned.transform.parent = transform.parent;
	        
	        C3SpawnAnimation anim = spawned.GetComponent<C3SpawnAnimation>();

	        yield return anim.StartCoroutine(anim.Execute());

	        BranchState();
        }

        private IEnumerator RotateToTarget()
        {
	        Vector2 lookDirection = (currentTarget - body.position).normalized;
	        
	        float oldAngle = body.rotation;
	        float newAngle = Mathf.Atan2 (lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90;

	        float t = 0;

	        while ((t += Time.deltaTime / rotationDuration) <= 1)
	        {
		        body.MoveRotation(Mathf.LerpAngle(oldAngle, newAngle, easeCurve.Evaluate(t)));
		        yield return null;
	        }
	        
	        body.MoveRotation(newAngle);
	        
	        yield return null;
	        
	        BranchState();
        }

        private IEnumerator MoveToPosition(Vector2 newPosition, float duration, string clip)
        {
	        Vector2 oldPosition = body.position;
	        
	        //animationHandler.PlayAudio(clip);

	        float t = 0;

	        while ((t += Time.deltaTime / duration) <= 1)
	        {
		        body.MovePosition(Vector2.Lerp(oldPosition, newPosition, easeCurve.Evaluate(t)));
		        animationHandler.Sample(clip, t);
		        yield return null;
	        }
	        
	        body.MovePosition(newPosition);
	        animationHandler.Sample(clip, 1.0f);
	        
	        yield return null;
	        
	        BranchState();
        }

        private void SpawnCooldownReset ()
        {
	        spawnTimer = Time.time + spawnCooldown.MinMaxRandom();
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
	        }
        }

        public static HashSet<C3> GetSpawned()
        {
	        return Spawned;
        }
        
#if UNITY_EDITOR        
        protected void OnDrawGizmos()
        {
	        Gizmos.color = Color.green;
	        Gizmos.DrawSphere(currentTarget, 0.1f);

	        if (spawnOrigin)
		        Gizmos.DrawSphere(spawnOrigin.TransformPoint(spawnOffset), 0.1f);
        }
#endif        
    }
}