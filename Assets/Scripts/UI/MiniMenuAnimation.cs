using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Net.Mime;

public class MiniMenuAnimation : MonoBehaviour
{
    [Serializable]
    class AnimationSegment
    {
        [SerializeField] private float duration;
        [SerializeField] private AnimationCurve curve;

        private Action<float> setter;

        public float Duration
        {
            get { return duration; }
        }

        public AnimationCurve Curve
        {
            get { return curve; }
        }
    }

    [SerializeField] private AnimationSegment drop;
    [SerializeField] private AnimationSegment rise;
    [SerializeField] private AnimationSegment fade;

    [Header("References")]
    [SerializeField] private RectTransform header;
    [SerializeField] private RectTransform original;
    [SerializeField] private RectTransform target;
    [SerializeField] private RectTransform back;
    [SerializeField] private CanvasGroup contents;
    [SerializeField] private Image background;
    [SerializeField] private Image blocker;
    
    private Coroutine activeSegment;
    private IEnumerator mirrorSegment;

    private float t;
    private Image backImage;
    private MenuButton backButton;

#if UNITY_EDITOR
    protected void Reset()
    {
        back = SafeGetComponent<RectTransform>(transform.Find("Back"));
        header = SafeGetComponent<RectTransform>(transform.Find("Header"));
        contents = SafeGetComponent<CanvasGroup>(transform.Find("Contents"));
        background = SafeGetComponent<Image>(transform.Find("Background"));
    }

    private T SafeGetComponent<T>(Transform target) where T : Component
    {
        if (target == null)
            return null;

        return target.GetComponent<T>();
    }
#endif

    protected void Awake()
    {
        backImage = back.GetComponent<Image>();
        backButton = back.GetComponent<MenuButton>();
    }

    protected void OnEnable()
    {
        backButton.OnClick += BackMenuButtonOnOnClick;
        SetDefault();
        StartAnimation();
    }

    protected void OnDisable()
    {
        backButton.OnClick -= BackMenuButtonOnOnClick;
    }

    private void BackMenuButtonOnOnClick(object sender, EventArgs e)
    {
        backImage.raycastTarget = false;
        ReverseAnimation();
    }

    private void SetDefault()
    {
        header.position = original.position;
        back.gameObject.SetActive(false);
        backImage.raycastTarget = true;
        contents.interactable = false;
        contents.alpha = 0;
        background.fillAmount = 0;
        blocker.raycastTarget = true;
    }

    private void StartAnimation()
    {
        t = 0;
        activeSegment = StartCoroutine(Drop());
        mirrorSegment = null;
    }

    private void ReverseAnimation()
    {
        if (mirrorSegment != null)
        {
            if (activeSegment != null)
                StopCoroutine(activeSegment);
            
            activeSegment = StartCoroutine(mirrorSegment);
        }
    }

    private float IncrementTime(float t, out float newT, float duration, AnimationCurve curve, int direction = 1)
    {
        newT = t + Time.unscaledDeltaTime / duration * direction;
        return curve.Evaluate(Mathf.Clamp01(newT));
    }

    private IEnumerator Drop()
    {
        original.gameObject.SetActive(false);

        while (t < 1)
        {
            float smoothT = IncrementTime(t, out t, drop.Duration, drop.Curve);
            header.position = Vector2.Lerp(original.position, back.position, smoothT);
            yield return null;
        }

        t = 0;
        activeSegment = StartCoroutine(Rise());
        mirrorSegment = RiseOut();
    }

    private IEnumerator Rise()
    {
        back.gameObject.SetActive(true);

        while (t < 1)
        {
            float smoothT = IncrementTime(t, out t, rise.Duration, rise.Curve);
            header.position = Vector2.Lerp(back.position, target.position, smoothT);
            background.fillAmount = smoothT;
            contents.alpha = smoothT;
            yield return null;
        }

        header.position = target.position;
        background.fillAmount = 1;
        
        contents.interactable = true;
        contents.alpha = 1;
        
        //t = 0;
        //activeSegment = StartCoroutine(Fade());
        //mirrorSegment = RiseOut();
    }

    private IEnumerator Fade()
    {
        contents.interactable = true;

        while (t < 1)
        {
            contents.alpha = IncrementTime(t, out t, fade.Duration, fade.Curve);
            yield return null;
        }

        contents.alpha = 1;
    }

    /*private IEnumerator FadeOut()
    {
        contents.interactable = false;

        while (t > 0)
        {
            contents.alpha = IncrementTime(t, out t, fade.Duration, fade.Curve, -1);
            yield return null;
        }

        contents.alpha = 0;

        t = 1;
        activeSegment = StartCoroutine(RiseOut());
        mirrorSegment = null;
    }*/

    private IEnumerator RiseOut()
    {
        blocker.raycastTarget = false;
        contents.interactable = false;
        
        while (t > 0)
        {
            float smoothT = IncrementTime(t, out t, rise.Duration, rise.Curve, -1);
            header.position = Vector2.Lerp(back.position, target.position, smoothT);
            background.fillAmount = smoothT;
            contents.alpha = smoothT;
            yield return null;
        }

        back.gameObject.SetActive(false);

        t = 1;
        activeSegment = StartCoroutine(DropOut());
        mirrorSegment = null;
    }

    private IEnumerator DropOut()
    {
        blocker.raycastTarget = false;
        
        while (t > 0)
        {
            float smoothT = IncrementTime(t, out t, drop.Duration, drop.Curve, -1);
            header.position = Vector2.Lerp(original.position, back.position, smoothT);
            yield return null;
        }

        t = 0;
        activeSegment = null;
        mirrorSegment = null;
        original.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}