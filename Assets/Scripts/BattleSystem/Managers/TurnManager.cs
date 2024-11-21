using System;
using System.Collections;
using UnityEngine;

public enum TurnActionType
{
    Move,
    DrawCard,
    PlayCard,
    
}

public class TurnAction
{
    public TurnActionType ActionType { get; private set; }
    public bool IsExecuted { get; private set; }
    public string Description { get; private set; }

    public TurnAction(TurnActionType type, string description = "")
    {
        ActionType = type;
        Description = description;
        IsExecuted = false;
    }

    public void MarkAsExecuted()
    {
        IsExecuted = true;
    }
}

public class TurnManager : SingletonBase<TurnManager>
{
    private TurnAction currentTurnAction;
    public bool IsActionExecutedThisTurn => currentTurnAction?.IsExecuted ?? false;
    
    public event Action OnTurnStart;
    public event Action OnTurnEnd;
    public event Action<TurnAction> OnActionExecuted;
    public void StartNewTurn()
    {
        currentTurnAction = null;
        OnTurnStart?.Invoke();
    }
    public bool CanExecuteAction(TurnActionType actionType)
    {
        return !IsActionExecutedThisTurn;
    }

    public void ExecuteAction(TurnActionType actionType, string description = "")
    {
        if (!CanExecuteAction(actionType))
        {
            Debug.LogWarning($"Cannot execute {actionType} action - Action already executed this turn");
            return;
        }

        currentTurnAction = new TurnAction(actionType, description);
        currentTurnAction.MarkAsExecuted();
        OnActionExecuted?.Invoke(currentTurnAction);
    }

    public void EndTurn()
    {
        OnTurnEnd?.Invoke();
        
    }
}