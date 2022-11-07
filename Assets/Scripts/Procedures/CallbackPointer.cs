namespace Procedures
{
    public abstract class CallbackPointer<T> where T : ICallback
    {
        public abstract void Invoke(T process);
    }

    public abstract class CallbackPointer<T, V> where T : ICallback
    {
        public abstract void Invoke(T process, V value);
    }
    
    public class InitializePointer : CallbackPointer<IInitialize>
    {
        public override void Invoke(IInitialize process)
        {
            process.Initialize();
        }
    }

    public class RefreshPointer : CallbackPointer<IRefresh>
    {
        public override void Invoke(IRefresh process)
        {
            process.Refresh();
        }
    }
    
    public class StartPointer : CallbackPointer<IStart>
    {
        public override void Invoke(IStart process)
        {
            process.Start();
        }
    }

    public class UpdatePointer : CallbackPointer<IUpdate>
    {
        public override void Invoke(IUpdate process)
        {
            process.Update();
        }
    }
    
    public class StopPointer : CallbackPointer<IStop>
    {
        public override void Invoke(IStop process)
        {
            process.Stop();
        }
    }

    public class EnabledPointer : CallbackPointer<IEnabled>
    {
        public override void Invoke(IEnabled process)
        {
            process.Enabled();
        }
    }

    public class DisabledPointer : CallbackPointer<IDisabled>
    {
        public override void Invoke(IDisabled process)
        {
            process.Disabled();
        }
    }

    public class PassiveUpdatePointer : CallbackPointer<IPassiveUpdate>
    {
        public override void Invoke(IPassiveUpdate process)
        {
            process.PassiveUpdate();
        }
    }
}