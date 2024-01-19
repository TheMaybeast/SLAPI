using System.Reflection;
using SLAPI.Memory;
using SLAPI.Memory.Patches;
using SLAPI.Utils;

namespace SLAPI;

public class SLAPIMain
{
    private readonly bool _hasInitialized;

    public SLAPIMain()
    {
        // Avoids initializing multiple times
        if (_hasInitialized) return;

        $"====== Loading SLAPI v{Assembly.GetExecutingAssembly().GetName().Version} ======".ToLog();

        // Processes game memory and returns false if error
        var memorySuccess = GameFunctions.Init() && GameOffsets.Init();
        if (!memorySuccess) return;

        _hasInitialized = true;
    }
    
    public bool SirenBlipPatch
    {
        get => BlipPatch.Patched;
        set
        {
            if (value && !BlipPatch.Patched)
                BlipPatch.Patch();
            else if (BlipPatch.Patched)
                BlipPatch.Remove();
        } 
    }

    ~SLAPIMain()
    {
        // Resets all managed vehicles to default siren sounds
        foreach (var slVehicle in SLVehicle.SLVehicles.Values)
            slVehicle.Sounds.SirenSounds = slVehicle.Sounds.DefaultSirenSounds;
        
        // Deallocates all created soundset structs
        unsafe
        {
            foreach (var soundSet in SoundSet.SoundSets)
                if(!soundSet.Vanilla) Manager.Destroy(soundSet.Sounds.SoundSetStruct);    
        }

        // Removes blip patch
        if (BlipPatch.Patched)
            BlipPatch.Remove();
        
        $"====== Unloaded SLAPI v{Assembly.GetExecutingAssembly().GetName().Version} ======".ToLog();
    }
}