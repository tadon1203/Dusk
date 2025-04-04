using System.Collections.Generic;
using Dusk.Core;
using Dusk.Core.Config;
using UnityEngine;
using Logger = Dusk.Core.Logger;

namespace Dusk.Modules;

public static class ModuleManager
{
    private static readonly List<BaseModule> Modules = new();
    
    public static IEnumerable<BaseModule> GetModules() => Modules.AsReadOnly();
    
    public static void Initialize()
    {
    }
    
    private static void RegisterModule(BaseModule module)
    {
        if (module == null)
        {
            Logger.Warning("Attempted to register a null module");
            return;
        }
        
        ConfigManager.RegisterModuleConfig(module);
        
        Modules.Add(module);
        Logger.Debug($"Module registered: {module.Name}");
    }
    
    public static void UpdateModules()
    {
        foreach (var module in Modules)
        {
            if (module.Enabled)
            {
                module.OnUpdate();
            }
            if (Input.GetKeyDown(module.ToggleKey))
            {
                module.Toggle();
            }
        }
    }

    public static void RenderModules()
    {
        foreach (var module in Modules)
        {
            if (module.Enabled)
            {
                module.OnRender();
            }
        }
    }
}