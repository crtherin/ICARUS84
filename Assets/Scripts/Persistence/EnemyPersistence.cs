using Damage;
using System;
using UnityEngine;

public class EnemyPersistence : Persistence<EnemyPersistence.Data>
{
    [Serializable]
    public struct Data
    {
        public bool Enabled;
        public Vector3 Position;
        public Vector3 Rotation;
        public float Health;

        public Data(bool enabled, Vector3 position, Vector3 rotation, float health)
        {
            Enabled = enabled;
            Position = position;
            Rotation = rotation;
            Health = health;
        }
    }

    [SerializeField] private MonoBehaviour behaviour;
    [SerializeField] private HealthHandler healthHandler;

    protected void OnEnable()
    {
        healthHandler.Death -= HealthHandlerOnDeath;
        healthHandler.Death += HealthHandlerOnDeath;
    }

    protected void OnDisable()
    {
        healthHandler.Death -= HealthHandlerOnDeath;
    }

    private void HealthHandlerOnDeath(object sender, DamageInfo e)
    {
        Stage();
    }
    
    public override Data ExportData()
    {
        return new Data(
            behaviour.enabled,
            transform.position, 
            transform.localEulerAngles,
            healthHandler.GetHealth());
    }

    public override void ImportData(Data data)
    {
        if (data.Health <= 0)
            Destroy(gameObject);

        behaviour.enabled = data.Enabled;
        transform.position = data.Position;
        transform.localEulerAngles = data.Rotation;
        healthHandler.Load(data.Health);
    }

#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();
        healthHandler = GetComponent<HealthHandler>();
    }
#endif
}