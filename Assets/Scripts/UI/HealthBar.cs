using Damage;
using UnityEngine;

public class HealthBar : SmoothBar
{
    [SerializeField] private HealthHandler handler;

    protected override float GetFill()
    {
        return handler.GetHealth() / handler.MaxHealth.Get();
    }
}