using Damage;
using UnityEngine;

namespace Shaders
{
    public class SpriteNoiseBar : MonoBehaviour
    {
        [SerializeField, Range(0, 1)] private float maxNoise;
        [SerializeField] private AnimationCurve curve;
        [SerializeField] private HealthHandler handler;
        [SerializeField] private SpriteRenderer[] renderers;

        private MaterialPropertyBlock propertyBlock;
    
        private static readonly int NoiseBlendID = Shader.PropertyToID("_NoiseBlend");

#if UNITY_EDITOR
        protected void Reset()
        {
            renderers = GetComponentsInChildren<SpriteRenderer>();
            curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        }
#endif
    
        protected void Start()
        {
            propertyBlock = new MaterialPropertyBlock();
        }

        protected void Update()
        {
            SetFill(GetFill());
        }

        private float GetFill()
        {
            return 1 - handler.GetHealth() / handler.MaxHealth.Get();
        }

        private void SetFill(float fill)
        {
            float blend = curve.Evaluate(fill) * maxNoise;
        
            for (int i = 0; i < renderers.Length; i++)
            {
                SpriteRenderer r = renderers[i];
            
                r.GetPropertyBlock(propertyBlock);
                propertyBlock.SetFloat(NoiseBlendID, blend);
                r.SetPropertyBlock(propertyBlock);
            }
        }
    }
}
