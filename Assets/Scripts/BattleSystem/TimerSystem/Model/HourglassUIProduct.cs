using System;
using UnityEngine;
using System.Collections;

public class HourglassUIProduct : TimedActor
{
	private HourglassUIAnimator hourglassUIAnimator;

	public override void Init(Hourglass hourglass)
	{
		hourglassUIAnimator = this.GetComponent<HourglassUIAnimator>();
		if (hourglassUIAnimator != null)
		{
			OnTimerStart += hourglassUIAnimator.CountTime;
		}
		base.Init(hourglass);
		
	}

	private void OnDestroy()
	{
		if (hourglassUIAnimator != null)
		{
			OnTimerStart -= hourglassUIAnimator.CountTime;
		}
	}
}