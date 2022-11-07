using System.Collections.Generic;
using Procedures;
using UnityEngine;

namespace Shaders
{
    public class SpriteAbilityStatus : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private int[] affectedParts;
        [SerializeField] private Procedure ability;
        
        [Header ("Cooldown Start")]
        [SerializeField] private float coolDuration = 1;
        [SerializeField] private float coolBounces = 0.5f;
        [SerializeField] private float coolMax = 1.0f;
        
        [Header ("Cooldown End")]
        [SerializeField] private float heatDuration = 1;
        [SerializeField] private float heatBounces = 0.5f;
        [SerializeField] private float heatMax = 1.0f;
    
        private Receiver<Cooldown, float> cooldownReceiver;

        private MaterialPropertyBlock block;
        
        private float t;

        private static readonly int[] CoolIDs = {
            Shader.PropertyToID("_CoolRA"),
            Shader.PropertyToID("_CoolGA"),
            Shader.PropertyToID("_CoolBA"),
            Shader.PropertyToID("_CoolRB"),
            Shader.PropertyToID("_CoolGB"),
            Shader.PropertyToID("_CoolBB"),
            Shader.PropertyToID("_CoolRC"),
            Shader.PropertyToID("_CoolGC"),
            Shader.PropertyToID("_CoolBC"),
            Shader.PropertyToID("_CoolRD"),
            Shader.PropertyToID("_CoolGD"),
            Shader.PropertyToID("_CoolBD")
        };
        
#if UNITY_EDITOR    
        protected void Reset()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            ability = GetComponent<Procedure>();
        }
#endif        

        private void Start()
        {
            cooldownReceiver = new Receiver<Cooldown, float>(new CooldownPercentageRegister(), new FloatMultiplier());

            List<Process> processes = ability.GetProcesses();

            for (var i = 0; i < processes.Count; i++)
            {
                if (processes[i] is Cooldown process)
                {
                    cooldownReceiver.Add(process);
                }
            }
        }

        protected void Update()
        {
            float cooldown = cooldownReceiver.Receive();

            if (cooldown > 0)
            {
                t += Time.deltaTime / coolDuration;
                if (t > 1) t = 1;
                
                SetCool(EaseOutElastic(t, coolBounces) * coolMax);
                return;
            }
            
            t -= Time.deltaTime / heatDuration;
            if (t < 0) t = 0;
            
            SetCool(heatMax - EaseOutElastic(1 - t, heatBounces) * heatMax);
        }

        private void SetCool(float c)
        {
            if (block == null)
                block = new MaterialPropertyBlock();

            spriteRenderer.GetPropertyBlock(block);

            for (int i = 0; i < affectedParts.Length; i++)
                block.SetFloat(CoolIDs[affectedParts[i]], c);

            spriteRenderer.SetPropertyBlock(block);
        }
        
        private static float EaseOutElastic(float t, float b)
        {
            if (t <= 0) return 0;
            if (t >= 1) return 1;

            float p = 0.3f;
            float a = 1;
            float s = p / 4;
            return a * Mathf.Pow(2, -10 * t) * Mathf.Sin((t - s) * (b * Mathf.PI) / p) + 1;
        }
    }
}