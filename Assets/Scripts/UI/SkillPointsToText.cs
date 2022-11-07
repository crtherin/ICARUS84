using UnityEngine;
using UnityEngine.UI;

public class SkillPointsToText : MonoBehaviour
{
	[SerializeField] private string prefix = "Coins";
	[SerializeField] private AbilityHandler handler;

	private Text text;

	protected void Awake ()
	{
		text = GetComponent<Text> ();
	}

	protected void Update ()
	{
		text.text = prefix + ": " + Mathf.RoundToInt (handler.SkillPointsCount ());
	}
}