using Damage;
using UnityEngine;
using UnityEngine.UI;

public class HealthToText : MonoBehaviour
{
	[SerializeField] private HealthHandler handler;
	
	private Text text;

	protected void Awake ()
	{
		text = GetComponent<Text> ();
	}

	protected void Update ()
	{
		text.text = "Health: " + Mathf.Round (handler.GetHealth ()) +
		            (handler.MaxHealth.Get () > 0 ? "/" + Mathf.Round (handler.MaxHealth.Get ()) : "");
	}
}