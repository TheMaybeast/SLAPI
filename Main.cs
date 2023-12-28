using SLAPI.Memory;
using SLAPI.Utils;

namespace SLAPI;

internal static class Main
{
    public static bool HasInitialized;

    public static bool Init()
    {
        // Avoids initializing multiple times
        if (HasInitialized) return true;

        // Creates logging
        _ = new Log();

        // Processes game memory and returns false if error
        var memorySuccess = GameFunctions.Init() && GameOffsets.Init();
        if (!memorySuccess) return false;

        HasInitialized = true;
        return true;
    }
}