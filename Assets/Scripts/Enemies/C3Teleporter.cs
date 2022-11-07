using UnityEngine;
using Miscellaneous;
using System.Collections;
using System.Collections.Generic;

namespace Enemies
{
    public class C3Teleporter : Singleton<C3Teleporter>
    {
        [SerializeField] private float closestUpdate = 0.1f;
        
        [Header ("Teleport")]
        [SerializeField] private Vector2 teleportDistance = new Vector2(1, 3);
        [SerializeField] private Vector2 teleportCooldown = new Vector2(6, 7);
        
        [Header ("Conditions")]
        [SerializeField] private Vector2 maxSelfTeleportDistance = new Vector2(3, 4);
        [SerializeField] private Vector2 minSpawnTeleportDistance = new Vector2(5, 6);

        private Transform player;
        private C3Teleport playerTeleport;
    
        private float closestUpdateTimer;
        private Transform closest;

        private float curMaxDist;
        private float curMinDist;

        private float teleportTimer;
        private bool isTeleporting;

        protected void Awake()
        {
            if (GetInstance() != this)
                Destroy(gameObject);
        }
    
        protected void Start()
        {
            player = GameObject.FindWithTag("Player").transform;
            playerTeleport = player.GetComponent<C3Teleport>();
        
            DistanceReset();
            TeleportReset();
        }
    
        protected void Update()
        {
            if (!player)
                return;

            if (C3.GetSpawned().Count == 0)
            {
                teleportTimer = Time.time + teleportCooldown.MinMaxRandom();
                return;
            }

            if (closestUpdateTimer < Time.time)
            {
                closestUpdateTimer = Time.time + closestUpdate;
                FindClosest();
            }

            if (!isTeleporting && teleportTimer < Time.time)
            {
                if (ShouldTeleport())
                {
                    teleportTimer = Time.time + teleportCooldown.MinMaxRandom();
                    StartCoroutine(Teleport());
                }
            }
        }

        private bool ShouldTeleport()
        {
            if (C1.GetSpawned().Count == 0)
                return false;
            
            Vector3 playerPos = player.position;
            
            if (closest && (closest.position - playerPos).magnitude > curMinDist)
                return true;
            
            HashSet<C3> spawners = C3.GetSpawned();

            foreach (C3 c3 in spawners)
            {
                if ((c3.transform.position - playerPos).magnitude < curMaxDist)
                    return true;
            }

            return false;
        }

        private IEnumerator Teleport()
        {
            if (playerTeleport.CanStartTeleport())
            {
                isTeleporting = true;

                yield return playerTeleport.StartCoroutine(playerTeleport.StartTeleport());
                
                List<C1> spawned = C1.GetSpawned();
                C1 randomC1 = spawned[Random.Range(0, spawned.Count)];
                Vector2 randomDir = Random.insideUnitCircle;
                float randomDist = teleportDistance.MinMaxRandom();

                Vector2 targetPos = randomC1.transform.position;
                targetPos += randomDir.normalized * randomDist;
                
                playerTeleport.TeleportTo(targetPos);

                yield return playerTeleport.StartCoroutine(playerTeleport.EndTeleport());

                isTeleporting = false;
            }
        }

        private void DistanceReset()
        {
            curMaxDist = maxSelfTeleportDistance.MinMaxRandom();
            curMinDist = minSpawnTeleportDistance.MinMaxRandom();
        }
        
        private void TeleportReset()
        {
            teleportTimer = Time.time + teleportCooldown.MinMaxRandom();
        }

        private void FindClosest()
        {
            closest = null;
        
            List<C1> spawned = C1.GetSpawned();

            if (spawned.Count == 0)
                return;
        
            float closestDistance = float.MaxValue;

            Vector3 playerPos = player.position;

            foreach (C1 c1 in spawned)
            {
                Transform c1Transform = c1.transform;
                float distance = (c1Transform.position - playerPos).sqrMagnitude;

                if (closestDistance > distance)
                {
                    closestDistance = distance;
                    closest = c1Transform;
                }
            }
        }
    
#if UNITY_EDITOR
        protected void OnDrawGizmos()
        {
            List<C1> spawned = C1.GetSpawned();

            foreach (C1 c1 in spawned)
            {
                Transform c1Transform = c1.transform;
                Gizmos.color = c1Transform == closest ? Color.green : new Color(0, 1, 0, 0.1f);
                Gizmos.DrawWireSphere(c1Transform.position, curMinDist);
            }

            HashSet<C3> spawners = C3.GetSpawned();

            foreach (C3 c3 in spawners)
            {
                Transform c3Transform = c3.transform;
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(c3Transform.position, curMaxDist);
            }
        }
#endif    
    }
}
