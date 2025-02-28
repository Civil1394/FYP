using UnityEngine;
using DG.Tweening;
public class HourglassGlobalCanvasAnimator : HourglassAnimatorBase
{
    private Vector3 initialHourGlassRotation;
    private Vector3 initialSandPosition;

    protected override void Awake()
    {
        initialHourGlassRotation = hourGlass.localRotation.eulerAngles;
        initialSandPosition = sand.rectTransform.localPosition;
    }

    protected override Tween RotateHourGlass()
    {
        // Vector3 targetRotation = initialHourGlassRotation + new Vector3(0, 0, 180f);
        // return hourGlass.DOLocalRotate(targetRotation, 0.5f).SetEase(Ease.OutBack);
        return null;
    }

    protected override Tween AnimateSand(float duration)
    {
        
        return null;
    }

    protected override void ResetState()
    {
        hourGlass.localRotation = Quaternion.Euler(initialHourGlassRotation);
        sand.rectTransform.localPosition = initialSandPosition;
        sand.color = Color.green;
    }

    protected override void AddShakeEffect()
    {
        currentSequence.AppendCallback(() =>
        {
            Vector3 targetRotation = initialHourGlassRotation + new Vector3(0, 0, 180f);
            hourGlass.DOShakeRotation(0.5f, 10f, 10, 90f)
                .OnComplete(() => hourGlass.localRotation = Quaternion.Euler(targetRotation))
                .SetEase(Ease.OutQuad);
        });
    }
}