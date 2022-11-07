namespace Procedures
{
    public abstract class CallbackRegister<T, V> where T : ICallback
    {
        public abstract V Invoke(T process);
    }

    public class CanRunRegister : CallbackRegister<ICanRun, bool>
    {
        public override bool Invoke(ICanRun process)
        {
            return process.CanRun();
        }
    }

    public class WaitForRegister : CallbackRegister<IWaitFor, bool>
    {
        public override bool Invoke(IWaitFor process)
        {
            return process.CanContinue();
        }
    }

    public class CooldownPercentageRegister : CallbackRegister<Cooldown, float>
    {
        public override float Invoke(Cooldown process)
        {
            return process.RemainingPercentage();
        }
    }
}