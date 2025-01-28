using UnityEngine;
using DG.Tweening;
public class GlobalCanvasHourGlass : HourGlassViewBase
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
        Vector3 targetRotation = initialHourGlassRotation + new Vector3(0, 0, 180f);
        return hourGlass.DOLocalRotate(targetRotation, 0.5f).SetEase(Ease.OutBack);
    }

    protected override Tween AnimateSand(float duration)
    {
        float sandDistance = 3f;
        Vector3 startPos = initialSandPosition;
        Vector3 endPos = startPos + Vector3.up * sandDistance;

        return sand.rectTransform.DOLocalMove(endPos, duration)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                float progress = Vector3.Distance(sand.rectTransform.localPosition, startPos) / sandDistance;
                UpdateSandColor(1 - progress);
            });
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