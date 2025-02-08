using System.Collections.Generic;
using UnityEngine;

public class PlayerHourGlassController : MonoBehaviour
{
    protected  List<AbilityData> passiveEffects { get; } = new List<AbilityData>();
    protected  List<int> triggerThreshold { get; private set; }

    public void Initialize(List<int> triggerThreshold)
    {
        this.triggerThreshold = triggerThreshold;
    }
    private void OnThresholdReached(int value)
    {
        //passiveEffects[value].TriggerAbility(BattleManager.Instance.PlayerActorInstance.gameObject.transform,);
    }
}