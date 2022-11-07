using UnityEngine;
using UnityEngine.UI;

public abstract class SmoothBar : MonoBehaviour
{
    [SerializeField] private float smoothTime = 0.2f;
    [SerializeField] private float maxSpeed = 1f;

    private Image image;
    private float fill;
    private float fillVelocity;

    protected void Awake()
    {
        image = GetComponent<Image>();
    }

    protected void Start()
    {
        // fill = GetFill();
        fill = 1;
        image.fillAmount = fill;
    }

    protected void Update()
    {
        fill = Mathf.SmoothDamp(fill, GetFill(), ref fillVelocity, smoothTime, maxSpeed);
        image.fillAmount = fill;
    }

    protected abstract float GetFill();
}