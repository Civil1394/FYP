using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public abstract class HourglassAnimatorBase : MonoBehaviour 
{
	protected Sequence currentSequence;
	[SerializeField] protected RectTransform hourGlass;
	[SerializeField] protected Image sand;
	private float alpha;
	protected virtual void Awake()
	{
		alpha = sand.color.a;
	}
	protected virtual void OnDestroy()
	{
		currentSequence?.Kill();
	}

	public virtual void CountTime(float duration)
	{
		currentSequence?.Kill();
		ResetState();
		currentSequence = DOTween.Sequence();

		currentSequence.Join(RotateHourGlass());
		currentSequence.Join(AnimateSand(duration));
		AddShakeEffect();
	}

	protected abstract Tween RotateHourGlass();
	protected abstract Tween AnimateSand(float duration);

	protected virtual void AddShakeEffect()
	{
		currentSequence.AppendCallback(() =>
		{
			hourGlass.DOShakeRotation(0.5f, 10f, 10, 90f)
				.SetEase(Ease.OutQuad);
		});
	}

	public virtual void StopAnimation()
	{
		currentSequence?.Kill();
		ResetState();
	}

	protected abstract void ResetState();

	protected void UpdateSandColor(float progress)
	{
		Color newColor = progress >= 0.5f 
			? Color.Lerp(Color.yellow, Color.green, (progress - 0.5f) * 2f)
			: Color.Lerp(Color.red, Color.yellow, progress * 2f);
        
		sand.color = newColor;
	}
}