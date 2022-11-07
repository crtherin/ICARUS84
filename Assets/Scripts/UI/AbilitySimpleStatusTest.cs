using UnityEngine;
using UnityEngine.UI;
using Procedures;
using System.Collections.Generic;

public class AbilitySimpleStatusTest : MonoBehaviour
{
    /*[SerializeField] private Procedure ability;
    [SerializeField] private CanvasGroup running;
    [SerializeField] private CanvasGroup cross;
    [SerializeField] private Image cooldownRadial;

    private Receiver<ICanRun, bool> canRunReceiver;
    private Receiver<Cooldown, float> cooldownReceiver;

    private void Start()
    {
        canRunReceiver = new Receiver<ICanRun, bool>(new CanRunRegister(), new AndMultiplier());
        cooldownReceiver = new Receiver<Cooldown, float>(new CooldownPercentageRegister(), new FloatMultiplier());

        List<Process> processes = ability.GetProcesses();        

        for (var i = 0; i < processes.Count; i++)
        {
            if (processes[i] is ICanRun canRunProcess && (canRunProcess is Exclusive || canRunProcess is Cost))
            {
                canRunReceiver.Add(canRunProcess);
            }
            
            if (processes[i] is Cooldown process)
            {
                cooldownReceiver.Add(process);
            }
        }
    }

    protected void Update()
    {
        running.alpha = ability.IsRunning ? 1 : 0;
        cross.alpha = canRunReceiver.Receive() ? 0 : 1;
        cooldownRadial.fillAmount = cooldownReceiver.Receive();
    }*/
}
