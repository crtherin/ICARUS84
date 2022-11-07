using Data;
using Rewired;
using UnityEngine;

namespace Procedures
{
	public class GetButtonDown : Process, IInitialize, ICanRun
	{
		[SerializeField] private StringData button = new StringData ("Button", "");

		private Player player;

		public void Initialize ()
		{
			player = Procedure.GetComponentInParent<PlayerInput> ().GetRewiredPlayer ();
		}

		public bool CanRun ()
		{
			return button == null || player.GetButtonDown (button);
		}

		public string GetButton ()
		{
			return button;
		}

		public Player GetPlayer ()
		{
			return player;
		}
	}
}