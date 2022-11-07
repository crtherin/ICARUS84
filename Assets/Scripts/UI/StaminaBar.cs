using Abilities;
using UnityEngine;

public class StaminaBar : SmoothBar
{
    [SerializeField] private StaminaHandler handler;
    
    protected override float GetFill()
    {
        return handler.GetStamina() / handler.MaxStamina.Get();
    }
}