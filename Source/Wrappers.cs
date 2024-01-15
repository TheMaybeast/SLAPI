using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Rage;
using SLAPI.Memory;
using SLAPI.Memory.Patches;
using SLAPI.Utils;

namespace SLAPI;

public unsafe class SoundSet
{
    internal static readonly List<SoundSet> SoundSets = new();
    
    internal SoundSetStruct* SoundSetStruct;
    public bool Vanilla { get; private set; }

    public uint NameHash;
    
    // Properties
    public uint Blip
    {
        get => SoundSetStruct->GetSound("blip");
        set
        {
            if (Vanilla)
            {
                "Attempting to edit a vanilla SoundSet".ToLog(LogLevel.ERROR);
                return;
            }

            SoundSetStruct->SetSound("blip", value);
        }
    }
    public uint Horn
    {
        get => SoundSetStruct->GetSound("horn");
        set
        {
            if (Vanilla)
            {
                "Attempting to edit a vanilla SoundSet".ToLog(LogLevel.ERROR);
                return;
            }

            SoundSetStruct->SetSound("horn", value);
        }
    }
    public uint Fucked
    {
        get => SoundSetStruct->GetSound("fucked");
        set
        {
            if (Vanilla)
            {
                "Attempting to edit a vanilla SoundSet".ToLog(LogLevel.ERROR);
                return;
            }

            SoundSetStruct->SetSound("fucked", value);
        }
    }
    public uint FuckedOneShot
    {
        get => SoundSetStruct->GetSound("fucked_one_shot");
        set
        {
            if (Vanilla)
            {
                "Attempting to edit a vanilla SoundSet".ToLog(LogLevel.ERROR);
                return;
            }

            SoundSetStruct->SetSound("fucked_one_shot", value);
        }
    }
    public uint Fast
    {
        get => SoundSetStruct->GetSound("fast");
        set
        {
            if (Vanilla)
            {
                "Attempting to edit a vanilla SoundSet".ToLog(LogLevel.ERROR);
                return;
            }

            SoundSetStruct->SetSound("fast", value);
        }
    }
    public uint Slow
    {
        get => SoundSetStruct->GetSound("slow");
        set
        {
            if (Vanilla)
            {
                "Attempting to edit a vanilla SoundSet".ToLog(LogLevel.ERROR);
                return;
            }

            SoundSetStruct->SetSound("slow", value);
        }
    }
    public uint HornFast
    {
        get => SoundSetStruct->GetSound("horn_fast");
        set
        {
            if (Vanilla)
            {
                "Attempting to edit a vanilla SoundSet".ToLog(LogLevel.ERROR);
                return;
            }

            SoundSetStruct->SetSound("horn_fast", value);
        }
    }
    public uint HornSlow
    {
        get => SoundSetStruct->GetSound("horn_slow");
        set
        {
            if (Vanilla)
            {
                "Attempting to edit a vanilla SoundSet".ToLog(LogLevel.ERROR);
                return;
            }

            SoundSetStruct->SetSound("horn_slow", value);
        }
    }
    public uint Warning
    {
        get => SoundSetStruct->GetSound("warning");
        set
        {
            if (Vanilla)
            {
                "Attempting to edit a vanilla SoundSet".ToLog(LogLevel.ERROR);
                return;
            }

            SoundSetStruct->SetSound("warning", value);
        }
    }

    private SoundSet() {}

    public SoundSet Clone(string name = null)
    {
        // Allocates new SoundSet struct
        var soundSetPtr = Manager.Allocate<SoundSetStruct>();
        
        // Copies source into new SoundSet struct
        var source = Marshal.PtrToStructure<SoundSetStruct>((IntPtr)SoundSetStruct);
        Marshal.StructureToPtr(source, (IntPtr)soundSetPtr, false);
        
        // Creates new empty SoundSet
        var soundSet = new SoundSet()
        {
            NameHash = Game.GetHashKey(name ?? $"new-soundset-{Game.TickCount}"),
            SoundSetStruct = soundSetPtr
        };
        SoundSets.Add(soundSet);
        return soundSet;
    }

    public void Dump(bool toConsole)
    {
        $"SoundSet: {NameHash} ({NameHash:X})".ToLog(toConsole: toConsole);
        $"  #0 blip: {Blip:X}".ToLog(toConsole: toConsole);
        $"  #1 horn: {Horn:X}".ToLog(toConsole: toConsole);
        $"  #2 fucked: {Fucked:X}".ToLog(toConsole: toConsole);
        $"  #3 fucked_one_shot: {FuckedOneShot:X}".ToLog(toConsole: toConsole);
        $"  #4 fast: {Fast:X}".ToLog(toConsole: toConsole);
        $"  #5 slow: {Slow:X}".ToLog(toConsole: toConsole);
        $"  #6 horn_fast: {HornFast:X}".ToLog(toConsole: toConsole);
        $"  #7 horn_slow: {HornSlow:X}".ToLog(toConsole: toConsole);
        $"  #8 warning: {Warning:X}".ToLog(toConsole: toConsole);
    }
    
    // Gets new or existing instances
    public static SoundSet Get(uint hash)
    {
        // Attempts to return existing custom instance
        var soundSet = SoundSets.FirstOrDefault(x => x.NameHash == hash);
        if (soundSet != null) return soundSet;
        
        // Attempts to return existing vanilla instance
        var audSoundSetPtr = Manager.Allocate<audSoundSet>();
        audSoundSetPtr->Init(hash);
        var soundSetPtr = (SoundSetStruct*)Marshal.ReadIntPtr((IntPtr)audSoundSetPtr);
        Manager.Destroy(audSoundSetPtr);

        // Returns null if no existing SoundSet in game memory
        if ((IntPtr)soundSetPtr == IntPtr.Zero) return null;

        // Creates new vanilla SoundSet and adds to list
        soundSet = new SoundSet()
        {
            NameHash = hash,
            SoundSetStruct = soundSetPtr,
            Vanilla = true
        };
        SoundSets.Add(soundSet);
        return soundSet;
    }
    public static SoundSet Get(string name) => Get(Game.GetHashKey(name));
}

public class SLVehicle
{
    internal static readonly Dictionary<Vehicle, SLVehicle> SLVehicles = new();
    
    private Vehicle _vehicle;
    
    // SoundSet
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
                ptr->Data = value.SoundSetStruct;
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
            var defaultSoundSet = SoundSet.Get(ptr->NameHash);
            
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