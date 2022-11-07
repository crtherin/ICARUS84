using System.Collections.Generic;

namespace Procedures
{
    public abstract class Transmission<T> where T : ICallback
    {
        protected HashSet<T> endpoints;

        protected Transmission()
        {
            endpoints = new HashSet<T>();
        }

        public void Add(T endpoint)
        {
            if (endpoints.Contains(endpoint))
                return;

            endpoints.Add(endpoint);
        }

        public void Remove(T endpoint)
        {
            if (!endpoints.Contains(endpoint))
                return;

            endpoints.Remove(endpoint);
        }
    }
	
    public class Broadcaster<T> : Transmission<T> where T : ICallback
    {
        private CallbackPointer<T> pointer;
		
        public Broadcaster(CallbackPointer<T> pointer)
        {
            this.pointer = pointer;
        }
		
        public void Broadcast()
        {
            foreach (T endpoint in endpoints)
            {
                if (endpoint.IsEnabled())
                    continue;
				
                pointer.Invoke(endpoint);
            }
        }
    }

    public class Broadcaster<T, V> : Transmission<T> where T : ICallback
    {
        private CallbackPointer<T, V> pointer;

        public Broadcaster(CallbackPointer<T, V> pointer)
        {
            this.pointer = pointer;
        }

        public void Broadcast(V value)
        {
            foreach (T endpoint in endpoints)
            {
                if (!endpoint.IsEnabled())
                    continue;
				
                pointer.Invoke(endpoint, value);
            }
        }
    }

    public class Receiver<T, V> : Transmission<T> where T : ICallback
    {
        private CallbackRegister<T, V> register;
        private ValueMultiplier<V> multiplier;

        public Receiver(CallbackRegister<T, V> register, ValueMultiplier<V> multiplier)
        {
            this.register = register;
            this.multiplier = multiplier;
        }

        public V Receive()
        {
            V value = default;
			
            bool isFirst = true;

            foreach (T endpoint in endpoints)
            {
                if (!endpoint.IsEnabled())
                    continue;

                V output = register.Invoke(endpoint);

                if (isFirst)
                {
                    value = output;
                    isFirst = false;
                    continue;
                }

                value = multiplier.Multiply(value, output);
            }

            return value;
        }
    }
}