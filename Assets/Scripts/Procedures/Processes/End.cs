namespace Procedures
{
	public class End : Process, IUpdate
	{
		public void Update ()
		{
			Procedure.Stop ();
		}
	}
}