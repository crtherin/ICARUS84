using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEnterEvent : MonoBehaviour
{
	[SerializeField] private UnityEvent triggerEnter;

	protected void OnTriggerEnter (Collider c)
	{
		if (triggerEnter != null)
			triggerEnter.Invoke ();
	}
}