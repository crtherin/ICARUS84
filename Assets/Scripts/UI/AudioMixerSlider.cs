using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioMixerSlider : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private string parameter;
    [SerializeField] private AudioMixer mixer;

    private Slider slider;
    private float value;

    protected void Awake()
    {
        value = PlayerPrefs.GetFloat(GetName());
        slider = GetComponent<Slider>();
        slider.value = value;
        SetValue(value);
    }

    protected void OnEnable()
    {
        slider.onValueChanged.AddListener(SetValue);
    }

    protected void OnDisable()
    {
        slider.onValueChanged.RemoveListener(SetValue);
        PlayerPrefs.SetFloat(GetName(), value);
    }

    private void SetValue(float val)
    {
        mixer.SetFloat(parameter, value = val);
    }

    private string GetName()
    {
        return name + mixer + parameter;
    }
}