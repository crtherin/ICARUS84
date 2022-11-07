using Expressions;
using UnityEngine;
using System.Collections;

namespace Enemies
{
    public class C3SpawnAnimation : MonoBehaviour, IExpressionElement
    {
        [SerializeField] private float beforeDelay = 0.5f;
        [SerializeField] private float afterDelay = 0.25f;
        [SerializeField] private AnimationCurve beforeCurve;
        [SerializeField] private AnimationCurve afterCurve;
        
        [Header("References")]
        [SerializeField] private GameObject mainVisual;
        [SerializeField] private GameObject spawnAnimation;
        [SerializeField] private C1 c1;
        [SerializeField] private CharacterMotor motor;
        [SerializeField] private new Rigidbody2D rigidbody2D;
        [SerializeField] private new CircleCollider2D collider2D;
        [SerializeField] private AudioClipContainer audioContainer;
        
        private AudioClipContainer.NamedClip audioSpawnStart;
        private AudioClipContainer.NamedClip audioSpawnEnd;
        
#if UNITY_EDITOR
        protected void Reset()
        {
            c1 = GetComponent<C1>();
            motor = GetComponent<CharacterMotor>();
            rigidbody2D = GetComponent<Rigidbody2D>();
            collider2D = GetComponent<CircleCollider2D>();
            audioContainer = GetComponentInChildren<AudioClipContainer>();
        }
#endif        

        private void Freeze()
        {
            c1.enabled = false;
            
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            rigidbody2D.simulated = false;
            collider2D.enabled = false;
            
            motor.CanMove.Flip(this);
            motor.CanRotate.Flip(this);
        }

        private void Unfreeze()
        {
            rigidbody2D.constraints = RigidbodyConstraints2D.None;
            rigidbody2D.simulated = true;
            collider2D.enabled = true;

            motor.CanMove.Unflip(this);
            motor.CanRotate.Unflip(this);

            c1.enabled = true;
        }

        public IEnumerator Execute()
        {
            audioSpawnStart = audioContainer.GetClip("Spawn_Start");
            audioSpawnEnd = audioContainer.GetClip("Spawn_End");
            
            Freeze();

            Vector3 size = spawnAnimation.transform.localScale;
            
            mainVisual.SetActive(false);
            yield return StartCoroutine(AnimationStart(size));            
            
            mainVisual.SetActive(true);
            yield return StartCoroutine(AnimationEnd(size));

            Unfreeze();
        }

        private IEnumerator AnimationStart(Vector3 size)
        {
            spawnAnimation.SetActive(true);
            
            audioContainer.PlayClip(audioSpawnStart);
            
            float t = 0;

            while ((t += Time.deltaTime / beforeDelay) < 1)
            {
                spawnAnimation.transform.localScale = size * beforeCurve.Evaluate(t);
                yield return null;
            }
            
            spawnAnimation.transform.localScale = size * beforeCurve.Evaluate(1);
        }

        private IEnumerator AnimationEnd(Vector3 size)
        {
            audioContainer.PlayClip(audioSpawnEnd);
            
            float t = 1;
            
            while ((t -= Time.deltaTime / afterDelay) > 0)
            {
                spawnAnimation.transform.localScale = size * afterCurve.Evaluate(t);
                yield return null;
            }
            
            spawnAnimation.SetActive(false);
        }
    }
}
