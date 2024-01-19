using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Rage;
using SLAPI.Memory;
using SLAPI.Utils;

namespace SLAPI;

public unsafe class Sounds
{
    // Pointer to SoundSetStruct relating to this SoundSet
    internal readonly SoundSetStruct* SoundSetStruct;
    internal readonly bool Vanilla;

    internal Sounds(SoundSetStruct* soundSetStruct, bool vanilla = false)
    {
        SoundSetStruct = soundSetStruct;
        Vanilla = vanilla;
    }
    
    // Amount of sounds
    public uint Count => SoundSetStruct->SoundCount;
    
    // Indexer to get/set sounds
    public uint this[uint scriptNameHash]
    {
        get => SoundSetStruct->GetSound(scriptNameHash);
        set
        {
            if (Vanilla)
            {
                "Attempting to edit a vanilla SoundSet".ToLog(LogLevel.ERROR);
                return;
            }
            SoundSetStruct->SetSound(scriptNameHash, value);   
        }
    }
    public (uint scriptName, uint metadataRef) this[int index]
    {
        get => (SoundSetStruct->Sounds[index].Name, SoundSetStruct->Sounds[index].MetadataRef);
        set
        {
            if (Vanilla)
            {
                "Attempting to edit a vanilla SoundSet".ToLog(LogLevel.ERROR);
                return;
            }
            SoundSetStruct->Sounds[index] = new TSounds()
            {
                Name = value.metadataRef,
                MetadataRef = value.scriptName
            };
        }
    }
}

public unsafe class SoundSet
{
    // Stores all SoundSets obtained/created with SLAPI
    public static readonly List<SoundSet> SoundSets = new();

    public bool Vanilla
    {
        get => Sounds.Vanilla;
    }
    
    public uint NameHash { get; set; }
    public Sounds Sounds;

    private SoundSet() {}

    public SoundSet Clone(string name = null)
    {
        // Allocates new SoundSet struct
        var soundSetPtr = Manager.Allocate<SoundSetStruct>();
        
        // Copies source into new SoundSet struct
        var source = Marshal.PtrToStructure<SoundSetStruct>((IntPtr)Sounds.SoundSetStruct);
        Marshal.StructureToPtr(source, (IntPtr)soundSetPtr, false);
        
        // Creates new empty SoundSet
        var soundSet = new SoundSet()
        {
            Sounds = new Sounds(soundSetPtr),
            NameHash = Game.GetHashKey(name ?? $"new-soundset-{Game.TickCount}"),
        };
        SoundSets.Add(soundSet);
        return soundSet;
    }

    public void Dump(bool toConsole)
    {
        $"SoundSet: {NameHash} ({NameHash.HashParse(HashType.SoundSet)})".ToLog(toConsole: toConsole);
        $"Num of Sounds: {Sounds.Count}".ToLog(toConsole: toConsole);

        for (var i = 0; i < Sounds.Count; i++)
            $"  #{i} {Sounds[i].scriptName.HashParse(HashType.ScriptName)}: {Sounds[i].metadataRef.HashParse(HashType.MetadataRef)}".ToLog(toConsole: toConsole);
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

        // If existing in game memory, creates new SoundSet and adds to list
        soundSet = new SoundSet()
        {
            Sounds = new Sounds(soundSetPtr, true),
            NameHash = hash,
        };
        SoundSets.Add(soundSet);
        return soundSet;
    }
    public static SoundSet Get(string name) => Get(Game.GetHashKey(name));
}