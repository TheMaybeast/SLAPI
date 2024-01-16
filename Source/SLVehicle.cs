using System.Collections.Generic;
using System.Runtime.InteropServices;
using Rage;
using SLAPI.Memory;
using SLAPI.Memory.Patches;
using SLAPI.Utils;

namespace SLAPI;

public class SLVehicle
{
    internal static readonly Dictionary<Vehicle, SLVehicle> SLVehicles = new();
    
    private Vehicle _vehicle;
    
    // SoundSet
    public SirenSoundSet DefaultSirenSounds { get; private set; }
    public SirenSoundSet SirenSounds
    {
        get
        {
            unsafe
            {
                var ptr = _vehicle.GetSirenSoundSetPtr();
                return SoundSet.Get(ptr->NameHash) as SirenSoundSet;
            }
        }
        set
        {
            unsafe
            {
                var ptr = _vehicle.GetSirenSoundSetPtr();
                ptr->NameHash = value.NameHash;
                ptr->Data = value.Sounds.SoundSetStruct;
            }
        }
    }

    // Siren Blip on Fast Toggle
    public static bool SirenBlipOnFastToggle
    {
        get => !BlipPatch.Patched;
        set
        {
            if (!value)
                BlipPatch.Patch();
            else
                BlipPatch.Remove();
        } 
    }

    // Siren State
    public eSirenState SirenState
    {
        get
        {
            if (!_vehicle.AssertSafe()) return 0;
            return (eSirenState)Marshal.ReadInt32(_vehicle.GetAudVehicleAudioEntityPtr() +
                                                  GameOffsets.audVehicleAudioEntity_SirenStateOffset);
        }
        set
        {
            if (!_vehicle.AssertSafe()) return;
            Marshal.WriteInt32(
                _vehicle.GetAudVehicleAudioEntityPtr() + GameOffsets.audVehicleAudioEntity_SirenStateOffset, (int)value);
        }
    }
    
    // Siren Time
    public uint SirenTime
    {
        get
        {
            if (!_vehicle.AssertSafe()) return 0;
            return (uint)Marshal.ReadInt32(_vehicle.GetAudVehicleAudioEntityPtr(),
                GameOffsets.audVehicleAudioEntity_SirenTimeOffset);
        }
        set
        {
            if (!_vehicle.AssertSafe()) return;
            Marshal.WriteInt32(_vehicle.GetAudVehicleAudioEntityPtr(), GameOffsets.audVehicleAudioEntity_SirenTimeOffset,
                (int)value);
        }
    }
    
    // Siren Last Change Time
    public uint SirenLastChangeTime
    {
        get
        {
            if (!_vehicle.AssertSafe()) return 0;
            return (uint)Marshal.ReadInt32(_vehicle.GetAudVehicleAudioEntityPtr() +
                                           GameOffsets.audVehicleAudioEntity_LastSirenChangeTimeOffset);
        }
    }
    
    // Siren Sequential Presses
    public uint SirenSequentialPresses
    {
        get
        {
            if (!_vehicle.AssertSafe()) return 0;
            return (uint)Marshal.ReadInt32(_vehicle.MemoryAddress + GameOffsets.CVehicle_SequentialSirenPressesOffset);
        }
    }
    
    // Horn Status
    public bool HornStatus => GameFunctions.IsHornOn(_vehicle.MemoryAddress);

    public static SLVehicle Get(Vehicle vehicle)
    {
        // Returns existing SLVehicle, if existing
        if (SLVehicles.TryGetValue(vehicle, out var slVehicle))
            return slVehicle;
        
        // Creates/returns new SLVehicle
        unsafe
        {
            var ptr = vehicle.GetSirenSoundSetPtr();
            var defaultSoundSet = SoundSet.Get(ptr->NameHash) as SirenSoundSet;
            
            slVehicle = new SLVehicle()
            {
                _vehicle = vehicle,
                DefaultSirenSounds = defaultSoundSet
            };
        }
        
        SLVehicles[vehicle] = slVehicle;
        return slVehicle;
    }
    
    private SLVehicle() {}
}