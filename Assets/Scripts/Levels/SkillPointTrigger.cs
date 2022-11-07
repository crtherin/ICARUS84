using UnityEngine;

namespace Levels
{
    public class SkillPointTrigger : MonoBehaviour
    {
        [SerializeField] private ChallengeManager challengeManager;

        protected void OnTriggerEnter2D(Collider2D other)
        {
            challengeManager.QueueSkillPoint();
        }

#if UNITY_EDITOR
        protected void Reset()
        {
            challengeManager = FindObjectOfType<ChallengeManager>();
        }
#endif
    }
}