using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ResolutionButton : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;

    private Button button;

    protected void Awake()
    {
        button = GetComponent<Button>();
    }

    protected void OnEnable()
    {
        button.onClick.AddListener(OnButtonClick);
    }

    protected void OnDisable()
    {
        button.onClick.RemoveListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        Settings.GetInstance().SetResolution(width, height);
    }
}