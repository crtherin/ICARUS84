using UnityEngine;
using UnityEngine.UI;

public interface IModifier<T>
{
	void ApplyTo (T target);
	void FillFrom (T source);
}

public interface IText : IModifier<Text>
{
}

public interface IRectTransform : IModifier<RectTransform>
{
}

[CreateAssetMenu (menuName = "Data/Other/Description")]
public class Description : ScriptableObject, IText, IRectTransform
{
	[SerializeField] [Multiline] private string text;
	[SerializeField] private float width;
	[SerializeField] private float height;

	public void ApplyTo (Text target)
	{
		target.text = text;
	}
	public void FillFrom (Text source)
	{
		text = source.text;
	}

	public void ApplyTo (RectTransform target)
	{
		target.sizeDelta = new Vector2 (width, height);
	}

	public void FillFrom (RectTransform source)
	{
		width = source.sizeDelta.x;
		height = source.sizeDelta.y;
	}
}