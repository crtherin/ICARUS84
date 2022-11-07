using UnityEngine;
using System.Collections.Generic;

namespace Data.UI
{
	public class DescriptionPointer : MonoBehaviour
	{
		[SerializeField] private Description source;
		[SerializeField] private List<Modifier> targets;

		public void SetSource (Description source)
		{
			this.source = source;
			
			gameObject.SetActive (source != null);

			if (source != null)
				UpdateAll ();
		}

		private void UpdateAll ()
		{
			if (targets == null)
				return;

			for (int i = targets.Count - 1; i >= 0; i--)
			{
				Modifier target = targets[i];

				if (target == null)
				{
					targets.RemoveAt (i);
					continue;
				}

				target.SetData (source);
			}
		}
	}
}