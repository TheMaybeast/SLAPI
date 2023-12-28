using SLAPI.Memory;
using SLAPI.Utils;

namespace SLAPI;

public static class Main
{
    internal static bool HasInitialized;

    public static bool Initialize()
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

    public static void Terminate() => Manager.Cleanup();
}