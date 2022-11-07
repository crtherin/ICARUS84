using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class ShieldToText : MonoBehaviour
	{
		[SerializeField] private ShieldHandler handler;
		
		private Text text;

		protected void Awake ()
		{
			text = GetComponent<Text> ();
		}

		protected void Update ()
		{
			text.text = !handler.UseShield
				? ""
				: "Shield: " + handler.ShieldCount () + "/" + handler.MaxShieldCount.Get ();
		}
	}
}