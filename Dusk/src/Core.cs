using System;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using Dusk.Modules;
using UnityEngine;

namespace Dusk;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class DuskPlugin : BasePlugin
{
    public new static ManualLogSource Log;

    public override void Load()
    {
        Log = base.Log;
        ModuleManager.Initialize();
        AddComponent<DuskMonoBehaviour>();
        Log.LogInfo("Dusk initialized");
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