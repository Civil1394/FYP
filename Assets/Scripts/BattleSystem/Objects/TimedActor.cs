using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
public abstract class TimedActor : MonoBehaviour
{
    public Hourglass hourglass = null;
    public float MinThreshold = 0f;
    public float MaxThreshold = 5f;
    public float ActionCooldown = 2f;
    [SerializeField] private bool startTimerOnAwake = true;
    
    private float currentCooldown;
    protected bool isTimerActive = false;

    [SerializeField] private TMP_Text actorHourglassData;
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
    }
    protected virtual void Start()
    {

    }
    
    protected virtual void Update()
    {
        if (!isTimerActive) return;
        if(actorHourglassData != null) actorHourglassData.text = Helpers.GetShortFormTimeType(hourglass.TimeType) +
                                                                 " / " +currentCooldown.ToString("F2") + 
                                                                 " / " + ActionCooldown.ToString("F1") +
                                                                 " / " + MaxThreshold.ToString("F1");
        if(ActionCooldown <= MinThreshold) OverDrive();
        if(ActionCooldown >= MaxThreshold) Collapse();

        
        currentCooldown -= Time.deltaTime;
        OnTimerTick?.Invoke(GetNormalizedTime());
        if (currentCooldown <= 0)
        {
            isTimerActive = false;
            OnTimerComplete?.Invoke();
            CheckTimerStatus();
        }
    }
    
    public void CheckTimerStatus()
    {
        currentCooldown = ActionCooldown;
        OnTimerStart?.Invoke(currentCooldown);
        isTimerActive = true;
    }

    public void PauseTimer()
    {
        isTimerActive = false;
    }

    public void ResumeTimer()
    {
        isTimerActive = true;
    }

    protected float GetNormalizedTime()
    {
        float v = (currentCooldown / ActionCooldown);
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
    
    protected abstract void DeathCheck();
    protected abstract void OnDeath(); 
}