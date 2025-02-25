using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class AbilityOnHudModel : MonoBehaviour
{
    [SerializeField] private Image iconFill;
    [SerializeField] private Image iconBg;
    [SerializeField] private Button iconButton;
    [SerializeField] private float fillDuration = 0.5f;

    private HexDirection direction;
    private int maxChargeStepsCount;
    private int chargedSteps = 0;
    private AbilityColorType abilityColor;
    private bool fullyCharged = false;

    private Action<HexDirection> onDirectionCharged;
    public void Init(HexDirection hexDirection,int requireSteps, Sprite iconSprite,AbilityColorType colorType, Action<HexDirection> OnFullyCharged)
    {
        this.direction = hexDirection;
        this.maxChargeStepsCount = requireSteps;
        this.iconFill.sprite = iconSprite;
        this.iconBg.sprite = iconSprite;

        this.iconFill.color = AbilityColorHelper.GetAbilityColor(colorType);
        //this.iconButton.onClick.RemoveAllListeners();
        //this.iconButton.onClick.AddListener(() => HandleChargeCompletion(OnFullyCharged));
        onDirectionCharged = OnFullyCharged;
        Reset();
    }

    private void HandleChargeCompletion(Action<HexDirection> fullyChargedCallback)
    {
        if (!fullyCharged) return;
        fullyChargedCallback?.Invoke(direction);
        Reset();
    }

    public void NotifyUpdate(int addOnSteps)
    {
        if (fullyCharged) return;
        
        chargedSteps = Math.Clamp(chargedSteps + addOnSteps, 0, maxChargeStepsCount);
        float targetFill = (float)chargedSteps / maxChargeStepsCount;

        DOTween.Kill(iconFill);
        iconFill.DOFillAmount(targetFill, fillDuration)
            .SetEase(Ease.OutQuad).OnComplete((() =>
            {
                if (chargedSteps >= maxChargeStepsCount) fullyCharged = true;
                HandleChargeCompletion(onDirectionCharged);
            }));
    }

    public void Reset()
    {
        chargedSteps = 0;
        iconFill.fillAmount = 0;
        fullyCharged = false;
        DOTween.Kill(iconFill);
    }
}

  