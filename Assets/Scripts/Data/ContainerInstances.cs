using System.Collections.Generic;
using UnityEngine;

namespace Data
{
	/*public class ContainerInstances : Singleton<ContainerInstances>
	{
		private readonly Dictionary<Container, Container> containers;

		protected void Awake ()
		{
			DontDestroyOnLoad (gameObject);
		}

		public Container GetContainer (Container container)
		{	
			return containers.ContainsKey (container)
				? containers[container]
				: (containers[container] = Instantiate (container));
		}
	}*/
}