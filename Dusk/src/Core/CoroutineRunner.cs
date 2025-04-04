using System.Collections;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using UnityEngine;

namespace Dusk.Core;

public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner _instance;
    
    public static bool IsInitialized => _instance != null;

    public static void Initialize(CoroutineRunner instance)
    {
        if (instance == null)
            Logger.Critical(nameof(instance) + " is null");
        
        _instance = instance;
    }

    public static Coroutine StartCoroutine(IEnumerator routine)
    {
        if (routine == null)
            Logger.Critical(nameof(routine) + " is null");
        
        return _instance?.StartCoroutine(routine.WrapToIl2Cpp());
    }
}