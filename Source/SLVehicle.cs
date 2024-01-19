using System.Collections.Generic;
using System.Runtime.InteropServices;
using Rage;
using SLAPI.Lights;
using SLAPI.Memory;
using SLAPI.Utils;

namespace SLAPI;

public class SoundInstance
{
    private Vehicle _vehicle;

    internal SoundInstance(Vehicle vehicle)
    {
        _vehicle = vehicle;

        unsafe
        {
            var ptr = _vehicle.GetSirenSoundSetPtr();
            DefaultSirenSounds = SoundSet.Get(ptr->NameHash);
        }
    }
    
    // Siren SoundSets
    public SoundSet DefaultSirenSounds { get; private set; }
    public SoundSet SirenSounds
    {
        get
        {
            unsafe
            {
                var ptr = _vehicle.GetSirenSoundSetPtr();
                return SoundSet.Get(ptr->NameHash);
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
}

public class LightInstance
{
    private Vehicle _vehicle;

    public SirenInstance SirenInstance => new(_vehicle);

    internal LightInstance(Vehicle vehicle)
    {
        _vehicle = vehicle;
    }
}

public class SLVehicle
{
    internal static readonly Dictionary<Vehicle, SLVehicle> SLVehicles = new();
    
    private Vehicle _vehicle;

    public SoundInstance Sounds;
    public LightInstance Lights;

    public static SLVehicle Get(Vehicle vehicle)
    {
        // Returns existing SLVehicle, if existing
        if (SLVehicles.TryGetValue(vehicle, out var slVehicle))
            return slVehicle;
        
        // Creates/returns new SLVehicle
        slVehicle = new SLVehicle()
        {
            _vehicle = vehicle,
            Lights = new LightInstance(vehicle),
            Sounds = new SoundInstance(vehicle),
        };
        
        SLVehicles[vehicle] = slVehicle;
        return slVehicle;
    }
    
    private SLVehicle() {}
}