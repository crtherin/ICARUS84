using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PhysicsAnimation : MonoBehaviour
{
    [Serializable]
    public class Keyframe
    {
        [SerializeField] private float duration;
        [SerializeField] private Vector2 position;

        public float Duration => duration;
        public Vector2 Position => position;
    }
    
    [SerializeField] private Keyframe[] keyframes;

    private new Rigidbody2D rigidbody;

    private int index;
    private Vector2 lastPosition;
    
    protected void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    protected void Start()
    {
        lastPosition = rigidbody.position;
    }

    protected void FixedUpdate()
    {
        if (keyframes.Length == 0)
            return;

        Keyframe keyframe = keyframes[index];
        
        float progress = InverseLerp(
            lastPosition,
            lastPosition + keyframe.Position,
            rigidbody.position);
        
        progress += Time.deltaTime / keyframe.Duration;
        
        rigidbody.MovePosition(Vector2.Lerp(
            lastPosition, 
            lastPosition + keyframe.Position, 
            Mathf.Clamp01(progress)));

        rigidbody.velocity = Vector2.zero;
        
        if (progress >= 1)
        {
            lastPosition += keyframe.Position;
            
            index += 1;
            if (index >= keyframes.Length)
                index = 0;
        }
    }
    
    private float InverseLerp(Vector2 a, Vector2 b, Vector2 value)
    {
        Vector2 ab = b - a;
        Vector2 av = value - a;
        return Mathf.Clamp01(Vector2.Dot(av, ab) / Vector2.Dot(ab, ab));
    }
}
