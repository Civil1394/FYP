using System.Collections.Generic;

public class EnemyHourGlassControllerBaseImpl : HourGlassControllerBase
{
    protected override List<AbilityData> passiveEffects { get; } = new List<AbilityData>();
    protected override List<int> triggerThreshold { get; }
    protected override void OnThresholdReached()
    {
        throw new System.NotImplementedException();
    }
}