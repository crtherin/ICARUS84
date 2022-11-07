using UnityEngine;

namespace Data
{
	public abstract class DataDrivenBehaviour : MonoBehaviour, IDataDriven
	{
		[SerializeField] private Container container;

		public Node FindNode (DataField field = null)
		{
			return container == null
				? null
				: container.GetRootNode ()
					.FindOrCreate (GetType ().Name
						.AddSpacing ()
						.FirstLetterToUpper ());
		}

		protected virtual void Awake ()
		{
#if UNITY_EDITOR
			container = container.GetAssetReference ();
#endif
			this.LoadFrom (FindNode ());
		}
	}
}