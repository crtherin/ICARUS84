using Procedures;
using UnityEngine;

public class AbilityHandler : MonoBehaviour, IPersistent<int>
{
    [SerializeField] private ProcedureHandler procedurelHandler;
    [SerializeField] private Transform skillPointsEffect;
    [SerializeField] private int skillPoints;

#if UNITY_EDITOR
    protected void OnValidate()
    {
        if (procedurelHandler == null)
            GetComponentInChildren<ProcedureHandler>();
    }
#endif

    public int SkillPointsCount()
    {
        return skillPoints;
    }

    public void AddSkillPoints(int points)
    {
        if (points <= 0)
            return;

        skillPoints += points;

        if (skillPointsEffect)
        {
            Transform spawned = Instantiate(skillPointsEffect, transform.position, Quaternion.identity);
            spawned.parent = transform;
        }
    }

    public void TogglePoints(int diff)
    {
        PlayerPrefs.SetInt("reservedPoints", PlayerPrefs.GetInt("reservedPoints", 0) + diff);
        skillPoints -= diff;
    }

    public int Save()
    {
        return skillPoints;
    }

    public void Load(int data)
    {
        skillPoints = data; // - PlayerPrefs.GetInt("reservedPoints", 0);
    }
}