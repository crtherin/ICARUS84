using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class DisplayModeToggle : MonoBehaviour
{
    [SerializeField] private FullScreenMode fullScreenMode;

    private Toggle toggle;

    protected void Awake()
    {
        toggle = GetComponent<Toggle>();
    }

    protected void Start()
    {
        if (Screen.fullScreenMode == fullScreenMode)
            toggle.isOn = true;
    }

    protected void OnEnable()
    {
        toggle.onValueChanged.AddListener(ToggleOnValueChanged);
    }

    private void ToggleOnValueChanged(bool isOn)
    {
        if (isOn)
            Settings.GetInstance().SetFullScreenMode(fullScreenMode);
    }
}