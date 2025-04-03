﻿using System;
using Dusk.Events;
using Dusk.Modules;

namespace Dusk.Extensions;

public static class ModuleEventExtensions
{
    public static void SubscribeEvent<T>(this BaseModule module, Action<T> handler)
    {
        EventManager.Subscribe(handler);
    }
    
    public static void PublishEvent<T>(this BaseModule module, T eventData)
    {
        EventManager.Publish(eventData);
    }
}