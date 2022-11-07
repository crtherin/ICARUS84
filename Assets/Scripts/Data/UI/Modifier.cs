using UnityEngine;

public abstract class Modifier : MonoBehaviour
{
	public virtual void SetData (ScriptableObject data)
	{
	}

#if UNITY_EDITOR
	protected virtual void PopulateData ()
	{
	}

	protected virtual bool PopulateDataValidate ()
	{
		return false;
	}

	[UnityEditor.MenuItem ("CONTEXT/Modifier/Populate Data", false)]
	private static void PopulateDataMenu (UnityEditor.MenuCommand command)
	{
		Modifier m = (Modifier) command.context;
		m.PopulateData ();
	}

	[UnityEditor.MenuItem ("CONTEXT/Modifier/Populate Data", true)]
	private static bool PopulateDataMenuValidate (UnityEditor.MenuCommand command)
	{
		Modifier m = (Modifier) command.context;
		return m.PopulateDataValidate ();
	}
#endif
}

public abstract class Modifier<T> : Modifier
	where T : Component
{
	[SerializeField] private ScriptableObject data;
	[SerializeField] private T target;

	protected void Start ()
	{
		if (data == null || target == null)
			return;

		IModifier<T> m = data as IModifier<T>;

		if (m != null)
			m.ApplyTo (target);
	}

	public override void SetData (ScriptableObject data)
	{
		IModifier<T> m = data as IModifier<T>;

		if (m == null)
			return;

		if (target != null)
			m.ApplyTo (target);

		this.data = data;
	}

#if UNITY_EDITOR
	protected void OnValidate ()
	{
		if (target == null)
			target = GetComponent<T> ();

		if (data == null)
			return;

		IModifier<T> m = data as IModifier<T>;

		if (m != null)
			return;

		Debug.LogWarning (
			string.Format ("Data type {0} isn't a modifier of type {1}.",
				data.GetType ().Name,
				typeof (T).Name));

		data = null;
	}

	protected override void PopulateData ()
	{
		IModifier<T> m = data as IModifier<T>;

		if (m != null)
			m.FillFrom (target);
	}

	protected override bool PopulateDataValidate ()
	{
		return target != null && data is IModifier<T>;
	}
#endif
}