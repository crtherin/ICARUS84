using Damage;
using System;
using Abilities;
using UnityEngine;

public class PlayerPersistence : Persistence<PlayerPersistence.Data>
{
    [Serializable]
    public struct Data
    {
        public Vector3 Checkpoint;
        public float Health;
        public float Stamina;
        public int SkillPoints;

        public Data(Vector3 checkpoint, float health, float stamina, int skillPoints)
        {
            Checkpoint = checkpoint;
            Health = health;
            Stamina = stamina;
            SkillPoints = skillPoints;
        }
    }

    [SerializeField] private HealthHandler healthHandler;
    [SerializeField] private StaminaHandler staminaHandler;
    [SerializeField] private AbilityHandler abilityHandler;
    private Vector3 checkpoint;

#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();
        healthHandler = GetComponent<HealthHandler>();
        staminaHandler = GetComponent<StaminaHandler>();
        abilityHandler = GetComponent<AbilityHandler>();
    }
#endif

    protected override void Start()
    {
        base.Start();
        healthHandler.Death += HealthHandlerOnDeath;
    }

    public override Data ExportData()
    {
        return new Data(transform.position, healthHandler.Save(), staminaHandler.Save(), abilityHandler.Save());
    }

    public override void ImportData(Data data)
    {
        transform.position = data.Checkpoint;
        healthHandler.Load(data.Health);
        staminaHandler.Load(data.Stamina);
        abilityHandler.Load(data.SkillPoints);
    }

    private void HealthHandlerOnDeath(object sender, DamageInfo e)
    {
        SceneManager.GetInstance().Load(SceneManager.Menu, 3);
    }

    /*public void SetCheckpoint(Checkpoint checkpoint)
    {
        true.Save("alive");
        this.checkpoint = checkpoint.transform.position;
    }
    
    public override void Save()
    {
        GetData().Save(Key);
    }*/

    /*public override void Load()
    {
        string key = Key;
        
        if (!key.Exists())
            return;
        
        Data data = key.Load<Data>();
        transform.position = data.Checkpoint;
        healthHandler.Load(data.Health);
        staminaHandler.Load(data.Stamina);
        abilityHandler.Load(data.SkillPoints);
    }*/

    /*private Data GetData()
    {
        return new Data(checkpoint, healthHandler.Save(), staminaHandler.Save(), abilityHandler.Save());
    }*/
}