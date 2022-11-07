using Enemies;
using UnityEngine;
using System.Collections.Generic;

namespace Levels
{
    public class BehaviourTrigger : Persistence<BehaviourTrigger.Data>
    {
        public struct Data
        {
            public bool HasTriggered;

            public Data(bool hasTriggered)
            {
                HasTriggered = hasTriggered;
            }
        }

        [SerializeField] private EnemyChallenge[] targets;
        [SerializeField] private BehaviourTrigger[] previous;

        private bool triggered;
        private HashSet<EnemyChallenge> alive;

        public override Data ExportData()
        {
            return new Data(triggered);
        }

        public override void ImportData(Data data)
        {
            if (data.HasTriggered)
            {
                // Trigger();
                Destroy(gameObject);
            }
        }

        public void UnregisterEnemy(EnemyChallenge enemy)
        {
            if (alive.Contains(enemy))
                alive.Remove(enemy);

            if (alive.Count == 0)
            {
                Stage();
            }
        }

        protected void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                return;

            Trigger();
        }

        private void Trigger()
        {
            if (triggered)
                return;
            
            triggered = true;
            
            foreach (var trigger in previous)
                if (trigger != null)
                    trigger.Trigger();

            foreach (EnemyChallenge target in targets)
            {
                if (target != null)
                {
                    target.Aggro(this);
                    
                    if (alive == null)
                        alive = new HashSet<EnemyChallenge>();
                    alive.Add(target);
                }
            }
            
            gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        protected void OnDrawGizmos()
        {
            Color color = Color.yellow;
            color.a = 0.2f;
            Gizmos.color = color;
            foreach (var target in targets)
            {
                if (target == null)
                    continue;

                Vector3 start = transform.position;
                Vector3 end = target.transform.position;
                Vector3 dir = end - start;
                dir = dir.normalized * (dir.magnitude - 0.75f);
                // Gizmos.DrawWireSphere(end, 1.5f);
                Gizmos.DrawRay(start, dir);
            }
            
            color = Color.magenta;
            color.a = 0.2f;
            Gizmos.color = color;
            foreach (var trigger in previous)
            {
                if (trigger == null)
                    continue;
                
                Vector3 start = transform.position;
                Vector3 end = trigger.transform.position;
                Vector3 dir = end - start;
                dir = dir.normalized * (dir.magnitude - 0.75f);
                Gizmos.DrawRay(start, dir);
            }
        }
#endif
    }
}