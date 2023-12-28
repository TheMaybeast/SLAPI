using Rage;
using SLAPI.Memory;
using System.Runtime.InteropServices;
using SLAPI.Utils;
using System;

namespace SLAPI;

public static class Functions
{
    public static eSirenState GetSirenState(Vehicle vehicle)
    {
        if (!AssertSafe(vehicle)) return 0;

        return (eSirenState)Marshal.ReadInt32(vehicle.GetAudVehicleAudioEntityPtr() +
                                              GameOffsets.audVehicleAudioEntity_SirenStateOffset);
    }

    public static void SetSirenState(Vehicle vehicle, eSirenState state)
    {
        if (!AssertSafe(vehicle)) return;

        Marshal.WriteInt32(
            vehicle.GetAudVehicleAudioEntityPtr() + GameOffsets.audVehicleAudioEntity_SirenStateOffset, (int)state);
    }

    public static uint GetSirenTime(Vehicle vehicle)
    {
        if (!AssertSafe(vehicle)) return 0;

        return (uint)Marshal.ReadInt32(vehicle.GetAudVehicleAudioEntityPtr(),
            GameOffsets.audVehicleAudioEntity_SirenTimeOffset);
    }

    public static void SetSirenTime(Vehicle vehicle, uint sirenTime)
    {
        if (!AssertSafe(vehicle)) return;

        Marshal.WriteInt32(vehicle.GetAudVehicleAudioEntityPtr(), GameOffsets.audVehicleAudioEntity_SirenTimeOffset,
            (int)sirenTime);
    }

    public static uint GetSirenLastChangeTime(Vehicle vehicle)
    {
        if (!AssertSafe(vehicle)) return 0;

        return (uint)Marshal.ReadInt32(vehicle.GetAudVehicleAudioEntityPtr() +
                                       GameOffsets.audVehicleAudioEntity_LastSirenChangeTimeOffset);
    }

    public static uint GetSirenSequentialPresses(Vehicle vehicle)
    {
        if (!AssertSafe(vehicle)) return 0;

        return (uint)Marshal.ReadInt32(vehicle.MemoryAddress + GameOffsets.CVehicle_SequentialSirenPressesOffset);
    }

    public static bool GetHornStatus(Vehicle vehicle) => GameFunctions.IsHornOn(vehicle.MemoryAddress);

    public static unsafe SoundSetWrapper GetSoundSet(uint soundSetNameHash)
    {
        var audSoundSetPtr = Manager.Allocate<audSoundSet>();
        audSoundSetPtr->Init(soundSetNameHash);

        var soundSetPtr = Marshal.ReadIntPtr((IntPtr)audSoundSetPtr);
        Manager.Destroy(audSoundSetPtr);

        return new SoundSetWrapper
        {
            SoundSet = (SoundSet*)soundSetPtr,
            NameHash = soundSetNameHash
        };
    }
    public static SoundSetWrapper GetSoundSet(string soundSetName) => GetSoundSet(Game.GetHashKey(soundSetName));

    public static unsafe void SetVehicleSoundSet(Vehicle vehicle, SoundSetWrapper soundSet)
    {
        if (!AssertSafe(vehicle)) return;

        var ptr = vehicle.GetSirenSoundSetPtr();
        ptr->Data = soundSet.SoundSet;
        ptr->NameHash = soundSet.NameHash;
    }
    public static void SetVehicleSoundSet(Vehicle vehicle, uint soundSetNameHash)
    {
        var soundSet = GetSoundSet(soundSetNameHash);
        SetVehicleSoundSet(vehicle, soundSet);
    }
    public static void SetVehicleSoundSet(Vehicle vehicle, string soundSetName) =>
        SetVehicleSoundSet(vehicle, Game.GetHashKey(soundSetName));

    public static unsafe SoundSetWrapper GetVehicleSoundSet(Vehicle vehicle)
    {
        if (!AssertSafe(vehicle)) return null;

        var ptr = vehicle.GetSirenSoundSetPtr();
        return new SoundSetWrapper
        {
            SoundSet = ptr->Data,
            NameHash = ptr->NameHash
        };
    }

    private static bool AssertSafe(Vehicle veh)
    {
        if (!veh)
        {
            "Attempted to call function with invalid vehicle".ToLog();
            return false;
        }

        if (veh.IsDead)
        {
            "Attempted to call function with dead vehicle".ToLog();
            return false;
        }

        if (!veh.HasSiren)
        {
            "Attempted to call function with vehicle without a siren".ToLog();
            return false;
        }

        return true;
    }
}