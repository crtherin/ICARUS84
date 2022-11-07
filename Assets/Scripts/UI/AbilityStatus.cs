using System.Collections.Generic;
using Procedures;
using UnityEngine;

namespace UI
{
    public class AbilityStatus : MonoBehaviour
    {
        [SerializeField] private Procedure ability;
        [SerializeField] private RectTransform active;
        [SerializeField] private GameObject disabled;
        [SerializeField] private float popDuration = 1;
        [SerializeField] private float popBounces = 0.5f;
    
        private Receiver<Cooldown, float> cooldownReceiver;

        private float t;

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
                t = 0;
                disabled.SetActive(true);
                return;
            }
        
            disabled.SetActive(false);
            t += Time.deltaTime / popDuration;
            active.localScale = Vector3.one * EaseOutElastic(t, popBounces);
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