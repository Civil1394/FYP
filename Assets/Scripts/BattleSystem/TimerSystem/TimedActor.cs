using UnityEngine;
using System;
using UnityEditor.UI;
using TMPro;
public class TimedActor : MonoBehaviour
{
    public float MinThreshold = 0f;
    public float MaxThreshold = 5f;
    public float ActionCooldown = 2f;
    [SerializeField] private bool startTimerOnAwake = true;
    
    private float currentCooldown;
    private bool isTimerActive = false;

    [SerializeField] private TMP_Text actionCoolDownText;
    #region Events

    public event Action<float> OnTimerStart;
    public event Action OnTimerComplete;
    public event Action<float> OnTimerTick; // Provides normalized time (0 to 1)
    public event Action OnOverDrive;
    public event Action OnCollapse;
    #endregion
    
    protected virtual void Start()
    {
        if (startTimerOnAwake)
        {
            CheckTimerStatus();
        }
    }
    
    protected virtual void Update()
    {
        if (!isTimerActive) return;
        if(actionCoolDownText != null) actionCoolDownText.text = currentCooldown.ToString("F2") + " / " + ActionCooldown.ToString("F1");
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

    public float GetNormalizedTime()
    {
        return 1 - (currentCooldown / ActionCooldown);
    }
    

    protected virtual void TimeManipulate(TimeManipulationType type,float flowTime)
    {
        switch (type)   
        {
               case TimeManipulationType.Boost:
                   ActionCooldown -= flowTime;
                   break;
               case TimeManipulationType.Slow:
                   ActionCooldown += flowTime;
                   break;
               case TimeManipulationType.None:
                   break;
        }
    }

    protected virtual void OverDrive()
    {
        Debug.Log("Overdrive");
        
    }

    protected virtual void Collapse()
    {
        Debug.Log("Collapse");
    }
}