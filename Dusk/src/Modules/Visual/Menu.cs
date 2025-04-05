using Dusk.Core.Config;
using UnityEngine;

namespace Dusk.Modules.Visual;

public class Menu : BaseModule
{
    private Vector2 MenuPosition { get;} = new(0, 0);
    
    private Vector2 _scrollPosition;
    
    public Menu() : base("Menu", "Provides configuration menu for all modules", ModuleType.Visual, KeyCode.Insert) { }

    public override void OnRender()
    {
        GUILayout.BeginArea(new Rect(MenuPosition.x, MenuPosition.y, 300.0f, Screen.height - 40), GUI.skin.window);

        DrawMenuContent();

        GUILayout.EndArea();
    }

    private void DrawMenuContent()
    {
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
        {
            DrawHeader();
            DrawModules();
            DrawConfigSection();
        }
        GUILayout.EndScrollView();
    }

    private void DrawHeader()
    {
        GUILayout.Label("<b>Dusk</b>", new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 14
        });
        GUILayout.Space(10);
    }

    private void DrawModules()
    {
        foreach (var module in ModuleManager.GetModules())
        {
            DrawModuleControls(module);
        }

        GUILayout.Space(20);
    }

    private void DrawModuleControls(BaseModule module)
    {
        GUILayout.BeginVertical("box");
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(module.Name);
                if (GUILayout.Button(module.Enabled ? "OFF" : "ON", GUILayout.Width(120)))
                {
                    module.Toggle();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Label(module.Description);
        }
        GUILayout.EndVertical();
    }

    private void DrawConfigSection()
    {
        if (GUILayout.Button("Save Config"))
        {
            ConfigManager.SaveConfig();
        }

        if (GUILayout.Button("Load Config"))
        {
            ConfigManager.LoadConfig();
        }
    }
}
