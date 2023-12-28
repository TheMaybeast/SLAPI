using Rage;
using System;
using System.Runtime.InteropServices;
using SLAPI.Memory;

namespace SLAPI.Utils;

public static class VehicleExtensions
{
    public static SLInstance GetSLInstance(this Vehicle veh)
    {
        if (SLInstance.SLInstances.TryGetValue(veh, out var instance))
            return instance;

        var temp = new SLInstance(veh);
        SLInstance.SLInstances.Add(veh, temp);
        return temp;
    }

    internal static IntPtr GetAudVehicleAudioEntityPtr(this Vehicle veh) => Marshal.ReadIntPtr(veh.MemoryAddress + GameOffsets.CVehicle_AudVehicleAudioEntity);
    internal static unsafe audSoundSet* GetSirenSoundSetPtr(this Vehicle veh) => (audSoundSet*)(veh.GetAudVehicleAudioEntityPtr() + GameOffsets.audVehicleAudioEntity_SirenSoundsOffset);
    internal static bool IsHornOn(this Vehicle veh) => GameFunctions.IsHornOn(veh.MemoryAddress);
}