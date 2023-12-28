using Rage;
using System;
using System.Runtime.InteropServices;
using SLAPI.Memory;

namespace SLAPI.Utils;

public static class VehicleExtensions
{
    internal static IntPtr GetAudVehicleAudioEntityPtr(this Vehicle veh) => Marshal.ReadIntPtr(veh.MemoryAddress + GameOffsets.CVehicle_AudVehicleAudioEntity);
    internal static unsafe audSoundSet* GetSirenSoundSetPtr(this Vehicle veh) => (audSoundSet*)(veh.GetAudVehicleAudioEntityPtr() + GameOffsets.audVehicleAudioEntity_SirenSoundsOffset);
}