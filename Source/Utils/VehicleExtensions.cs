using Rage;
using System;
using System.Runtime.InteropServices;
using SLAPI.Memory;

namespace SLAPI.Utils;

internal static class VehicleExtensions
{
    public static IntPtr GetAudVehicleAudioEntityPtr(this Vehicle veh) => Marshal.ReadIntPtr(veh.MemoryAddress + GameOffsets.CVehicle_AudVehicleAudioEntity);
    public static unsafe audSoundSet* GetSirenSoundSetPtr(this Vehicle veh) => (audSoundSet*)(veh.GetAudVehicleAudioEntityPtr() + GameOffsets.audVehicleAudioEntity_SirenSoundsOffset);
}