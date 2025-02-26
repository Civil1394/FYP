using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.Mathematics;

public class HourglassOnHudAnimator : HourglassAnimatorBase
{
    private RectTransform rectTransform;
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    protected override Tween RotateHourGlass()
    {
        return hourGlass.DORotate(new Vector3(0, 0, 180f), 0.5f).SetEase(Ease.OutBack);
    }

    protected override Tween AnimateSand(float duration)
    {
        
        float sandDistance = rectTransform.rect.height;
        return sand.rectTransform.DOAnchorPosY(sandDistance, duration)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                float progress = 1 - Mathf.Abs(sand.rectTransform.anchoredPosition.y / sandDistance);
                UpdateSandColor(progress);
            });
    }

    protected override void ResetState()
    {
        hourGlass.rotation = Quaternion.identity;
        sand.rectTransform.anchoredPosition = Vector2.zero;
        sand.color = Color.green;
    }
}