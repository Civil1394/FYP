using UnityEngine;
using System;
using System.Collections.Generic;

// Base interface for all action handlers
public interface IActionHandler
{
    void Execute(object parameter = null);
}

// Handler for parameterless actions
public class VoidActionHandler : IActionHandler
{
    public Action action;

    public VoidActionHandler(Action action)
    {
        this.action = action;
    }

    public void Execute(object parameter = null)
    {
        action.Invoke();
    }
}

// Handler for parameterized actions
public class ActionHandler<T> : IActionHandler
{
    public Action<T> action;

    public ActionHandler(Action<T> action)
    {
        this.action = action;
    }

    public void Execute(object parameter = null)
    {
        if (parameter is T typedParameter)
        {
            action.Invoke(typedParameter);
        }
    }
}

// Main GenericAction class that can handle both void and parameterized actions
public class GenericAction
{
    private Dictionary<Type, List<IActionHandler>> handlers = new Dictionary<Type, List<IActionHandler>>();
    private List<IActionHandler> voidHandlers = new List<IActionHandler>();

    // Add listener for parameterized actions
    public void AddListener<T>(Action<T> listener)
    {
        Type type = typeof(T);
        
        if (!handlers.ContainsKey(type))
        {
            handlers[type] = new List<IActionHandler>();
        }
        
        handlers[type].Add(new ActionHandler<T>(listener));
    }

    // Add listener for void actions
    public void AddListener(Action listener)
    {
        voidHandlers.Add(new VoidActionHandler(listener));
    }

    // Remove listener for parameterized actions
    public void RemoveListener<T>(Action<T> listener)
    {
        Type type = typeof(T);
        
        if (handlers.ContainsKey(type))
        {
            var handlersOfType = handlers[type];
            handlersOfType.RemoveAll(h => 
                (h as ActionHandler<T>)?.action == listener);
            
            if (handlersOfType.Count == 0)
            {
                handlers.Remove(type);
            }
        }
    }

    // Remove listener for void actions
    public void RemoveListener(Action listener)
    {
        voidHandlers.RemoveAll(h => 
            (h as VoidActionHandler)?.action == listener);
    }

    // Invoke parameterized actions
    public void Invoke<T>(T parameter)
    {
        Type type = typeof(T);
        
        if (handlers.ContainsKey(type))
        {
            foreach (var handler in handlers[type])
            {
                handler.Execute(parameter);
            }
        }
    }

    // Invoke void actions
    public void Invoke()
    {
        foreach (var handler in voidHandlers)
        {
            handler.Execute();
        }
    }

    public void RemoveAllListener()
    {
        handlers.Clear();
        voidHandlers.Clear();
    }
}

