using Dusk.Core;
using VRC.SDKBase;

namespace Dusk.Utilities;

public static class VRCUtility
{
    public static VRCPlayerApi GetLocalPlayer()
    {
        if (Networking.LocalPlayer == null)
        {
            Logger.Critical("Attempted to get local player, but it is not in world");
            return null;
        }
        
        return Networking.LocalPlayer;
    }

    public static bool IsInWorld()
    {
        return GetLocalPlayer() != null;
    }
}