using UnityEngine;

[RequireComponent (typeof (Animator))]
[RequireComponent (typeof (SpriteRenderer))]
public class PlayerAnimator : MonoBehaviour
{
	private Animator animator;
	private new Rigidbody2D rigidbody;

	protected void Awake ()
	{
		animator = GetComponent<Animator> ();
		rigidbody = GetComponentInParent<Rigidbody2D> ();
	}

	protected void Update ()
	{
		animator.SetFloat ("Speed", rigidbody.velocity.magnitude);
	}
}