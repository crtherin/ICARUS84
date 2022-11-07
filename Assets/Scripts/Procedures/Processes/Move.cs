using Data;
using UnityEngine;

namespace Procedures
{
	public class Move : Process, IUpdate
	{
		[SerializeField] private FloatData speed = new FloatData ("Speed", 10);

		public void Update ()
		{
			Procedure.transform.position += Procedure.transform.up * speed * Time.deltaTime;
		}
	}
}