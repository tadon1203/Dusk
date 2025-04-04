using System;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using Dusk.Core.Config;
using Dusk.Modules;
using UnityEngine;

namespace Dusk.Core;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class DuskPlugin : BasePlugin
{
    public override void Load()
    {
        Logger.Initialize();
        Logger.Info("Initializing Dusk...");

        ConfigManager.Initialize(); 
        ModuleManager.Initialize();
        
        CoroutineRunner.Initialize(AddComponent<CoroutineRunner>());
        AddComponent<DuskMonoBehaviour>();
    
        Logger.Info("Dusk initialized successfully");
    }
}

public class DuskMonoBehaviour : MonoBehaviour
{
    public DuskMonoBehaviour(IntPtr handle) : base(handle) {}

    private void Update()
    {
        ModuleManager.UpdateModules();
    }

    private void OnGUI()
    {
        ModuleManager.RenderModules();
    }
}