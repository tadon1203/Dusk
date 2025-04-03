using System.Collections.Generic;
using Dusk.Modules.Visual;

namespace Dusk.Modules;

public static class ModuleManager
{
    private static readonly List<BaseModule> Modules = new();
    private static MenuModule _menuModule;
    
    public static IEnumerable<BaseModule> GetModules() => Modules.AsReadOnly();
    
    public static void Initialize()
    {
        _menuModule = new MenuModule();
        RegisterModule(_menuModule);
    }
    
    private static void RegisterModule(BaseModule module)
    {
        if (module == null) return;
        Modules.Add(module);
        DuskPlugin.Log.LogInfo($"Module registered: {module.Name}");
    }
    
    public static void UpdateModules()
    {
        foreach (var module in Modules)
        {
            if (module.Enabled)
            {
                module.OnUpdate();
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