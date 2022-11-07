using UnityEngine;

public class MenuNavigation : MonoBehaviour
{
	[SerializeField] private bool isInteractable;

	[System.Serializable]
	public class Node
	{
		[SerializeField] private string button;
		[SerializeField] private MenuNavigation target;
	}
}

public interface INavigationOnEnter
{
	void NavigationOnEnter ();
}

public interface INavigationOnExit
{
	void NavigationOnExit ();
}

public interface INavigationOnSelect
{
	void NavigationOnSelect ();
}