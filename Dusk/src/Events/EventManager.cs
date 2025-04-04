using System;
using System.Collections.Generic;
using Dusk.Core;

namespace Dusk.Events;

public static class EventManager
{
    private static readonly Dictionary<Type, Delegate> _eventTable = new();

    public static void Subscribe<T>(Action<T> handler)
    {
        if (handler == null)
        {
            Logger.Critical(nameof(handler) + " is null");
            return;
        }
        
        if (_eventTable.TryGetValue(typeof(T), out var existing))
        {
            _eventTable[typeof(T)] = Delegate.Combine(existing, handler);
        }
        else
        {
            _eventTable[typeof(T)] = handler;
        }
    }
    
    public static void Unsubscribe<T>(Action<T> handler)
    {
        if (_eventTable.TryGetValue(typeof(T), out var existing))
        {
            _eventTable[typeof(T)] = Delegate.Remove(existing, handler);
        }
    }
    
    public static void Publish<T>(T eventData)
    {
        if (_eventTable.TryGetValue(typeof(T), out var action))
        {
            (action as Action<T>)?.Invoke(eventData);
        }
    }
}