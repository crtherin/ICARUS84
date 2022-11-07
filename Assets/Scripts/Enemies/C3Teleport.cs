using System.Collections;
using UnityEngine;

namespace Enemies
{
    public class C3Teleport : MonoBehaviour
    {
        [SerializeField] private float beforeDelay = 0.5f;
        [SerializeField] private float afterDelay = 0.2f;
        [SerializeField] private AnimationCurve beforeCurve;
        [SerializeField] private AnimationCurve afterCurve;
        [SerializeField] private Transform effect;

        private AudioClipContainer audioContainer;
        private AudioClipContainer.NamedClip audioTeleportStart;
        private AudioClipContainer.NamedClip audioTeleportEnd;
        
        private bool isTeleporting;

        protected void Awake()
        {
            audioContainer = GetComponentInChildren<AudioClipContainer>();
            audioTeleportStart = audioContainer.GetClip("Teleport_Start");
            audioTeleportEnd = audioContainer.GetClip("Teleport_End");
        }

        public bool CanStartTeleport()
        {
            return !isTeleporting;
        }

        public IEnumerator StartTeleport()
        {
            isTeleporting = true;
            effect.gameObject.SetActive(true);
            
            audioContainer.PlayClip(audioTeleportStart);

            float t = 0;

            while ((t += Time.deltaTime / beforeDelay) < 1)
            {
                effect.transform.localScale = Vector3.one * beforeCurve.Evaluate(t);
                yield return null;
            }
            
            effect.transform.localScale = Vector3.one * beforeCurve.Evaluate(1);
        }

        public void TeleportTo(Vector2 targetPos)
        {
            transform.position = targetPos;
        }

        public IEnumerator EndTeleport()
        {
            audioContainer.PlayClip(audioTeleportEnd);
            
            float t = 0;

            while ((t += Time.deltaTime / afterDelay) < 1)
            {
                effect.transform.localScale = Vector3.one * afterCurve.Evaluate(1 - t);
                yield return null;
            }
            
            effect.gameObject.SetActive(false);
            isTeleporting = false;
        }
    }
}
