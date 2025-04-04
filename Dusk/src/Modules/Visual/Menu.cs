using Dusk.Core.Config;
using UnityEngine;

namespace Dusk.Modules.Visual;

public class Menu : BaseModule
{
    public Vector2 MenuPosition { get; set; } = new Vector2(20, 20);
    public float MenuWidth { get; } = 200f;
    
    private Rect _menuRect;
    private Vector2 _scrollPosition;
    
    public Menu() : base("Menu", "Provides configuration menu for all modules", ModuleType.Visual, KeyCode.Insert) { }

    public override void OnRender()
    {
        _menuRect = new Rect(MenuPosition.x, MenuPosition.y, MenuWidth, Screen.height - 40);
        _menuRect = GUI.Window(0, _menuRect, (GUI.WindowFunction)DrawMenuWindow, "Dusk Menu");
    }

    private void DrawMenuWindow(int windowID)
    {
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

        GUILayout.Label("Modules");
        GUILayout.Space(10);

        foreach (var module in ModuleManager.GetModules())
        {
            DrawModuleControls(module);
        }

        GUILayout.Space(20);
        DrawConfigSection();

        GUILayout.EndScrollView();
        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }

    private void DrawModuleControls(BaseModule module)
    {
        GUILayout.BeginVertical("box");
        
        GUILayout.BeginHorizontal();
        GUILayout.Label(module.Name);
        if (GUILayout.Button(module.Enabled ? "OFF" : "ON", GUILayout.Width(40)))
        {
            module.Toggle();
        }
        GUILayout.EndHorizontal();

        GUILayout.Label(module.Description);
        
        GUILayout.EndVertical();
        GUILayout.Space(5);
    }

    private void DrawConfigSection()
    {
        GUILayout.Label("Settings");
        
        if (GUILayout.Button("Save Config"))
        {
            ConfigManager.SaveConfig();
        }
    }
}