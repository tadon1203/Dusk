using System.IO;

namespace Dusk.Utilities;

public static class FileUtility
{
    public static void EnsureDirectoryExists(string dir)
    {
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }
}