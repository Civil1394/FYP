using System;
using System.Collections.Generic;

public interface IStatusEffectHandler
{
    float ProcessDamageEffect(IDamagable damagable);
    void ApplyControlEffect(TimedActor actor);
    void ApplyStatEffect(TimedActor actor);
    void ApplyUtilityEffect();
    
    void RemoveControlEffect(TimedActor actor);
    void RemoveStatEffect(TimedActor actor);
    void RemoveUtilityEffect();
}
