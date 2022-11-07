using Procedures;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
	public class EnrageToText : MonoBehaviour
	{
		[SerializeField] private ProcedureHandler procedureHandler;
		
		private Text text;
		private Enrage enrage;
		private KillCharges killCharges;
		private EnrageImmediate enrageImmediate;
		private Procedure procedure;

		protected void Awake ()
		{
			text = GetComponent<Text> ();
		}

		protected void Update ()
		{
			if (procedure == null)
			{
				procedure = procedureHandler.GetProcedure("Enrage");
			}
			
			if (killCharges == null)
			{
				killCharges = procedure.GetProcess<KillCharges> ();
			}

			if (enrage == null)
			{
				enrage = procedure.GetProcess<Enrage>();
			}

			if (enrageImmediate == null)
			{
				enrageImmediate = procedure.GetProcess<EnrageImmediate>();
			}

			string remaining = "";

			if (enrage != null)
			{
				if (enrage.Remaining () > 0)
					remaining += " (" + enrage.Remaining () + ")";
			}
			else if (enrageImmediate != null)
			{
				if (enrageImmediate.Remaining () > 0)
				{
					float r = enrageImmediate.Remaining ();
					r = Mathf.Round (r * 100f) / 100f;
					remaining += " (" + r + ")";
				}
			}

			text.text = "Enrage Charges: " + killCharges.GetCharges () + remaining;
		}
	}
}