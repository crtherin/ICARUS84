using Damage;
using Miscellaneous;
using UnityEngine;
using System.Collections.Generic;

namespace Handlers
{
    public class SplashParticleHandler : MonoBehaviour
    {
        class Particle
        {
            private readonly SpriteRenderer renderer;
            private readonly Transform transform;
            private float deathTime;
            private float velocity;
            private float drag;
            private Vector3 direction;

            public Particle(SpriteRenderer renderer)
            {
                this.renderer = renderer;
                renderer.enabled = false;
                
                transform = renderer.transform;

                deathTime = 0;
                velocity = 0;
                drag = 0;
                direction = Vector3.zero;
            }

            public void Reset(Sprite sprite, float newLifetime, float newVelocity, float newDrag, float scale, Transform newTarget, Vector3 newDirection)
            {
                deathTime = newLifetime;
                velocity = newVelocity;
                drag = newDrag;
                direction = newDirection;

                transform.position = newTarget.position;
                transform.rotation = newTarget.rotation;
                transform.localScale = Vector3.one * scale;
                renderer.sprite = sprite;
                renderer.enabled = true;
            }

            public void Update(float dt)
            {
                velocity -= velocity * drag * dt;
                transform.position += direction * (velocity * dt);
            }

            public bool IsDead(float t)
            {
                return deathTime < t;
            }

            public void Hide()
            {
                renderer.sprite = null;
                renderer.enabled = false;
            }
        }

        private static SplashParticleHandler _instance;
        
        [SerializeField] private Transform particlePrefab;
        [SerializeField] private float coneAngle = 30;
        [SerializeField] private MinMaxFloat velocity = new MinMaxFloat(1, 2);
        [SerializeField] private MinMaxFloat drag = new MinMaxFloat(1, 2);
        [SerializeField] private MinMaxFloat lifetime = new MinMaxFloat(1, 2);
        [SerializeField] private bool stayForever = true;
        [SerializeField] private MinMaxInt count = new MinMaxInt(5, 10);
        [SerializeField] private float scale = 0.2f;

        private readonly Stack<Particle> freeParticles = new Stack<Particle>();
        private readonly HashSet<Particle> activeParticles = new HashSet<Particle>();
        private readonly HashSet<Particle> expiredParticles = new HashSet<Particle>();

        protected void Awake()
        {
            _instance = this;
        }

        protected void Update()
        {
            if (activeParticles.Count == 0)
            {
                enabled = false;
                return;
            }
            
            expiredParticles.Clear();

            float t = Time.time;
            float dt = Time.deltaTime;

            foreach (Particle activeParticle in activeParticles)
            {
                activeParticle.Update(dt);

                if (activeParticle.IsDead(t))
                    expiredParticles.Add(activeParticle);
            }

            foreach (Particle expiredParticle in expiredParticles)
            {
                activeParticles.Remove(expiredParticle);
                
                if (!stayForever)
                    PoolParticle(expiredParticle);
            }
        }

        private void Spawn(Sprite sprite, DamageInfo e)
        {
            Particle particle = GetFreeParticle();
            
            particle.Reset(
                sprite,
                Time.time + lifetime.Random,
                velocity.Random * e.Damage,
                drag.Random,
                scale,
                e.Target.transform,
                Quaternion.Euler(0, 0, Random.Range(-coneAngle / 2, coneAngle / 2)) * e.Direction);
            
            activeParticles.Add(particle);

            if (!enabled)
                enabled = true;
        }

        private Particle GetFreeParticle()
        {
            if (freeParticles.Count > 0)
                return freeParticles.Pop();

            Transform newTransform = Instantiate(particlePrefab, Vector3.zero, Quaternion.identity, transform);
            SpriteRenderer newRenderer = newTransform.GetComponent<SpriteRenderer>();
            Particle newParticle = new Particle(newRenderer);

            return newParticle;
        }

        private void PoolParticle(Particle particle)
        {
            particle.Hide();
            activeParticles.Remove(particle);
            freeParticles.Push(particle);
        }

        public static void Create(Sprite sprite, DamageInfo e)
        {
            if (!_instance)
            {
                Debug.LogError("Splash Particle Handler instance not found in the scene.");
                return;
            }

            int count = _instance.count.Random;
            
            for (int i = 0; i < count; i++)
                _instance.Spawn(sprite, e);
        }
    }
}