using System;
using Procedures;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[Serializable]
public class AbilityTreeNodeReference
{
    [SerializeField] private Procedure procedure;
    [SerializeField] private int variation = -1;
    [SerializeField] private int upgrade = 0;

    public VariationTree GetTree()
    {
        return procedure.GetTree();
    }

    public Procedure Procedure => procedure;

    public int Variation => variation;

    public int Upgrade => upgrade;

    public int RequiredPoints()
    {
        if (variation < 0)
            return 0;

        if (upgrade < 1)
            return 1;

        return 2;
    }
}

[Serializable]
public class AbilityTreeButtonStates
{
    [SerializeField] private Sprite normal;
    [SerializeField] private Sprite available;
    [SerializeField] private Sprite selected;

    public Sprite Normal
    {
        get { return normal; }
    }

    public Sprite Available
    {
        get { return available; }
    }

    public Sprite Selected
    {
        get { return selected; }
    }
}

public class AbilityTreeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private const float Duration = 0.25f;
    private const float MinAlpha = 0.75f;

    [SerializeField] private AbilityTreeNodeReference treeNode;
    [SerializeField] private AbilityTreeButtonStates states;
    [SerializeField] private UnityEvent onPointerEnter;
    [SerializeField] private Image descriptionTarget;
    [SerializeField] private Sprite description;

    private Image image;
    private AbilityTree abilityTree;
    private AbilityHandler abilityHandler;

    private int state; // 0 = normal, 1 = available, 2 = selected

    private Coroutine coroutine;
    private Color color;
    private float t;

    protected void Awake()
    {
        image = GetComponent<Image>();
        abilityTree = GetComponentInParent<AbilityTree>();
        abilityHandler = GetComponentInParent<AbilityHandler>();
        color = image.color;
    }

    protected void OnEnable()
    {
        Refresh();
        TryStopCoroutine();
        UpdateAlpha(t = 0);
        abilityTree.Refresh += Refresh;
    }

    protected void Start()
    {
        image.alphaHitTestMinimumThreshold = 0.9f;
    }

    protected void OnDisable()
    {
        abilityTree.Refresh -= Refresh;
    }

    private void Refresh()
    {
        int skillPoints = abilityHandler.SkillPointsCount();
        int requiredPoints = treeNode.RequiredPoints();
        bool available = skillPoints >= requiredPoints;

        VariationTree tree = treeNode.GetTree();
        int selectedVariation = tree.GetSelection();

        if (selectedVariation < 0)
        {
            SetState(available ? 1 : 0);
            return;
        }

        if (selectedVariation != treeNode.Variation)
        {
            SetState(0);
            return;
        }

        if (treeNode.Upgrade < 1)
        {
            SetState(2);
            return;
        }

        int selectedUpgrade = tree.GetVariation(selectedVariation).GetUpgrade();

        if (selectedUpgrade < 1)
        {
            SetState(skillPoints >= 1 ? 1 : 0);
            return;
        }

        SetState(selectedUpgrade == treeNode.Upgrade ? 2 : 0);
    }

    private void SetState(int state)
    {
        this.state = state;

        if (state == 0)
            image.sprite = states.Normal;
        else if (state == 1)
            image.sprite = states.Available;
        else if (state == 2)
            image.sprite = states.Selected;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TryStopCoroutine();
        onPointerEnter.Invoke();
        coroutine = StartCoroutine(AnimateTowards(1, Animation()));

        if (descriptionTarget != null)
        {
            descriptionTarget.sprite = description;
            descriptionTarget.enabled = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TryStopCoroutine();
        coroutine = StartCoroutine(AnimateTowards(0, null));
        
        if (descriptionTarget != null && descriptionTarget.sprite == description)
        {
            descriptionTarget.enabled = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (treeNode.Variation < 0)
            return;

        if (state == 0)
            return;

        if (state == 1)
        {
            if (treeNode.Upgrade < 1)
            {
                abilityHandler.TogglePoints(1);
                treeNode.GetTree().SelectVariation(treeNode.Variation);
                treeNode.GetTree().GetVariation(treeNode.Variation).SetUpgrade(treeNode.Upgrade);
                abilityTree.OnRefresh();
                treeNode.Procedure.Save();
            }
            else
            {
                abilityHandler.TogglePoints(treeNode.GetTree().GetSelection() == treeNode.Variation ? 1 : 2);
                treeNode.GetTree().SelectVariation(treeNode.Variation);
                treeNode.GetTree().GetVariation(treeNode.Variation).SetUpgrade(treeNode.Upgrade);
                abilityTree.OnRefresh();
                treeNode.Procedure.Save();
            }

            return;
        }

        if (state == 2)
        {
            if (treeNode.Upgrade > 0 || treeNode.GetTree().GetVariation(treeNode.Variation).GetUpgrade() > 0)
            {
                abilityHandler.TogglePoints(-1);
                treeNode.GetTree().GetVariation(treeNode.Variation).SetUpgrade(0);
                abilityTree.OnRefresh();
                treeNode.Procedure.Save();
            }

            if (treeNode.Upgrade < 1)
            {
                abilityHandler.TogglePoints(-1);
                treeNode.GetTree().SelectVariation(-1);
                abilityTree.OnRefresh();
                treeNode.Procedure.Save();
            }
        }
    }

    private IEnumerator AnimateTowards(float v, IEnumerator next)
    {
        float sign = Mathf.Sign(v - t);

        while (Mathf.Abs(sign) > 0)
        {
            t += Time.deltaTime / Duration * sign;

            float newSign = Mathf.Sign(v - t);
            if ((sign > 0 && newSign < 0) || (sign < 0 && newSign > 0))
            {
                t = v;
                UpdateAlpha();
                break;
            }

            UpdateAlpha();
            yield return null;
        }

        if (next != null)
            coroutine = StartCoroutine(next);
    }

    private IEnumerator Animation()
    {
        while (true)
        {
            t += Time.deltaTime / Duration;

            if (t > 2)
                t -= 2;

            float mirroredTime = t < 1 ? t : 2 - t;
            UpdateAlpha(mirroredTime);
            yield return null;
        }
    }

    private void UpdateAlpha(float t)
    {
        color.a = Mathf.Lerp(1, MinAlpha, t);
        image.color = color;
    }

    private void UpdateAlpha()
    {
        UpdateAlpha(t);
    }

    private void TryStopCoroutine()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }
}