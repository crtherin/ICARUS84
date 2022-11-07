using System;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class PersistenceBase : MonoBehaviour
{
    [SerializeField] private StringID instanceID = new StringID();
    
    private PersistenceManager persistenceManager;
    
    protected bool HasKey => !string.IsNullOrEmpty(instanceID.Value);
    protected string Key => instanceID.Value;
    protected PersistenceManager PersistenceManager => persistenceManager;

    protected void Awake()
    {
        persistenceManager = FindObjectOfType<PersistenceManager>();
    }

    public abstract void Stage();

#if UNITY_EDITOR
    public void Initialize()
    {
        instanceID = new StringID();
        instanceID.Initialize();
        UnityEditor.EditorUtility.SetDirty(this);
    }

    public void Clear()
    {
        instanceID.Clear();
        UnityEditor.EditorUtility.SetDirty(this);
    }

    protected virtual void Reset()
    {
        persistenceManager = FindObjectOfType<PersistenceManager>();
    }
#endif
}