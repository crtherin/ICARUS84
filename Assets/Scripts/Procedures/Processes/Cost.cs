using Data;
using Abilities;
using UnityEngine;

namespace Procedures
{
    public class Cost : Process, IInitialize, ICanRun, IStart
    {
        [SerializeField] private IntData cost = new IntData("Cost", 10);
        private StaminaHandler staminaHandler;

        public void Initialize()
        {
            staminaHandler = Procedure.GetComponentInParent<StaminaHandler>();
        }

        public bool CanRun()
        {
            bool hasEnough = staminaHandler.HasEnoughStamina(cost);
            return hasEnough;
        }

        public void Start()
        {
            staminaHandler.TakeStamina(cost);
        }
    }
}