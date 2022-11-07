using Damage;
using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Handlers
{
    [DisallowMultipleComponent]
    [RequireComponent (typeof(Rigidbody2D))]
    [RequireComponent (typeof(HealthHandler))]
    public class HurtHandler : MonoBehaviour
    {
        enum Type
        {
            Absolute,
            PercentageTotal,
            PercentageCurrent,
            PercentageMissing
        }

        struct Effect
        {
            public float Time { get; private set; }
            public float Strength { get; private set; }
            public Vector3 Direction { get; private set; }

            public Effect(float time, float strength, Vector3 direction)
            {
                Time = time;
                Strength = strength;
                Direction = direction;
            }
        }

        [Serializable]
        struct TintPart
        {
            public string name;
            public bool isEnabled;
            public bool isPeripheral;
            [ShowIfPlayMode, ReadOnly] public bool isAffected;
            public BodyPart[] bodyParts;
        }

        [Serializable]
        struct BodyPart
        {
            public TintHandler tintHandler;
            public int tintIdx;
        }

        [SerializeField] private Type type;

        [Header ("Knockback")]
        [SerializeField] private float knockbackForceMultiplier = 1;
        [SerializeField] private float knockbackDurationMultiplier = 1;

        [Header("Flash")]
        // [SerializeField] private Color flashTint = Color.red;
        [SerializeField] private float flashAnimDuration = 0.2f;
        [SerializeField] private float flashTintMultiplier = 1;
        [SerializeField] private float flashDurationMultiplier = 1;
        [SerializeField] private int peripheralIgnoredMin = 1;
        [SerializeField] private int peripheralIgnoredMax = 1;
        [SerializeField] private TintPart[] tintParts;

        [Header("Splash")]
        [SerializeField] private Sprite splashSprite;

        private readonly HashSet<Effect> activeForces = new HashSet<Effect>();
        private readonly HashSet<Effect> activeFlashes = new HashSet<Effect>();

        private new Rigidbody2D rigidbody;
        private HealthHandler healthHandler;
        private List<int> peripheralIndices;

        private float flashAnim;
        private float lastMaxTint;

        protected void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            healthHandler = GetComponent<HealthHandler>();
        }

        protected void Start()
        {
            healthHandler.Receive += HealthHandlerOnReceive;

            peripheralIndices = new List<int>();

            for (int i = 0; i < tintParts.Length; i++)
            {
                if (tintParts[i].isPeripheral)
                    peripheralIndices.Add(i);
            }
        }

        protected void Update()
        {
            if (activeFlashes.Count > 0)
            {
                activeFlashes.RemoveWhere(effect => Time.time > effect.Time);

                if (activeFlashes.Count == 0)
                {
                    flashAnim = Mathf.Clamp01(flashAnim - Time.deltaTime / flashAnimDuration);
                    SetTint(flashAnim * lastMaxTint);
                    return;
                }
                
                float maxStrength = activeFlashes.Max(effect => effect.Strength);
                float maxTint = 1 - 1 / (maxStrength * flashTintMultiplier);
                lastMaxTint = maxTint;
                
                flashAnim = Mathf.Clamp01(flashAnim + Time.deltaTime / flashAnimDuration);
                SetTint(flashAnim * maxTint);
            } 
            else if (activeFlashes.Count == 0)
            {
                flashAnim = Mathf.Clamp01(flashAnim - Time.deltaTime / flashAnimDuration);
                SetTint(flashAnim * lastMaxTint);
            }
        }

        protected void FixedUpdate()
        {
            if (activeForces.Count > 0)
            {
                activeForces.RemoveWhere(effect => Time.time > effect.Time);
                
                if (activeForces.Count == 0)
                {
                    return;
                }
                
                foreach (Effect effect in activeForces)
                {
                    if (effect.Direction.sqrMagnitude > 0.1f)
                    {
                        rigidbody.AddForce(effect.Direction * (effect.Strength * knockbackForceMultiplier),
                            ForceMode2D.Force);
                    }
                }
            }
        }

        private void HealthHandlerOnReceive(object sender, DamageInfo e)
        {
            float strength = MapStrength(e.Damage);
            
            activeForces.Add(new Effect(Time.time + strength * knockbackDurationMultiplier,
                strength,
                e.Direction));
            
            activeFlashes.Add(new Effect(Time.time + strength * flashDurationMultiplier,
                strength,
                e.Direction));

            ShufflePeripheralParts();
            
            SplashParticleHandler.Create(splashSprite, e);
        }

        private float MapStrength(float damage)
        {
            switch (type)
            {
                default:
                    return damage;
                case Type.Absolute:
                    return damage;
                case Type.PercentageTotal:
                    return damage / healthHandler.MaxHealth;
                case Type.PercentageCurrent:
                    return damage / healthHandler.GetHealth();
                case Type.PercentageMissing:
                    return damage / (healthHandler.MaxHealth - healthHandler.GetHealth());
            }
        }

        private void ShufflePeripheralParts()
        {
            Shuffle(peripheralIndices);
            
            int unaffectedCount = Random.Range(peripheralIgnoredMin, peripheralIgnoredMax);
            
            for (var i = 0; i < peripheralIndices.Count; i++)
            {
                int peripheralIndex = peripheralIndices[i];
                tintParts[peripheralIndex].isAffected = unaffectedCount < i;
            }
        }

        private static void Shuffle<T>(IList<T> ts)
        {
            var count = ts.Count;
            var last = count - 1;
            
            for (var i = 0; i < last; ++i)
            {
                var r = Random.Range(i, count);
                var tmp = ts[i];
                ts[i] = ts[r];
                ts[r] = tmp;
            }
        }

        private void SetTint(float tint)
        {
            foreach (TintPart tintPart in tintParts)
            {
                SetTintPart(tintPart, tintPart.isEnabled && (!tintPart.isPeripheral || tintPart.isAffected) ? tint : 0);
            }
        }

        private void SetTintPart(TintPart tintPart, float tint)
        {
            foreach (BodyPart bodyPart in tintPart.bodyParts)
            {
                bodyPart.tintHandler.SetTint(bodyPart.tintIdx, tint);
            }
        }
    }
}