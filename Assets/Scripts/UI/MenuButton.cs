using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    [Serializable]
    private class Sprites
    {
        [SerializeField] private Sprite idle;
        [SerializeField] private Sprite highlighted;
        [SerializeField] private Sprite pressed;
        [SerializeField] private Sprite disabled;

        public Sprite Idle
        {
            get { return idle; }
        }

        public Sprite Highlighted
        {
            get { return highlighted; }
        }

        public Sprite Pressed
        {
            get { return pressed; }
        }

        public Sprite Disabled
        {
            get { return disabled; }
        }
    }

    private enum State
    {
        Idle,
        Highlighted,
        Pressed,
        Disabled
    }

    [SerializeField] private Sprites sprites;
    [SerializeField] private UnityEvent onPointerClick;

    public event EventHandler OnClick;

    private State state;
    private Image image;

    protected void Awake()
    {
        image = GetComponent<Image>();
    }

    protected void OnEnable()
    {
        Refresh(IsEnabled());
        UpdateSprite();
    }

    protected virtual bool IsEnabled()
    {
        return true;
    }

    protected virtual void Click()
    {
        if (onPointerClick != null)
            onPointerClick.Invoke();
        
        OnClick.SafeInvoke(this, null);
    }

    private void Refresh(bool isEnabled)
    {
        state = isEnabled ? State.Idle : State.Disabled;
        image.raycastTarget = isEnabled;
    }

    private void UpdateSprite()
    {
        image.sprite = GetSprite();
    }

    private Sprite GetSprite()
    {
        switch ((int) state)
        {
            default: return sprites.Disabled;
            case 0: return sprites.Idle;
            case 1: return sprites.Highlighted;
            case 2: return sprites.Pressed;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        state = State.Highlighted;
        UpdateSprite();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        state = State.Idle;
        UpdateSprite();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (state != State.Highlighted)
            return;

        state = State.Pressed;
        UpdateSprite();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (state != State.Pressed)
            return;

        state = State.Highlighted;
        UpdateSprite();
        Click();
    }
}