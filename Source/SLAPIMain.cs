using SLAPI.Memory;
using SLAPI.Utils;

namespace SLAPI;

public class SLAPIMain
{
    internal bool HasInitialized;

    public SLAPIMain()
    {
        // Avoids initializing multiple times
        if (HasInitialized) return;

        Log.Init();

        // Processes game memory and returns false if error
        var memorySuccess = GameFunctions.Init() && GameOffsets.Init();
        if (!memorySuccess) return;

        HasInitialized = true;
    }

    ~SLAPIMain()
    {
        Log.Terminate();
    }
}