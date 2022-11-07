#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Timeline;

namespace Levels.Tiles
{
    [HideInMenu]
    public class Tile : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private float snap = 1;
        [SerializeField] protected LayerMask mask;

        public float Snap => snap;

        public LayerMask Mask => mask;
        
        protected void OnValidate()
        {
            hideFlags = HideFlags.DontSaveInBuild;
            snap = Mathf.Clamp(snap, 0.1f, 2.0f);
        }

        protected void OnEnable()
        {
            Refresh();
        }

        public virtual void Refresh()
        {
        }

        public static T OverlapTile<T>(Vector2 position, LayerMask mask) where T : Tile
        {
            Collider2D hit = Physics2D.OverlapPoint(position, mask);
            return hit ? hit.GetComponentInParent<T>() : null;
        }

        public static T OverlapTile<T>(Vector2 position, Vector2 direction, float snap, LayerMask mask)
            where T : Tile
        {
            return OverlapTile<T>(position + direction * snap, mask);
        }

        public static Tile OverlapTile(Vector2 position, LayerMask mask)
        {
            return OverlapTile<Tile>(position, mask);
        }

        public static Transform SpawnPrefab(Transform prefab, Vector2 position = default, Quaternion rotation = default)
        {
            if (!PrefabUtility.IsPartOfPrefabAsset(prefab.gameObject))
                return Instantiate(prefab, position, rotation);

            Object spawnedObject = PrefabUtility.InstantiatePrefab(prefab.gameObject);
            GameObject spawned = spawnedObject as GameObject;

            if (spawned == null)
            {
                DestroyImmediate(spawnedObject);
                return Instantiate(prefab, position, rotation);
            }

            prefab = spawned.transform;
            prefab.position = position;
            prefab.rotation = rotation;
            return prefab;
        }
#endif
    }
}