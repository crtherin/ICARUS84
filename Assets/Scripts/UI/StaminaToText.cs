using Abilities;
using UnityEngine;
using UnityEngine.UI;

public class StaminaToText : MonoBehaviour
{
	[SerializeField] private StaminaHandler handler;
	
	private Text text;

	protected void Awake ()
	{
		text = GetComponent<Text> ();
	}

	protected void Update ()
	{
		text.text = "Stamina: " + handler.GetStamina () + "/" + handler.MaxStamina.Get ();
	}
}