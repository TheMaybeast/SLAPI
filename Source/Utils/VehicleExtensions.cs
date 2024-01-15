using Rage;
using System;
using System.Runtime.InteropServices;
using SLAPI.Memory;

namespace SLAPI.Utils;

public static class VehicleExtensions
{
    internal static IntPtr GetAudVehicleAudioEntityPtr(this Vehicle veh) => Marshal.ReadIntPtr(veh.MemoryAddress + GameOffsets.CVehicle_AudVehicleAudioEntity);
    internal static unsafe audSoundSet* GetSirenSoundSetPtr(this Vehicle veh) => (audSoundSet*)(veh.GetAudVehicleAudioEntityPtr() + GameOffsets.audVehicleAudioEntity_SirenSoundsOffset);
    public static SLVehicle GetSLVehicle(this Vehicle veh) => SLVehicle.Get(veh);
    
    internal static bool AssertSafe(this Vehicle veh)
    {
        if (!veh)
        {
            "Attempted to call function with invalid vehicle".ToLog(LogLevel.ERROR);
            return false;
        }

        if (veh.IsDead)
        {
            "Attempted to call function with dead vehicle".ToLog(LogLevel.ERROR);
            return false;
        }

        if (!veh.HasSiren)
        {
            "Attempted to call function with vehicle without a siren".ToLog(LogLevel.ERROR);
            return false;
        }

        return true;
    }
}