using Data;
using UnityEngine;

namespace Procedures
{
	public class PlayAnimatorState : Process, IInitialize, IStart, IUpdate
	{
		[SerializeField] private StringData state = new StringData ("State", "");

		private Animator animator;
		private bool canTrigger;

		public void Initialize ()
		{
			animator = Procedure.GetComponent<Animator> ();
		}

		public void Start ()
		{
			canTrigger = true;
		}

		public void Update ()
		{
			if (!canTrigger)
				return;

			canTrigger = false;
			animator.Play (state);
		}
	}
}