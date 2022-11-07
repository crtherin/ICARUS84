using Damage;
using Levels;
using UnityEngine;

namespace Enemies
{
    public class EnemyChallenge : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour behaviour;
        [SerializeField] private ChallengeManager challengeManager;
        
        private HealthHandler healthHandler;
        private BehaviourTrigger behaviourTrigger;

        protected void Start()
        {
            healthHandler = GetComponent<HealthHandler>();
            
            if (healthHandler)
                healthHandler.Death += HealthHandlerOnDeath;
        }

        protected void OnEnable()
        {
            challengeManager.RegisterEnemy();
        }

        public void Aggro(BehaviourTrigger trigger)
        {
            if (behaviour.enabled)
                return;
            
            behaviour.enabled = true;
            behaviourTrigger = trigger;
            challengeManager.AggroEnemy();
        }
        
        private void HealthHandlerOnDeath(object sender, DamageInfo e)
        {
            if (behaviourTrigger != null)
                behaviourTrigger.UnregisterEnemy(this);
            
            if (challengeManager != null)
                challengeManager.UnregisterEnemy();
        }

#if UNITY_EDITOR
        protected void Reset()
        {
            behaviour = GetComponent<MonoBehaviour>();
            challengeManager = FindObjectOfType<ChallengeManager>();
        }
#endif
    }
}