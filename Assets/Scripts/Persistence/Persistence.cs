using UnityEngine;

[DisallowMultipleComponent]
public abstract class Persistence<T> : PersistenceBase
{
    protected virtual void Start()
    {
        if (HasKey && Key.Exists())
            ImportData(Key.Load<T>());
    }

    public override void Stage()
    {
        if (PersistenceManager == null)
        {
            Debug.Log("Persistence Manager Null", this);
            return;
        }
        
        PersistenceManager.Stage(Key, ExportData());
    }

    public abstract T ExportData();

    public abstract void ImportData(T data);
}