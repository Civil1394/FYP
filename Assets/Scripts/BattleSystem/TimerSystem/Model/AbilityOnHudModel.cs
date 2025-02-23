using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class AbilityOnHudModel : MonoBehaviour
{
    [SerializeField] private Image iconFill;
    [SerializeField] private Image iconBg;
    [SerializeField] private float fillDuration = 0.5f;

    private int maxChargeStepsCount;
    private int chargedSteps = 0; 

    public void Init(int requireSteps, Sprite iconSprite)
    {
        this.maxChargeStepsCount = requireSteps;
        this.iconFill.sprite = iconSprite;
        this.iconBg.sprite = iconSprite;
        Reset(); 
    }

    public void NotifyUpdate(int addOnSteps)
    {
        if (chargedSteps >= maxChargeStepsCount) return;
        chargedSteps += addOnSteps;
        float targetFill = (float)chargedSteps / maxChargeStepsCount;
        
        DOTween.Kill(iconFill);
        iconFill.DOFillAmount(targetFill, fillDuration)
            .SetEase(Ease.OutQuad); // Smooth easing
    }

    public void Reset()
    {
        chargedSteps = 0;
        iconFill.fillAmount = 0;
        DOTween.Kill(iconFill); 
    }
}