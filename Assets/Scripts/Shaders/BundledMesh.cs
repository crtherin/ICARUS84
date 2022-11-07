using System;
using UnityEngine;

namespace Shaders
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class BundledMesh : MonoBehaviour
    {
        [Serializable]
        public class Triangle
        {
            [SerializeField] private Vector2 a;
            [SerializeField] private Vector2 b;
            [SerializeField] private Vector2 c;

            public Vector2 A => a;
            public Vector2 B => b;
            public Vector2 C => c;
        }
        
        [Serializable]
        public class Bundle
        {
            [SerializeField] private Triangle[] triangles;

            public int Count => triangles.Length;

            public Triangle Get(int i)
            {
                return triangles[i];
            }
        }

        [SerializeField] private Bundle[] bundles;
        [SerializeField] private int displayedBundle;

        protected void OnDrawGizmos()
        {
            if (bundles != null && displayedBundle >= 0 && displayedBundle < bundles.Length)
            {
                Bundle bundle = bundles[displayedBundle];

                for (int i = 0; i < bundle.Count; i += 1)
                {
                    Triangle triangle = bundle.Get(i);

                    Vector3 a = TransformVertex(triangle.A);
                    Vector3 b = TransformVertex(triangle.B);
                    Vector3 c = TransformVertex(triangle.C);
                    
                    Gizmos.DrawLine(a, b);
                    Gizmos.DrawLine(b, c);
                    Gizmos.DrawLine(c, a);
                }
            }
        }

        private Vector3 TransformVertex(Vector2 position)
        {
            Vector3 p = transform.position;
            p.x += position.x;
            p.y += position.y;
            return p;
        }
    }
}