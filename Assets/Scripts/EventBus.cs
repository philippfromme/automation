using System;
using System.Collections.Generic;

public static class EventBus
{
    private static Dictionary<Type, Delegate> _assignedActions = new();

    public static void Subscribe<T>(Action<T> action)
    {
        if (_assignedActions.TryGetValue(typeof(T), out var existingAction))
        {
            _assignedActions[typeof(T)] = Delegate.Combine(existingAction, action);
        }
        else
        {
            _assignedActions[typeof(T)] = action;
        }
    }

    public static void Unsubscribe<T>(Action<T> action)
    {
        if (_assignedActions.TryGetValue(typeof(T), out var existingAction))
        {
            Delegate newAction = Delegate.Remove(existingAction, action);

            if (newAction == null)
            {
                _assignedActions.Remove(typeof(T));
            }
            else
            {
                _assignedActions[typeof(T)] = newAction;
            }
        }
    }

    public static void Publish<T>(T eventData)
    {
        if (_assignedActions.TryGetValue(typeof(T), out Delegate action))
        {
            ((Action<T>)action)?.Invoke(eventData);
        }
    }
}