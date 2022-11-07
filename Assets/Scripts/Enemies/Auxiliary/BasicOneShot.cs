using System;
using UnityEngine;

public class BasicOneShot : MonoBehaviour
{
	[SerializeField] private string animationName;

	private Animator animator;
	private new SpriteRenderer renderer;

	private Action callback;

	protected void Awake ()
	{
		animator = GetComponent<Animator> ();
		renderer = GetComponent<SpriteRenderer> ();
	}

	public void Trigger (Action action)
	{
		if (callback != null)
			callback.Invoke ();

		callback = action;

		renderer.enabled = true;
		animator.Play (animationName);
	}

	public void OneShotEnd ()
	{
		renderer.enabled = false;

		if (callback == null)
			return;

		callback.Invoke ();
		callback = null;
	}
}