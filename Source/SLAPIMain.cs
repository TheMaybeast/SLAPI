using SLAPI.Memory;
using SLAPI.Utils;

namespace SLAPI;

public class SLAPIMain
{
    private readonly bool _hasInitialized;

    public SLAPIMain()
    {
        // Avoids initializing multiple times
        if (_hasInitialized) return;

        Log.Init();

        // Processes game memory and returns false if error
        var memorySuccess = GameFunctions.Init() && GameOffsets.Init();
        if (!memorySuccess) return;

        _hasInitialized = true;
    }

    ~SLAPIMain()
    {
        Log.Terminate();
    }
}