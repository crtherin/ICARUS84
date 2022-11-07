using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CharacterMotor))]
public class LocalAvoidance : MonoBehaviour
{
    enum Mode
    {
        Circular,
        Velocity
    }
    
    enum Falloff
    {
        None,
        Linear,
        Quadratic
    }
    
    private static readonly HashSet<Rigidbody2D> registered = new HashSet<Rigidbody2D>();

    [SerializeField] private float refreshRate = 0.5f;
    [SerializeField] private float strength = 50;
    [SerializeField] private float random = 5;
    [SerializeField] private float distance = 5;
    [SerializeField] private float radius = 2;
    [SerializeField] private Mode mode = Mode.Velocity;
    [SerializeField] private Falloff falloff = Falloff.None;

    private float nextTime;
    private float currentRandom;
    
    private LayerMask mask;
    private new Rigidbody2D rigidbody;
    private readonly HashSet<Rigidbody2D> neighbours = new HashSet<Rigidbody2D>();

    protected void Awake()
    {
        mask = 1 << LayerMask.NameToLayer("Enemies");
        rigidbody = GetComponent<Rigidbody2D>();
    }

    protected void OnEnable()
    {
        registered.Add(rigidbody);
    }

    protected void OnDisable()
    {
        registered.Remove(rigidbody);
    }

    protected void FixedUpdate()
    {
        Vector2 position = rigidbody.position;

        if (nextTime < Time.time)
        {
            nextTime = Time.time + refreshRate;
            currentRandom = Random.value * random;
            
            Collider2D[] colliders = Physics2D.OverlapCircleAll(position, distance, mask);
            neighbours.Clear();

            foreach (Collider2D c in colliders)
            {
                Rigidbody2D body = c.attachedRigidbody;

                if (body && registered.Contains(body))
                    neighbours.Add(body);
            }
        }

        foreach (Rigidbody2D neighbour in neighbours)
        {
            if (!neighbour)
                continue;

            Vector2 neighbourPosition = neighbour.position;
            Vector2 direction = position - neighbourPosition;
            float dist = direction.magnitude;
            direction.Normalize();

            if (dist > radius)
                continue;

            float force = strength + currentRandom;
            force *= ApplyMode(direction);
            force *= ApplyFalloff(dist);
            
            rigidbody.AddForce(direction.normalized * force, ForceMode2D.Force);
        }
    }

    private float ApplyMode(Vector2 direction)
    {
        switch (mode)
        {
            default:
                return 1;
            case Mode.Circular:
                return 1;
            case Mode.Velocity:
                return Vector3.Project(direction.normalized, rigidbody.velocity.normalized).magnitude;
        }
    }

    private float ApplyFalloff(float dist)
    {
        switch (falloff)
        {
            default:
                return 1;
            case Falloff.None:
                return 1;
            case Falloff.Linear:
                return 1 - dist / radius;
            case Falloff.Quadratic:
                return (1 - dist / radius) * (1 - dist / radius);
        }
    }
    
#if UNITY_EDITOR
    protected void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawWireSphere(transform.position, radius);
        
        Gizmos.color = new Color(1, 1, 0, 0.2f);
        Gizmos.DrawWireSphere(transform.position, distance);


        if (!Application.isPlaying)
            return;
        
        foreach (Rigidbody2D neighbour in neighbours)
        {
            Vector2 neighbourPosition = neighbour.position;
            Vector2 direction = neighbourPosition - rigidbody.position;
            float dist = direction.magnitude;

            if (dist > radius)
                continue;

            float force = 1 - dist / radius;

            Gizmos.color = Color.Lerp(Color.clear, Color.magenta, force);
            Gizmos.DrawLine(neighbourPosition, rigidbody.position);
        }
    }
#endif
}