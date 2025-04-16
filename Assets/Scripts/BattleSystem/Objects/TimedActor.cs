using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
using RainbowArt.CleanFlatUI;
public abstract class TimedActor : MonoBehaviour
{
    public Hourglass hourglass = null;
    public float MinThreshold = 0f;
    public float MaxThreshold = 5f;
    public float BaseActionCooldown { get; private set; }
    public float ActionCooldown 
    {
        get { return BaseActionCooldown * ActionCooldownMultiplier; }
        set { BaseActionCooldown = value; }
    }
    public float CurrentCooldown;
    public bool IsTimerActive = false;
    public float ActionCooldownMultiplier = 1.0f;
    
    [SerializeField] private bool startTimerOnAwake = true;
    [SerializeField] private TMP_Text actorHourglassData;
    [SerializeField] private ProgressBarAuto progressBar;
    protected ObjectStatusEffectManager statusManager;
    
    #region Events

    public event Action<float> OnTimerStart;
    public event Action OnTimerComplete;
    public event Action<float> OnTimerTick; // Provides normalized time (0 to 1)
    public event Action OnOverDrive;
    public event Action OnCollapse;
    #endregion

   public virtual void Init(Hourglass hourglass)
    {
        this.hourglass = hourglass;
        MinThreshold = hourglass.MinThreshold;
        MaxThreshold = hourglass.MaxThreshold;
        ActionCooldown = this.hourglass.Sand;
        hourglass.IsOccupied = true;
        CheckTimerStatus();
        if(progressBar != null) 
            progressBar.InitLoadSpeed(hourglass.Sand);
    }
    protected virtual void Start()
    {
        statusManager = GetComponent<ObjectStatusEffectManager>();
        if (statusManager == null)
        {
            statusManager = gameObject.AddComponent<ObjectStatusEffectManager>();
        }
        //
        // statusManager.OnStatusEffectApplied += HandleStatusEffectApplied;
        // statusManager.OnStatusEffectRemoved += HandleStatusEffectRemoved;
    }

    private void OnDestroy()
    {
        // if (statusManager != null)
        // {
        //     statusManager.OnStatusEffectApplied -= HandleStatusEffectApplied;
        //     statusManager.OnStatusEffectRemoved -= HandleStatusEffectRemoved;
        // }
    }

    protected virtual void Update()
    {
        if (!IsTimerActive) return;
        if(actorHourglassData != null) actorHourglassData.text = Helpers.GetShortFormTimeType(hourglass.TimeType) +
                                                                 " / " +CurrentCooldown.ToString("F2") + 
                                                                 " / " + ActionCooldown.ToString("F1") +
                                                                 " / " + MaxThreshold.ToString("F1");
        if(ActionCooldown <= MinThreshold) OverDrive();
        if(ActionCooldown >= MaxThreshold) Collapse();

        
        CurrentCooldown -= Time.deltaTime;
        OnTimerTick?.Invoke(GetNormalizedTime());
        if (CurrentCooldown <= 0)
        {
            IsTimerActive = false;
            OnTimerComplete?.Invoke();
            CheckTimerStatus();
        }
    }
    
    public void CheckTimerStatus()
    {
        CurrentCooldown = ActionCooldown;
        OnTimerStart?.Invoke(CurrentCooldown);
        IsTimerActive = true;
    }
    #region TimerUtilities
    public void AdjustCurrentCooldown(float multiplier)
    {
        if (IsTimerActive)
        {
            CurrentCooldown *= multiplier;
        }
    }

    public void PauseTimer()
    {
        IsTimerActive = false;
    }

    public void ResumeTimer()
    {
        IsTimerActive = true;
    }
    
    protected float GetNormalizedTime()
    {
        float v = (CurrentCooldown / ActionCooldown);
        return v;
    }
    

    protected virtual void TimeManipulate(float flowTime)
    {
        ActionCooldown -= flowTime;
    }

    protected virtual void OverDrive()
    {
        Debug.Log("Overdrive");
        
    }

    protected virtual void Collapse()
    {
        Debug.Log("Collapse");
    }
    #endregion
    
    
    protected abstract void DeathCheck();
    protected abstract void OnDeath(); 
}