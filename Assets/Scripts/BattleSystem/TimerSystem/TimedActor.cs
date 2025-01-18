using UnityEngine;
using System;

public class TimedActor : MonoBehaviour
{
    public float actionCooldown = 2f;
    [SerializeField] private bool startTimerOnAwake = true;
    
    private float currentCooldown;
    private bool isTimerActive = false;
    
    public event Action<float> OnTimerStart;
    public event Action OnTimerComplete;
    public event Action<float> OnTimerTick; // Provides normalized time (0 to 1)
    
    protected virtual void Start()
    {
        if (startTimerOnAwake)
        {
            StartNewTimer();
        }
    }

    // This method updates the timer each frame if it is active.
    protected virtual void Update()
    {
        if (!isTimerActive) return;
        
        currentCooldown -= Time.deltaTime;
        OnTimerTick?.Invoke(GetNormalizedTime());
        
        if (currentCooldown <= 0)
        {
            isTimerActive = false;
            OnTimerComplete?.Invoke();
            ExecuteAction();
        }
    }


    public void StartNewTimer()
    {
        currentCooldown = actionCooldown;
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
        return 1 - (currentCooldown / actionCooldown);
    }

    protected virtual void ExecuteAction()
    {
        // Override this in derived classes
        //Debug.Log("Executing action");
        StartNewTimer(); // Restart timer after action
    }

    protected virtual void TimeManipulate(TimeManipulationType type,float flowTime)
    {
        switch (type)   
        {
               case TimeManipulationType.Boost:
                   actionCooldown -= flowTime;
                   break;
               case TimeManipulationType.Slow:
                   actionCooldown += flowTime;
                   break;
               case TimeManipulationType.None:
                   break;
        }
    }
    
}