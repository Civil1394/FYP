using System;
using System.Collections.Generic;
using UnityEngine;

public static class StatusEffectHandlerFactory
{
    // Registry to map StatusEffectId to the corresponding Type
    private static readonly Dictionary<StatusEffectId, Type> effectHandlerRegistry = new Dictionary<StatusEffectId, Type>();

    // Static constructor to initialize the registry
    static StatusEffectHandlerFactory()
    {
        // Register default handlers
        RegisterHandler(StatusEffectId.Poison, typeof(PoisonInstance));
        RegisterHandler(StatusEffectId.Frost, typeof(FrostInstance));
        // Add other handlers as needed
    }

    // Method to register a new handler type
    public static void RegisterHandler(StatusEffectId id, Type handlerType)
    {
        if (!typeof(StatusEffectInstance).IsAssignableFrom(handlerType))
        {
            Debug.LogError($"Type {handlerType.Name} is not a subclass of StatusEffectInstance");
            return;
        }

        effectHandlerRegistry[id] = handlerType;
    }


    public static StatusEffectInstance CreateHandler(
        StatusEffectData data,
        int initialStacks = 1)
    {
        // Get the type from the registry or use default
        Type handlerType = effectHandlerRegistry.TryGetValue(data.id, out Type registeredType) 
            ? registeredType 
            : typeof(StatusEffectInstance);

        // Create the instance
        StatusEffectInstance instance = (StatusEffectInstance)Activator.CreateInstance(handlerType);

        // Initialize basic properties
        instance.Data = data;
        instance.RemainingDuration = data.duration;
        instance.CurrentStacks = Mathf.Min(initialStacks, data.maxStacks);
        instance.TimeSinceLastTick = 0f;
        instance.IsActive = true;

        return instance;
    }
}
