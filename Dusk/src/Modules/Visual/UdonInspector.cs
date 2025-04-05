using System;
using System.Collections.Generic;
using Dusk.Utilities;
using UnityEngine;
using VRC.Udon;
using Logger = Dusk.Core.Logger;
using Object = UnityEngine.Object;

namespace Dusk.Modules.Visual;

public class UdonInspector : BaseModule
{
    private Vector2 MenuPosition { get;} = new(300, 0);
    
    private Vector2 _scrollPos;
    private readonly Dictionary<UdonBehaviour, string> _udonCache = new();
    private KeyValuePair<UdonBehaviour, string> _selectedUdon;

    public UdonInspector() : base("UdonInspector", "Disassemble and analyze Udon Behaviours", ModuleType.Visual, KeyCode.F10) { }

    public override void OnEnable()
    {
        try
        {
            RefreshUdonCache();
        }
        catch (Exception ex)
        {
            Logger.Error($"[UdonInspector] Initialization failed: {ex}");
        }
    }

    public override void OnDisable()
    {
        _udonCache.Clear();
        _selectedUdon = default;
    }

    public override void OnRender()
    {
        if (!VRCUtility.IsInWorld()) return;
        
        GUILayout.BeginArea(new Rect(MenuPosition.x, MenuPosition.y, 250, Screen.height - 40), GUI.skin.window);
        {
            DrawHeader();
            DrawControls();
            
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            {
                DrawUdonList();
            }
            GUILayout.EndScrollView();
            
            DrawDisassemblyControls();
        }
        GUILayout.EndArea();
    }
    
    private void DrawHeader()
    {
        GUILayout.Label("<b>Udon Inspector</b>", new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 14
        });

        GUILayout.Space(10);
    }

    private void DrawControls()
    {
        if (GUILayout.Button("Refresh Udon Cache"))
        {
            RefreshUdonCache();
        }
    }

    private void DrawUdonList()
    {
        try
        {
            GUILayout.Label($"Found UdonBehaviours ({_udonCache.Count})");
        
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            {
                foreach (var kv in _udonCache)
                {
                    var udonBehaviour = kv.Key;
                    string ubName = kv.Value;
                    var selectedUdonBehaviour = _selectedUdon.Key;
                
                    bool isSelected = selectedUdonBehaviour == udonBehaviour;
                    var btnStyle = isSelected ? GUI.skin.button : GUI.skin.box;

                    if (GUILayout.Button(ubName, btnStyle))
                    {
                        _selectedUdon = new KeyValuePair<UdonBehaviour, string>(udonBehaviour, ubName);
                    }
                }
            }
            GUILayout.EndVertical();
        }
        catch (Exception ex)
        {
            Logger.Error($"[UdonInspector] Udon list rendering failed: {ex}");
        }
    }
    
    private void DrawDisassemblyControls()
    {
        if (_selectedUdon.Key == null) return;
        
        GUILayout.Space(15);
        if (GUILayout.Button($"Disassemble {_selectedUdon.Key}"))
        {
            UdonDisassembler.Disassemble(_selectedUdon.Key, _selectedUdon.Value);
        }
    }
    
    private void RefreshUdonCache()
    {
        if (!VRCUtility.IsInWorld()) return;
        
        _udonCache.Clear();
        var allObjs = Object.FindObjectsOfType<GameObject>();
        foreach (var go in allObjs)
        {
            if (go.TryGetComponent<UdonBehaviour>(out var ub))
            {
                _udonCache.Add(ub, go.name);
            }
        }
        Logger.Debug($"[UdonInspector] {_udonCache.Count} UdonBehaviours found");
    }
}