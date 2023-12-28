using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Rage;
using SLAPI.Memory;
using SLAPI.Utils;

namespace SLAPI;

public enum SirenState
{
    OFF = 0,
    SLOW = 1,
    FAST = 2,
    WARNING = 3
}

public unsafe class SLInstance
{
    internal static Dictionary<IntPtr, SLInstance> SLInstances = new();

    private readonly Vehicle _vehicle;
    private readonly IntPtr _vehiclePtr;

    private readonly IntPtr _audVehicleAudioEntityPtr;
    public readonly audSoundSet* audSoundSet;

    private readonly uint _defaultAudSoundSetNameHash;

    public SLInstance(Vehicle veh)
    {
        if (!Main.Init()) throw new Exception("FATAL: Attempting to create SLInstance with incompatible game version.");

        // Saves vehicle and relevant memory addresses
        _vehicle = veh;
        _vehiclePtr = veh.MemoryAddress;
        _audVehicleAudioEntityPtr = veh.GetAudVehicleAudioEntityPtr();
        audSoundSet = veh.GetSirenSoundSetPtr();

#if DEBUG
        $"Initiating SLInstance for vehicle {_vehicle.MemoryAddress}".ToLog();
        $"  AudVehicleEntity: {_audVehicleAudioEntityPtr}".ToLog();
        $"  SirensAudSoundSet: {(uint)audSoundSet}".ToLog();
#endif

        // Saves default audSoundSet NameHash
        _defaultAudSoundSetNameHash = audSoundSet->NameHash;
    }

    public SirenState SirenState
    {
        get => (SirenState)Marshal.ReadInt32(_audVehicleAudioEntityPtr + GameOffsets.audVehicleAudioEntity_SirenStateOffset);
        set => Marshal.WriteInt32(_audVehicleAudioEntityPtr + GameOffsets.audVehicleAudioEntity_SirenStateOffset, (int)value);
    }
    public uint SirenTime
    {
        get => (uint)Marshal.ReadInt32(_audVehicleAudioEntityPtr, GameOffsets.audVehicleAudioEntity_SirenTimeOffset);
        set => Marshal.WriteInt32(_audVehicleAudioEntityPtr, GameOffsets.audVehicleAudioEntity_SirenTimeOffset, (int)value);
    }
    public uint SirenLastChangeTime => (uint)Marshal.ReadInt32(_audVehicleAudioEntityPtr + GameOffsets.audVehicleAudioEntity_LastSirenChangeTimeOffset);
    public uint SequentialSirenPresses => (uint)Marshal.ReadInt32(_vehicle.MemoryAddress + GameOffsets.CVehicle_SequentialSirenPressesOffset);
    public bool IsHornOn => _vehicle.IsHornOn();

    public void Reset()
    {
#if DEBUG
        $"Attempting to reset vehicle {_vehiclePtr}".ToLog();
#endif
        if (!_vehicle)
        {
            $"  Vehicle {_vehiclePtr} does not exist, removing from pool.".ToLog();
            SLInstances.Remove(_vehiclePtr);
        }
        audSoundSet->Init(_defaultAudSoundSetNameHash);
#if DEBUG
        $"Reset vehicle {_vehiclePtr}".ToLog();
#endif
    }
    public static void ResetAll() => SLInstances.Values.ToList().ForEach(x => x.Reset());
}