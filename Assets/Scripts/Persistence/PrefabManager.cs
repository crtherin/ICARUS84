using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Miscellaneous;

public class PrefabManager : Singleton<PrefabManager>
{
    [Serializable]
    class PrefabInfo
    {
        [SerializeField] private string id;
        [SerializeField] private GameObject prefab;

        public string Id => id;
        public GameObject Prefab => prefab;

        public PrefabInfo()
        {
            id = null;
            prefab = null;
        }

        public PrefabInfo(string id, GameObject prefab)
        {
            this.id = id;
            this.prefab = prefab;
        }
    }

    [SerializeField] private PrefabInfo[] references;

    private Dictionary<string, GameObject> table;

    protected void Awake()
    {
        table = references.ToDictionary(p => p.Id, p => p.Prefab, new StructuralEqualityComparer<string>());
    }

    public static GameObject Get(string id)
    {
        return GetInstance().table[id];
    }

    #region EDITOR

#if UNITY_EDITOR
    [Serializable]
    class PrefabReference
    {
        [SerializeField] private string id;
        [SerializeField] private GameObject prefab;
        [SerializeField] private List<MonoBehaviour> users;

        public string Id => id;
        public GameObject Prefab => prefab;

        public PrefabReference()
        {
            id = null;
            prefab = null;
            users = new List<MonoBehaviour>();
        }

        public PrefabReference(string id, GameObject prefab)
        {
            this.id = id;
            this.prefab = prefab;
            users = new List<MonoBehaviour>();
        }

        public void Register(MonoBehaviour user)
        {
            if (!Contains(user))
                users.Add(user);
        }

        public bool Contains(MonoBehaviour user)
        {
            users.RemoveAll(u => u == null);
            return users.Contains(user);
        }

        public bool Unregister(MonoBehaviour user)
        {
            if (Contains(user))
                users.Remove(user);

            return users.Count == 0;
        }
    }

    [SerializeField] private List<PrefabReference> sceneReferences;

    protected void Reset()
    {
        sceneReferences = new List<PrefabReference>();
    }

    protected void OnValidate()
    {
        for (int i = sceneReferences.Count - 1; i >= 0; i--)
        {
            sceneReferences[i].Contains(null);
            UnregisterSceneUser(sceneReferences[i], null);
        }
    }

    public static void RegisterSceneUser(string id, GameObject prefab, MonoBehaviour user)
    {
        PrefabReference found = null;

        foreach (PrefabReference prefabReference in GetInstance().sceneReferences)
        {
            if (prefabReference.Id == id)
            {
                found = prefabReference;
                break;
            }
        }

        if (found == null)
        {
            found = new PrefabReference(id, prefab);
            GetInstance().sceneReferences.Add(found);
        }

        found.Register(user);

        UpdateReferences();
    }

    public static void UnregisterSceneUser(string id, MonoBehaviour user)
    {
        PrefabReference found = null;

        foreach (PrefabReference prefabReference in GetInstance().sceneReferences)
        {
            if (prefabReference.Id.SequenceEqual(id))
            {
                found = prefabReference;
                break;
            }
        }

        if (found != null)
            UnregisterSceneUser(found, user);
    }

    private static void UnregisterSceneUser(PrefabReference prefabReference, MonoBehaviour user)
    {
        if (prefabReference.Unregister(user))
            GetInstance().sceneReferences.Remove(prefabReference);

        UpdateReferences();
    }

    private static void UpdateReferences()
    {
        GetInstance().references = GetInstance().sceneReferences.Select(r => new PrefabInfo(r.Id, r.Prefab)).ToArray();
    }

    public static GameObject GetEditorReference(string id)
    {
        return GetInstance().references.FirstOrDefault(r => r.Id == id)?.Prefab;
    }
#endif

    #endregion
}